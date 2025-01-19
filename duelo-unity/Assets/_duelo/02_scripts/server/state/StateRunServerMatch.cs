namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Server.Match;
    using Ind3x.Extensions;
    using Ind3x.State;
    using Unity.Services.Core;
    using Unity.Services.Matchmaker;
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

            var allocationData = Ind3x.Model.ServerAllocation.ReadServerJson();

            InitializeUnityServices()
                .ContinueWith(() => MatchmakerService.Instance.GetMatchmakingResults(allocationData.AllocatedUuid))
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
            catch (System.Exception)
            {
                Debug.Log("[StateRunServerMatch] Unity Services failed to initialize");
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
        #endregion
    }
}