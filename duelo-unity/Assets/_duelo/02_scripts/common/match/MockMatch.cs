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

    public class MockMatch : ObservableMatch, IClientMatch, IServerMatch
    {
        #region Private Fields
        #endregion

        #region IClientMatch Properties
        public MatchPlayer DevicePlayer => Players.FirstOrDefault(p => p.Value.IsDevicePlayer).Value;
        #endregion

        #region IServerMatch Properties
        #endregion

        #region Private Implementation Fields
        private readonly List<MatchRound> _rounds = new();
        #endregion

        #region Initialization
        public MockMatch(MatchDto matchDto) : base(matchDto)
        {
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
            Debug.Log($"[MockMatch] Joining match {InitialDto.MatchId}");
            return UniTask.CompletedTask;
        }

        public UniTask PublishSyncState(string state)
        {
            Debug.Log($"[MockMatch] Publishing sync state: {state}");
            return UniTask.CompletedTask;
        }

        public override void Dispose() { }

        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, InitialDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, InitialDto.Players.Defender);
        }
        #endregion

        #region Helpers
        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(this, role, playerDto);
            Debug.Log($"[ClientMatch] Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region IServerMatch Methods
        public UniTask<MatchRound> NewRound()
        {
            uint timeAllowedMs = ClockConfig.InitialTimeAllowedMs;

            Dictionary<PlayerRole, PlayerRoundStateDto> states = null;
            if (_rounds.Count == 0)
            {
                states = new()
                {
                    { PlayerRole.Challenger, Players[PlayerRole.Challenger].GetRoundStateDto() },
                    { PlayerRole.Defender, Players[PlayerRole.Defender].GetRoundStateDto() }
                };
            }
            else
            {
                var previousRound = _rounds.Last();
                var previousState = previousRound.GetRoundStatesDto();
                states = new()
                {
                    { PlayerRole.Challenger, previousState.Challenger },
                    { PlayerRole.Defender, previousState.Defender }
                };

                previousRound.End();
            }

            var newRound = new MatchRound(_rounds.Count, timeAllowedMs, states);

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

        public UniTask KickoffMovementPhase()
        {
            throw new NotImplementedException();
        }

        public UniTask KickoffActions(Action<ActionPhaseDto> onActionReceived)
        {
            throw new NotImplementedException();
        }

        public void EndMovementPhase()
        {
            throw new NotImplementedException();
        }

        public UniTask KickoffActionsPhase()
        {
            throw new NotImplementedException();
        }

        public void EndActionsPhase()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}