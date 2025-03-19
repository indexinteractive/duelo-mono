namespace Duelo
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Match;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Gameboard;
    using Duelo.Server.Match;
    using Unity.Services.Matchmaker.Models;
    using UnityEngine;

    /// <summary>
    /// This class is a misnomer. It is intended for testing of <see cref="Server.State.StateExecuteRound"/>
    /// with data from a <see cref="Common.Model.MatchDto"/> as a stand in for player updates
    /// </summary>
    public class ServerPhaseTesting : MonoBehaviour
    {
        #region Public Properties
        [Header("Initialization Settings")]
        [Tooltip("The firebase MatchDto data that would come from firebase during a game")]
        public MatchDto MatchDto;
        #endregion

        #region Private Fields
        private IServerMatch _match => GlobalState.Match as IServerMatch;
        private MockService _services;
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            _services = new MockService(MatchDto);

            GlobalState.StateMachine = new Ind3x.State.StateMachine();
            GlobalState.Prefabs = FindAnyObjectByType<PrefabList>();
            GlobalState.Map = FindAnyObjectByType<DueloMap>();

            UniTask.Delay(1)
                .ContinueWith(LoadMatchData)
                .ContinueWith(StateMatchStartup)
                .ContinueWith(StateMatchLobby)
                .ContinueWith(StateMatchInitializeGame)
                .ContinueWith(StateInitializeRounds)
                .ContinueWith(StateExecuteRound);
        }
        #endregion

        #region Loading
        private async UniTask LoadMatchData()
        {
            Debug.Log("[ServerPhaseTesting] Loading db data");

            GlobalState.Match = new MockMatch(MatchDto);

            await UniTask.Yield();
        }
        #endregion

        #region Server States
        private async UniTask StateMatchStartup()
        {
            GlobalState.Kernel = new MatchKernel();
            DueloMapDto mapDto = await _services.GetMap(MatchDto.MapId);
            GlobalState.Map.Load(mapDto);

            _match.LoadAssets();

            GlobalState.Kernel.RegisterEntities(GlobalState.Match.Players[PlayerRole.Defender], GlobalState.Match.Players[PlayerRole.Challenger]);

            await UniTask.Yield();
        }

        private async UniTask StateMatchLobby()
        {
            foreach (var p in GlobalState.Match.Players)
            {
                p.Value.Status.Value = ConnectionStatus.Online;
            }

            await UniTask.Yield();
        }

        private async UniTask StateMatchInitializeGame()
        {
            await UniTask.Yield();
        }

        private async UniTask StateInitializeRounds()
        {
            foreach (var round in MatchDto.Rounds)
            {
                await _match.NewRound();
                GlobalState.Match.CurrentRound.CurrentValue.PlayerMovement[PlayerRole.Defender] = round.Movement.Defender;
                GlobalState.Match.CurrentRound.CurrentValue.PlayerMovement[PlayerRole.Challenger] = round.Movement.Challenger;
                GlobalState.Match.CurrentRound.CurrentValue.PlayerAction[PlayerRole.Defender] = round.Action.Defender;
                GlobalState.Match.CurrentRound.CurrentValue.PlayerAction[PlayerRole.Challenger] = round.Action.Challenger;
            }

            await UniTask.Yield();
        }
        #endregion

        #region Execute Round State
        private async UniTask StateExecuteRound()
        {
            await UniTask.NextFrame()
                .ContinueWith(() =>
                {
                    GlobalState.Kernel.QueueMovementPhase(GlobalState.Match.CurrentRound.CurrentValue.PlayerMovement.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                })
                .ContinueWith(GlobalState.Kernel.RunRound);
        }
        #endregion
    }
}