namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Util;
    using UnityEngine;

    public class StateChooseAction : ServerMatchState
    {
        #region Private Fields
        private Countdown _countdown;
        private float _lastLoggedTime = -1f;
        private const string DISPLAY_FORMAT = "00.00";
        #endregion

        #region GameState Implementation
        public override void OnEnter()
        {
            Debug.Log("StateChooseAction");
            Match.SetState(MatchState.ChooseAction)
                .Save()
                .ContinueWith(() => Match.CurrentRound.KickoffActions(OnActionsReceived))
                .ContinueWith(() => Match.WaitForSyncState())
                .ContinueWith(() =>
                {
                    _countdown = new Countdown();
                    _countdown.OnCountdownUpdated += OnCountdownUpdated;
                    _countdown.OnCountdownFinished += OnCountdownFinished;
                    _countdown.StartTimer(Match.Clock.CurrentTimeAllowedMs);
                });
        }

        public override void Update()
        {
            _countdown?.Update();
        }
        #endregion

        #region Events
        private void OnActionsReceived(ActionPhaseDto actions)
        {
            if (actions?.Challenger?.ActionId != null && actions?.Defender?.ActionId != null)
            {
                Debug.Log("Both players have chosen their actions");
            }
        }

        private void OnCountdownUpdated(float timeLeft)
        {
            if (Mathf.FloorToInt(timeLeft) != Mathf.FloorToInt(_lastLoggedTime))
            {
                _lastLoggedTime = timeLeft;
                Debug.Log("[StateChooseAction] Time left: " + timeLeft.ToString(DISPLAY_FORMAT));
            }
        }

        private void OnCountdownFinished()
        {
            Match.CurrentRound.EndActions(OnActionsReceived);
            StateMachine.SwapState(new StateLateActions());
        }
        #endregion
    }
}