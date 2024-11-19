namespace Duelo.Server.State
{
    using Duelo.Common.Model;
    using UnityEngine;

    public class StateChooseAction : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateChooseAction");

            Match.CurrentRound.OnActions(OnActionsReceived);
        }

        private void OnActionsReceived(ActionPhaseDto actions)
        {
            if (actions?.challenger?.ActionId != null && actions?.defender?.ActionId != null)
            {
                Debug.Log("Both players have chosen their actions");
                OnPlayerActionsCompleted();
            }
        }

        private void OnPlayerActionsCompleted()
        {
            StateMachine.SwapState(new StateLateActions());
        }
    }
}