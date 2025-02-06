namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using UnityEngine;

    public class ExecuteRoundPhase : GameScreen
    {
        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ExecuteRoundPhase] OnEnter");

            GlobalState.Kernel.QueueMovementPhase(_match.CurrentRound.Movement);
            GlobalState.Kernel.RunRound()
                .ContinueWith(OnExecuteComplete);
        }
        #endregion

        #region Helpers
        public void OnExecuteComplete()
        {
            Debug.Log("[StateExecuteRound] Execution complete");
            StateMachine.PopState();
        }
        #endregion
    }
}