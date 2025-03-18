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
        private ServerMatch _match => GlobalState.Match as ServerMatch;
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
                // .ContinueWith(StateInitializeRounds)
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

            GlobalState.Kernel.RegisterEntities(_match.Players[PlayerRole.Defender], _match.Players[PlayerRole.Challenger]);

            await UniTask.Yield();
        }

        private async UniTask StateMatchLobby()
        {
            foreach (var p in _match.Players)
            {
                p.Value.Status.Value = ConnectionStatus.Online;
            }

            await UniTask.Yield();
        }

        private async UniTask StateMatchInitializeGame()
        {
            await UniTask.Yield();
        }

        // private async UniTask StateInitializeRounds()
        // {
        //     foreach (var round in MatchDto.Rounds)
        //     {
        //         await _match.NewRound();
        //         _match.CurrentRound.CurrentValue.PlayerMovement = round.Movement;
        //         _match.CurrentRound.PlayerAction = round.Action;
        //     }

        //     await UniTask.Yield();
        // }
        #endregion

        #region Execute Round State
        private async UniTask StateExecuteRound()
        {
            await UniTask.NextFrame()
                .ContinueWith(() =>
                {
                    GlobalState.Kernel.QueueMovementPhase(_match.CurrentRound.CurrentValue.PlayerMovement.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
                })
                .ContinueWith(GlobalState.Kernel.RunRound);
        }
        #endregion
    }
}