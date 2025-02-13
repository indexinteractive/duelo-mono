namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Match;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Services.Matchmaker.Models;
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
    public class ServerMatch : IServerMatch
    {
        #region Match Properties
        public string MatchId { get; private set; }
        public string MapId { get; private set; }
        public DateTime CreatedTime { get; private set; }
        public MatchState State { get; private set; }
        public readonly MatchmakingResults MatchmakerData;

        public readonly MatchPlayersDto PlayersDto;

        private readonly List<MatchRound> _rounds = new();
        public MatchRound CurrentRound => _rounds.LastOrDefault();

        public Dictionary<PlayerRole, MatchPlayer> Players { get; private set; } = new();
        #endregion

        #region Private Fields
        private readonly MatchClock _clock;
        private SyncStateDto _dbSyncState;
        #endregion

        #region Firebase References
        public DatabaseReference MatchRef => MatchService.Instance.GetRef(DueloCollection.Match, MatchId);
        public DatabaseReference SyncRef => MatchRef.Child("sync");
        #endregion

        #region Player Properties
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged { get; set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Publishes all match data to firebase as a <see cref="MatchDto"/>
        /// </summary>
        public ServerMatch(MatchmakingResults matchmakerData)
        {
            if (matchmakerData == null)
            {
                Debug.LogError("[ServerMatch] Matchmaker data is null");
                Application.Quit(Duelo.Common.Util.ExitCode.MatchNotFound);
            }

            if (matchmakerData.MatchProperties.Players.Count != 2)
            {
                Debug.LogError("[ServerMatch] Matchmaker data should have 2 players");
                Application.Quit(Duelo.Common.Util.ExitCode.InvalidMatch);
            }

            MatchmakerData = matchmakerData;

            MatchId = matchmakerData.MatchId;
            State = MatchState.Startup;
            // TODO: Should have map selection logic based on matchmaker data
            MapId = "devmap";
            CreatedTime = DateTime.UtcNow;

            _clock = new MatchClock();

            PlayersDto = MatchPlayersDto.FromMatchmakerData(matchmakerData);
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
        public async UniTask<MatchRound> NewRound()
        {
            _clock.NewRound();

            CurrentRound?.End();
            var newRound = new MatchRound(_rounds.Count, _clock.CurrentTimeAllowedMs, MatchRef);
            _rounds.Add(newRound);

            await CurrentRound.Publish();

            return newRound;
        }
        #endregion

        #region Player Management
        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, PlayersDto.Challenger);
            SpawnPlayer(PlayerRole.Defender, PlayersDto.Defender);
        }

        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"[ServerMatch] Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            var playerRef = MatchRef.Child("players").Child(role.ToString().ToLower());
            matchPlayer.Initialize(playerRef, role, playerDto);
            matchPlayer.OnStatusChanged += UpdatePlayersConnection;

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region Server / Client Sync State
        public async UniTask PublishSyncState(MatchState newState)
        {
            State = newState;

            var data = new SyncStateDto()
            {
                Server = State,
                Round = CurrentRound?.RoundNumber,
                Challenger = null,
                Defender = null
            };

            string json = JsonConvert.SerializeObject(data);

            Debug.Log($"[ServerMatch] Publishing sync state to firebase -- {json}");
            await SyncRef.SetRawJsonValueAsync(json);
        }

        private void OnSyncStateChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError($"[FirebaseMatch] Error reading sync state: {e.DatabaseError.Message}");
                return;
            }

            if (e.Snapshot.Exists)
            {
                string jsonValue = "";
                try
                {
                    jsonValue = e.Snapshot.GetRawJsonValue();
                    _dbSyncState = JsonConvert.DeserializeObject<SyncStateDto>(jsonValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FirebaseMatch] Error parsing sync state: {ex.Message}");
                    Debug.Log(jsonValue);
                }
            }
        }

        /// <summary>
        /// Waits for the server to be in sync with both player clients.
        /// Sync is achieved when the server state matches the value
        /// in the sync/:playerId/state node for both players
        /// </summary>
        public async UniTask WaitForSyncState(MatchState newState)
        {
            await PublishSyncState(newState);

            SyncRef.ValueChanged += OnSyncStateChanged;

            Debug.Log($"[ServerMatch] Waiting for states to sync to {{ {State} }}");
            await UniTask.WaitUntil(() => _dbSyncState != null
                && _dbSyncState.Challenger == State
                && _dbSyncState.Defender == State
            );

            SyncRef.ValueChanged -= OnSyncStateChanged;
        }
        #endregion

        #region Firebase
        /// <summary>
        /// Called when the match is ready for initial publish to firebase.
        /// <see cref="Server.State.StateRunServerMatch.OnEnter"/>
        /// </summary>
        /// <returns></returns>
        public async UniTask Publish()
        {
            var dto = new MatchDto
            {
                MatchId = MatchId,
                MapId = MapId,
                CreatedTime = CreatedTime,
                Players = PlayersDto,
                ClockConfig = _clock.ToDto(),
                MatchmakerDto = MatchmakerData,
                // These values should be empty on publish, since they will be updated throughout the match
                SyncState = null,
                Rounds = null
            };

            var json = JsonConvert.SerializeObject(dto);

            Debug.Log($"[ServerMatch] Pushing match data to firebase -- {json}");
            await MatchRef.SetRawJsonValueAsync(json).AsUniTask();
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            foreach (var player in Players)
            {
                player.Value.OnStatusChanged -= UpdatePlayersConnection;
            }

            SyncRef.ValueChanged -= OnSyncStateChanged;
        }
        #endregion
    }
}