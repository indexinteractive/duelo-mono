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
                .ContinueWith(() =>
                {
                    Kernel.QueueMovementPhase(Match.CurrentRound.PlayerMovement);
                    return Kernel.RunRound();
                })
                .ContinueWith(() => Match.WaitForSyncState(MatchState.ExecuteRoundFinished))
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateEndRound());
                });
        }
    }
}