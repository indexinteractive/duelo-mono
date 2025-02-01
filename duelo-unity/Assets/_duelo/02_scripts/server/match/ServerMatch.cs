namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class ConnectionChangedEventArgs
    {
        public ConnectionStatus ChallengerStatus { get; set; }
        public ConnectionStatus DefenderStatus { get; set; }
    }

    /// <summary>
    /// Object that represents a match in the server. Communicates directly
    /// with firebase to perform updates on the match state.
    /// </summary>
    public class ServerMatch : IDisposable
    {
        #region Data / DTO Fields
        private MatchDto _dto;

        /// <summary>
        /// This value is a local cache for the sync state of the match, but it
        /// does not necessarily represent the actual state of the match. This is because
        /// the server is the truth, but clients need to update their state to match the server.
        /// </summary>
        private SyncStateDto _dbSyncState;

        public string MapId => _dto.MapId;
        public MatchPlayersDto PlayersDto => _dto.Players;
        #endregion

        #region Match Properties
        public string MatchId { get; private set; }

        public MatchState State { get; private set; }
        public readonly MatchClock Clock;
        private readonly DatabaseReference _syncRef;

        public readonly List<MatchRound> Rounds;
        public MatchRound CurrentRound => Rounds.LastOrDefault();

        public Dictionary<PlayerRole, MatchPlayer> Players = new();
        public DatabaseReference MatchRef => MatchService.Instance.GetRef(DueloCollection.Match, MatchId);
        #endregion

        #region Player Properties
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged;
        #endregion

        #region Initialization
        public ServerMatch(MatchDto dbData)
        {
            _dto = dbData;
            MatchId = dbData.MatchId;
            State = MatchState.Startup;

            Rounds = new List<MatchRound>();
            Clock = new MatchClock(dbData.ClockConfig);

            _syncRef = MatchRef.Child("sync");
            _syncRef.ValueChanged += OnSyncStateChanged;
        }
        #endregion

        #region Player Connections
        private void UpdatePlayersConnection(PlayerStatusChangedEvent e)
        {
            try
            {
                Debug.Log($"[FirebaseMatch] Player status changed: {string.Join(", ", Players.Select(x => $"{x.Value.Role}={x.Value.Status}"))}");

                var eventArgs = new ConnectionChangedEventArgs
                {
                    ChallengerStatus = Players[PlayerRole.Challenger].Status,
                    DefenderStatus = Players[PlayerRole.Defender].Status
                };

                OnPlayersConnectionChanged?.Invoke(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseMatch] Error checking player online status: {ex.Message}");
            }
        }
        #endregion

        #region Match States
        public async UniTask SetState(MatchState state)
        {
            State = state;

            await MatchRef.Child("state").SetValueAsync(state.ToString());
        }

        public ServerMatch NewRound()
        {
            Clock.NewRound();

            if (CurrentRound != null)
            {
                CurrentRound.End();
            }

            var round = new MatchRound(this);
            Rounds.Add(round);

            return this;
        }
        #endregion

        #region Player Management
        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"[ServerMatch] Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(MatchId, role, playerDto);
            matchPlayer.OnStatusChanged += UpdatePlayersConnection;

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region Server / Client Sync State
        public async UniTask PublishSyncState()
        {
            var data = new SyncStateDto()
            {
                Server = State,
                Round = CurrentRound?.RoundNumber,
                Challenger = null,
                Defender = null
            };

            string json = JsonConvert.SerializeObject(data);

            Debug.Log($"[ServerMatch] Publishing sync state to firebase -- {json}");
            await _syncRef.SetRawJsonValueAsync(json);
        }

        private void OnSyncStateChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError($"[FirebaseMatch] Error reading sync state: {e.DatabaseError.Message}");
                return;
            }

            try
            {
                string jsonValue = e.Snapshot.GetRawJsonValue();
                _dbSyncState = JsonConvert.DeserializeObject<SyncStateDto>(jsonValue);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseMatch] Error parsing sync state: {ex.Message}");
            }
        }

        /// <summary>
        /// Waits for the server to be in sync with both player clients.
        /// Sync is achieved when the server state matches the value
        /// in the sync/:playerId/state node for both players
        /// </summary>
        public async UniTask WaitForSyncState()
        {
            await PublishSyncState();

            Debug.Log($"[ServerMatch] Waiting for states to sync to {{ {State} }}");
            await UniTask.WaitUntil(() => _dbSyncState != null
                && _dbSyncState.Challenger == State
                && _dbSyncState.Defender == State
            );
        }
        #endregion

        #region Firebase
        /// <summary>
        /// Creates a dictionary of any match data that needs to be saved to firebase
        /// </summary>
        private Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                { "state", State.ToString() }
            };

            return data;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _syncRef.ValueChanged -= OnSyncStateChanged;
            foreach (var player in Players)
            {
                player.Value.OnStatusChanged -= UpdatePlayersConnection;
            }
        }
        #endregion
    }
}