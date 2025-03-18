namespace Duelo.Client.Screen
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using UnityEngine;

    public class ExecuteRoundPhase : GameScreen
    {
        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ExecuteRoundPhase] OnEnter");

            GlobalState.Map.ClearPath(_player.Role);

            _player.DestroyGhost();

            var round = Match.CurrentRound.CurrentValue;
            GlobalState.Kernel.QueueMovementPhase(round.PlayerMovement.ToDictionary(m => m.Key, m => m.Value));
            GlobalState.Kernel.QueueActionPhase(round.PlayerAction.ToDictionary(a => a.Key, a => a.Value));
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