namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateExecuteRound : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateExecuteRound]");
            Match.WaitForSyncState(MatchState.ExecuteRound)
                .ContinueWith(QueuePlayerMovement)
                .ContinueWith(Kernel.RunRound)
                .ContinueWith(() => Match.WaitForSyncState(MatchState.ExecuteRoundFinished))
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateEndRound());
                });
        }

        public void QueuePlayerMovement()
        {
            var challengerMovement = Match.CurrentRound.PlayerMovement?.Challenger;
            if (challengerMovement != null)
            {
                Debug.Log("[StateExecuteRound] Queueing challenger movement");
                var args = new object[] { challengerMovement.TargetPosition };
                Kernel.QueuePlayerAction(PlayerRole.Challenger, challengerMovement.ActionId, args);
            }

            var defenderMovement = Match.CurrentRound.PlayerMovement?.Defender;
            if (defenderMovement != null)
            {
                Debug.Log("[StateExecuteRound] Queueing defender movement");
                var args = new object[] { defenderMovement.TargetPosition };
                Kernel.QueuePlayerAction(PlayerRole.Defender, defenderMovement.ActionId, args);
            }
        }
    }
}