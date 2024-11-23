namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Util;
    using UnityEngine;

    public class StateChooseMovement : ServerMatchState
    {
        #region Private Fields
        private Countdown _countdown;
        private const string DISPLAY_FORMAT = "00.00";
        #endregion

        #region GameState Implementation
        public override void OnEnter()
        {
            Debug.Log("StateChooseMovement");

            Match.CurrentRound.OnMovement(OnMovementReceived).ContinueWith(() =>
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
        private void OnMovementReceived(MovementPhaseDto movement)
        {
            if (movement?.Challenger?.Position != null && movement?.Defender?.Position != null)
            {
                Debug.Log("Both players have chosen their movements");
            }
        }

        private void OnCountdownUpdated(float timeLeft)
        {
            Debug.Log("[StateChooseMovement] Time left: " + timeLeft.ToString(DISPLAY_FORMAT));
        }

        private void OnCountdownFinished()
        {
            Match.CurrentRound.OffMovement(OnMovementReceived);
            StateMachine.SwapState(new StateChooseAction());
        }
        #endregion
    }
}