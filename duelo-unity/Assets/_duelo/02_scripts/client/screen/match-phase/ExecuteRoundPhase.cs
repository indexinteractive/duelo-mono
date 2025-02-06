namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using UnityEngine;

    public class ExecuteRoundPhase : GameScreen
    {
        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ExecuteRoundPhase] OnEnter");

            QueuePlayerMovement();
            GlobalState.Kernel.RunRound()
                .ContinueWith(OnExecuteComplete);
        }
        #endregion

        #region Helpers
        public void QueuePlayerMovement()
        {
            var challengerMovement = _match.CurrentRound.Movement?.Challenger;
            if (challengerMovement != null)
            {
                Debug.Log("[StateExecuteRound] Queueing challenger movement");
                var args = new object[] { challengerMovement.TargetPosition };
                GlobalState.Kernel.QueuePlayerAction(PlayerRole.Challenger, challengerMovement.ActionId, args);
            }

            var defenderMovement = _match.CurrentRound.Movement?.Defender;
            if (defenderMovement != null)
            {
                Debug.Log("[StateExecuteRound] Queueing defender movement");
                var args = new object[] { defenderMovement.TargetPosition };
                GlobalState.Kernel.QueuePlayerAction(PlayerRole.Defender, defenderMovement.ActionId, args);
            }
        }

        public void OnExecuteComplete()
        {
            Debug.Log("[StateExecuteRound] Execution complete");
            StateMachine.PopState();
        }
        #endregion
    }
}