namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Server.Match;
    using Ind3x.State;
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
            Debug.Log("StateRunServerMatch");

            MatchService.Instance.GetMatch(ServerData.StartupOptions.MatchId)
                .ContinueWith(matchDto =>
                {
                    Debug.Log("found match: " + matchDto.MatchId);

                    if (!ValidateMatch(matchDto))
                    {
                        Debug.LogError("[GameMain] Invalid match, crashing");
                        Application.Quit(1);
                    }

                    ServerData.Match = new ServerMatch(matchDto);
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