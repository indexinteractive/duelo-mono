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
                .ContinueWith(matchmakerData => MatchService.Instance.CreateMatch(matchmakerData))
                .ContinueWith(matchDto =>
                {
                    if (matchDto == null)
                    {
                        Debug.LogError("[StateRunServerMatch] Match not found, crashing");
                        Application.Quit(Duelo.Common.Util.ExitCode.MatchNotFound);
                    }

                    Debug.Log("[StateRunServerMatch] found match: " + matchDto.MatchId);

                    if (!ValidateMatch(matchDto))
                    {
                        Debug.LogError("[GameMain] Invalid match, crashing");
                        Application.Quit(Duelo.Common.Util.ExitCode.InvalidMatch);
                    }

                    GameData.ServerMatch = new ServerMatch(matchDto);
                    _matchStateMachine.PushState(new StateMatchStartup());
                });
        }

        private async UniTask<MatchmakingResults> FetchMatchmakingResults()
        {
            var allocationData = Ind3x.Model.ServerAllocation.ReadServerJson();
            if (allocationData != null)
            {
                return await MatchmakerService.Instance.GetMatchmakingResults(allocationData.AllocatedUuid);
            }

            Debug.Log("[StateRunServerMatch] Allocation data is empty, assuming test scenario");

            return CreateTestMatchmakingResults();
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

        private MatchmakingResults CreateTestMatchmakingResults()
        {
            var p1 = new Player("challenger", new PlayerProfileDto() { CharacterUnitId = "devCapsuleBlue", Gamertag = "TestPlayer1" });
            var p2 = new Player("defender", new PlayerProfileDto() { CharacterUnitId = "devCapsuleRed", Gamertag = "TestPlayer2" });

            var teams = new List<Team>
            {
                new Team("challenger", "challenger", new List<string> { "challenger" }),
                new Team("defender", "defender", new List<string> { "defender" })
            };

            var players = new List<Player> { p1, p2 };
            var properties = new MatchProperties(teams, players);

            return new MatchmakingResults(properties, null, null, null, null, null, GameData.StartupOptions.MatchId);
        }
        #endregion
    }
}