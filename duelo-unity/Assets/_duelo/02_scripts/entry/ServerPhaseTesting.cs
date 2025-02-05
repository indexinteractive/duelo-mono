namespace Duelo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Gameboard;
    using Duelo.Server.Match;
    using Duelo.Server.State;
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

        [Tooltip("The matchmaking results that would come from the matchmaker service during a game")]
        #endregion

        #region Private Fields
        private MatchmakingResults _matchmakingResults;
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            Debug.Log("[ServerPhaseTesting] Starting server phase testing scene");
            yield return Ind3x.Util.FirebaseInstance.Instance.Initialize("PHASE_TESTING", false);

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

            GlobalState.StartupOptions = new StartupOptions(StartupMode.Server, new string[] {
                "--matchId", MatchDto.MatchId
            });

            _matchmakingResults = CreateTestMatchmakingResults();
            GlobalState.ServerMatch = new ServerMatch(_matchmakingResults);

            await UniTask.Yield();
        }
        #endregion

        #region Server States
        private async UniTask StateMatchStartup()
        {
            GlobalState.Kernel = new MatchKernel();
            DueloMapDto mapDto = await MapService.Instance.GetMap(MatchDto.MapId);
            GlobalState.Map.Load(mapDto);

            var match = GlobalState.ServerMatch;
            match.SpawnPlayer(PlayerRole.Challenger, match.PlayersDto.Challenger);
            match.SpawnPlayer(PlayerRole.Defender, match.PlayersDto.Defender);

            GlobalState.Kernel.RegisterEntities(match.Players.Values.ToArray());

            await UniTask.Yield();
        }

        private async UniTask StateMatchLobby()
        {
            foreach (var p in GlobalState.ServerMatch.Players.Values)
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
            var match = GlobalState.ServerMatch;
            foreach (var round in MatchDto.Rounds)
            {
                match.Rounds.Add(new MatchRound(match));
                match.CurrentRound.PlayerMovement = round.Movement;
                match.CurrentRound.PlayerAction = round.Action;
            }

            await UniTask.Yield();
        }
        #endregion

        #region Execute Round State
        private async UniTask StateExecuteRound()
        {
            // This is not exactly how this should be tested
            // but for now it is too much work to mock GlobalState.ServerMatch
            // so it will do
            var stateExecute = new StateExecuteRound();

            await UniTask.NextFrame()
                .ContinueWith(stateExecute.QueuePlayerMovement)
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