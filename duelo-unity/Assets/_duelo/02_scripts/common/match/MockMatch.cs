namespace Duelo.Client.Match
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using UnityEngine;

    public class MockMatch : IClientMatch, IServerMatch
    {
        #region Private Fields
        private readonly MatchDto _matchDto;
        #endregion

        #region IClientMatch Properties
        public string MatchId => _matchDto.MatchId;
        public string MapId => _matchDto.MapId;
        public Dictionary<PlayerRole, MatchPlayer> Players { get; private set; } = new();

        public MatchDto CurrentDto => _matchDto;
        MatchRoundDto IClientMatch.CurrentRound => _matchDto.Rounds.Last();

        public MatchPlayer DevicePlayer => Players.Values.FirstOrDefault(p => p.IsDevicePlayer);
        public Action<MatchDto, MatchDto> OnStateChange { get; set; }
        #endregion

        #region IServerMatch Properties
        MatchRound IServerMatch.CurrentRound => _rounds.Last();
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged { get; set; }
        #endregion

        #region Private Implementation Fields
        private readonly List<MatchRound> _rounds = new();
        #endregion

        #region Initialization
        public MockMatch(MatchDto matchDto)
        {
            _matchDto = matchDto;
        }
        #endregion

        #region IClientMatch Methods
        public void DispatchMovement(int actionId, Vector3 position)
        {
            Debug.Log($"[MockMatch] Dispatching movement for {DevicePlayer.Role}: {actionId} to {position}");
        }

        public void DispatchAttack(int actionId)
        {
            Debug.Log($"[MockMatch] Dispatching attack for {DevicePlayer.Role}: {actionId}");
        }

        public UniTask JoinMatch()
        {
            Debug.Log($"[MockMatch] Joining match {_matchDto.MatchId}");
            return UniTask.CompletedTask;
        }

        public UniTask PublishSyncState(string state)
        {
            Debug.Log($"[MockMatch] Publishing sync state: {state}");
            return UniTask.CompletedTask;
        }

        public void Dispose() { }
        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, CurrentDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, CurrentDto.Players.Defender);
        }
        #endregion

        #region Helpers
        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(null, role, playerDto);
            Debug.Log($"[ClientMatch] Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region IServerMatch Methods
        public UniTask<MatchRound> NewRound()
        {
            uint timeAllowedMs = _matchDto.ClockConfig.InitialTimeAllowedMs;
            var newRound = new MatchRound(_rounds.Count, timeAllowedMs, null);
            _rounds.Add(newRound);

            return UniTask.FromResult(newRound);
        }

        public UniTask PublishSyncState(MatchState state)
        {
            Debug.Log("[MockMatch] Publishing sync state");
            return UniTask.CompletedTask;
        }

        public UniTask WaitForSyncState(MatchState newState)
        {
            Debug.Log("[MockMatch] Waiting for sync state");
            return UniTask.CompletedTask;
        }
        #endregion
    }
}