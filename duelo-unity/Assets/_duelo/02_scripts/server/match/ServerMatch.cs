namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
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
    public class ServerMatch
    {
        #region Data / DTO Fields
        private MatchDto _dto;

        public string MapId => _dto.MapId;
        public MatchPlayersDto PlayersDto => _dto.Players;
        #endregion

        #region Match Properties
        public string MatchId { get; private set; }

        public MatchState State { get; private set; }
        public readonly MatchClock Clock;

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
        public ServerMatch SetState(MatchState state)
        {
            State = state;
            return this;
        }

        public ServerMatch NewRound()
        {
            Clock.NewRound();

            var round = new MatchRound(this);
            Rounds.Add(round);

            return this;
        }
        #endregion

        #region Player Management
        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GameData.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GameData.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(MatchId, role, playerDto);
            matchPlayer.OnStatusChanged += UpdatePlayersConnection;

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region Firebase
        public async UniTask Save()
        {
            await MatchService.Instance.SetData(MatchId, ToDictionary());
            if (CurrentRound != null)
            {
                await CurrentRound.Save();
            }
        }

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
    }
}