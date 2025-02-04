namespace Duelo.Server.State
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Server.Match;
    using Ind3x.Extensions;
    using Ind3x.State;
    using Unity.Services.Core;
    using Unity.Services.Matchmaker;
    using Unity.Services.Matchmaker.Models;
    using UnityEngine;

    /// <summary>
    /// Mounted by <see cref="GameMain"/> in server mode and kicks off a match flow
    /// </summary>
    public class StateRunServerMatch : GameState
    {
        private StateMachine _matchStateMachine = new StateMachine();

        #region GameState Implementation
        public override void OnEnter()
        {
            Debug.Log("[StateRunServerMatch] OnEnter");

            InitializeUnityServices()
                .ContinueWith(FetchMatchmakingResults)
                .ContinueWith(async matchmakerData =>
                {
                    GlobalState.ServerMatch = new ServerMatch(matchmakerData);
                    try
                    {
                        await GlobalState.ServerMatch.Publish();
                        _matchStateMachine.PushState(new StateMatchStartup());
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[StateRunServerMatch] Failed to publish match {e.Message}");
                        Application.Quit(Duelo.Common.Util.ExitCode.InvalidMatch);
                    }
                });
        }

        public override StateExitValue OnExit()
        {
            GlobalState.ServerMatch?.Dispose();
            _matchStateMachine?.CurrentState?.OnExit();
            return null;
        }

        private async UniTask<MatchmakingResults> FetchMatchmakingResults()
        {
#if DUELO_LOCAL
            Debug.Log("[StateRunServerMatch] Running local match, create test matchmaking results");

            await UniTask.Yield();
            return CreateTestMatchmakingResults();
#else
            Debug.Log("[StateRunServerMatch] Fetching matchmaking results");
            var allocationData = Ind3x.Model.ServerAllocation.ReadServerJson();
            if (allocationData != null)
            {
                return await MatchmakerService.Instance.GetMatchmakingResults(allocationData.AllocatedUuid);
            }
#endif
        }

        public override void Update()
        {
            _matchStateMachine.Update();
        }

        public override void FixedUpdate()
        {
            _matchStateMachine.FixedUpdate();
        }
        #endregion

        #region Private Helpers
        public async UniTask InitializeUnityServices()
        {
            try
            {
                Debug.Log("[StateRunServerMatch] Initializing Unity Services");
                await UnityServices.InitializeAsync();
            }
            catch (System.Exception e)
            {
                Debug.Log("[StateRunServerMatch] Unity Services failed to initialize");
                Debug.LogError(e);
                Application.Quit(Duelo.Common.Util.ExitCode.UnityServicesFailed);
            }
        }

        private bool ValidateMatch(MatchDto match)
        {
            if (match == null)
            {
                Debug.LogError("[GameMain] Match is null (not found)");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(match.MatchId))
            {
                Debug.LogError("[GameMain] Match id is null or empty");
                return false;
            }
            else if (match.ClockConfig == null)
            {
                Debug.LogError("[GameMain] Match clock config is null");
                return false;
            }

            return true;
        }

#if DUELO_LOCAL
        private MatchmakingResults CreateTestMatchmakingResults()
        {
            var p1UnityPlayerId = "TEST_PLAYER_1";
            var p1 = new Player(p1UnityPlayerId, new PlayerProfileDto()
            {
                CharacterUnitId = "devCapsuleBlue",
                Gamertag = "TestPlayer1"
            });

            var p2UnityPlayerId = "TEST_PLAYER_2";
            var p2 = new Player(p2UnityPlayerId, new PlayerProfileDto()
            {
                CharacterUnitId = "devCapsuleRed",
                Gamertag = "TestPlayer2"
            });

            var teams = new List<Team>
            {
                new Team("challenger", "challenger", new List<string> { p1UnityPlayerId }),
                new Team("defender", "defender", new List<string> { p2UnityPlayerId })
            };

            var players = new List<Player> { p1, p2 };
            var properties = new MatchProperties(teams, players);

            return new MatchmakingResults(properties, null, null, null, null, null, GlobalState.StartupOptions.MatchId);
        }
#endif
        #endregion
    }
}