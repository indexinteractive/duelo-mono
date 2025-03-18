namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Database;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Services.Matchmaker.Models;
    using UnityEngine;

    /// <summary>
    /// Object that represents a match in the server. Communicates directly
    /// with firebase to perform updates on the match state.
    /// </summary>
    public class ServerMatch : ObservableMatch, IServerMatch
    {
        #region Match Properties
        public readonly MatchmakingResults MatchmakerData;
        #endregion

        #region Private Fields
        private readonly MatchClock _clock;
        private readonly IMatchDatabase Db;
        private HashSet<Action> _unsubscribers = new();
        #endregion

        #region Movement Phase
        private Action _movementChangedSub;
        private Action _actionChangedSub;
        #endregion

        #region Initialization
        /// <summary>
        /// Publishes all match data to firebase as a <see cref="MatchDto"/>
        /// </summary>
        public ServerMatch(MatchmakingResults matchmakerData, IMatchDatabase db) : base(matchmakerData)
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
            Db = db;

            _clock = new MatchClock(ClockConfig);
        }
        #endregion

        #region Match States
        public async UniTask<MatchRound> NewRound()
        {
            _clock.NewRound();

            Dictionary<PlayerRole, PlayerRoundStateDto> states;
            if (Rounds.Count == 0)
            {
                states = new()
                {
                    { PlayerRole.Challenger, Players[PlayerRole.Challenger].GetRoundStateDto() },
                    { PlayerRole.Defender, Players[PlayerRole.Defender].GetRoundStateDto() }
                };
            }
            else
            {
                var previousRound = Rounds.Last();
                var previousState = previousRound.GetRoundStatesDto();
                states = new()
                {
                    { PlayerRole.Challenger, previousState.Challenger },
                    { PlayerRole.Defender, previousState.Defender }
                };

                var dto = previousRound.End();
                await Db.UpdateRound(previousRound.RoundNumber, dto);
            }

            var newRound = new MatchRound(Rounds.Count, _clock.CurrentTimeAllowedMs, states);
            Rounds.Add(newRound);

            await Db.PublishRound(newRound.RoundNumber, newRound.Publish());

            return newRound;
        }
        #endregion

        #region Movement Phase
        /// <summary>
        /// Kicks off the movement phase by setting the timer and adds a
        /// callback to the movement node listener.
        /// Called in <see cref="State.StateChooseMovement.OnEnter"/>
        /// </summary>
        public async UniTask KickoffMovementPhase()
        {
            var round = CurrentRound.CurrentValue;
            var dto = round.KickoffMovement();

            _movementChangedSub = Db.Subscribe($"{SchemaMatchField.Rounds}/{round.RoundNumber}/{SchemaMatchRoundField.Movement}", OnMovementReceived);

            await Db.BeginMovementPhase(round.RoundNumber, dto);
        }

        private void OnMovementReceived(string json)
        {
            Debug.Log($"[MatchRound] MovementValueChanged: {json}");
            var data = JsonConvert.DeserializeObject<MovementPhaseDto>(json);
            if (data.Challenger != null || data.Defender != null)
            {
                CurrentRound.CurrentValue.UpdateMovement(data);
            }
        }

        public void EndMovementPhase()
        {
            _movementChangedSub?.Invoke();
        }
        #endregion

        #region Action Phase
        public async UniTask KickoffActionsPhase()
        {
            var round = CurrentRound.CurrentValue;
            var dto = round.KickoffActions();

            _actionChangedSub = Db.Subscribe($"{SchemaMatchField.Rounds}/{round.RoundNumber}/{SchemaMatchRoundField.Action}", OnActionsReceived);

            await Db.BeginActionPhase(round.RoundNumber, dto);
        }

        private void OnActionsReceived(string json)
        {
            Debug.Log($"[MatchRound] ActionValueChanged: {json}");
            var data = JsonConvert.DeserializeObject<ActionPhaseDto>(json);
            if (data.Challenger != null || data.Defender != null)
            {
                CurrentRound.CurrentValue.UpdateActions(data);
            }
        }

        public void EndActionsPhase()
        {
            _actionChangedSub?.Invoke();
        }
        #endregion

        #region Player Management
        public void OnMatchPlayersChanged(string json)
        {
            var playersDto = JsonConvert.DeserializeObject<MatchPlayersDto>(json);
            Players[PlayerRole.Challenger].UpdateFromDto(playersDto.Challenger);
            Players[PlayerRole.Defender].UpdateFromDto(playersDto.Defender);
        }

        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, InitialDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, InitialDto.Players.Defender);

            var sub = Db.Subscribe(SchemaMatchField.Players, OnMatchPlayersChanged);
            _unsubscribers.Add(sub);
        }

        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"[ServerMatch] Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(this, role, playerDto);

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region Server / Client Sync State
        public async UniTask PublishSyncState(MatchState newState)
        {
            SyncState.Server.Value = newState;

            var data = new SyncStateDto()
            {
                Server = SyncState.Server.Value,
                Challenger = null,
                Defender = null
            };

            await Db.PublishServerSyncState(data);
        }

        private void OnSyncStateChanged(string json)
        {
            var dbSyncState = JsonConvert.DeserializeObject<SyncStateDto>(json);
            SyncState.Update(dbSyncState);
        }

        /// <summary>
        /// Waits for the server to be in sync with both player clients.
        /// Sync is achieved when the server state matches the value
        /// in the sync/:playerId/state node for both players
        /// </summary>
        public async UniTask WaitForSyncState(MatchState newState)
        {
            await PublishSyncState(newState);

            var unsubscribe = Db.Subscribe(SchemaMatchField.Sync, OnSyncStateChanged);
            _unsubscribers.Add(unsubscribe);

            Debug.Log($"[ServerMatch] Waiting for states to sync to {{ {newState} }}");
            await UniTask.WaitUntil(() => SyncState != null
                && SyncState.Challenger.Value == newState
                && SyncState.Defender.Value == newState
            );

            _unsubscribers.Remove(unsubscribe);
            unsubscribe();
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

            await Db.PublishMatch(dto);
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            Db.Dispose();

            foreach (var unsub in _unsubscribers)
            {
                unsub();
            }

            _unsubscribers.Clear();
        }
        #endregion
    }
}