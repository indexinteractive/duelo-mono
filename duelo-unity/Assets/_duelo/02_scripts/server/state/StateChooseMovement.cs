namespace Duelo.Server.State
{
    using Duelo.Common.Model;
    using UnityEngine;

    public class StateChooseMovement : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateChooseMovement");

            Match.CurrentRound.OnMovement(OnMovementReceived);
        }

        private void OnMovementReceived(MovementPhaseDto movement)
        {
            if (movement?.challenger?.Position != null && movement?.defender?.Position != null)
            {
                Debug.Log("Both players have chosen their movements");
                OnReadyToGo();
            }
        }

        private void OnReadyToGo()
        {
            Match.CurrentRound.OffMovement(OnMovementReceived);
            StateMachine.SwapState(new StateChooseAction());
        }
    }
}