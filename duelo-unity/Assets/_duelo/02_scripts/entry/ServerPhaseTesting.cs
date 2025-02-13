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
        private IServerMatch _match => GlobalState.ServerMatch;
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

            GlobalState.ServerMatch = new MockMatch(MatchDto);

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

            GlobalState.Kernel.RegisterEntities(_match.Players.Values.ToArray());

            await UniTask.Yield();
        }

        private async UniTask StateMatchLobby()
        {
            foreach (var p in _match.Players.Values)
            {
                p.Status = ConnectionStatus.Online;
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
                _match.CurrentRound.PlayerMovement = round.Movement;
                _match.CurrentRound.PlayerAction = round.Action;
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
                    GlobalState.Kernel.QueueMovementPhase(_match.CurrentRound.PlayerMovement);
                })
                .ContinueWith(GlobalState.Kernel.RunRound);
        }
        #endregion

        #region Helpers
        private MatchmakingResults CreateTestMatchmakingResults()
        {
            var p1UnityPlayerId = MatchDto.Players.Challenger.UnityPlayerId;
            var p1 = new Player(p1UnityPlayerId, new PlayerProfileDto()
            {
                CharacterUnitId = MatchDto.Players.Challenger.Profile.CharacterUnitId,
                Gamertag = MatchDto.Players.Challenger.Profile.Gamertag
            });

            var p2UnityPlayerId = MatchDto.Players.Defender.UnityPlayerId;
            var p2 = new Player(p2UnityPlayerId, new PlayerProfileDto()
            {
                CharacterUnitId = MatchDto.Players.Defender.Profile.CharacterUnitId,
                Gamertag = MatchDto.Players.Defender.Profile.Gamertag
            });

            var teams = new List<Team>
            {
                new Team("challenger", "challenger", new List<string> { p1UnityPlayerId }),
                new Team("defender", "defender", new List<string> { p2UnityPlayerId })
            };

            var players = new List<Player> { p1, p2 };
            var properties = new MatchProperties(teams, players);

            return new MatchmakingResults(properties, null, null, null, null, null, MatchDto.MatchId);
        }
        #endregion
    }
}