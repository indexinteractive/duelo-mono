namespace Duelo.Server.State
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateExecuteRound : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateExecuteRound]");

            Server.WaitForSyncState(MatchState.ExecuteRound)
                .ContinueWith(() =>
                {
                    var round = Match.CurrentRound.CurrentValue;
                    Kernel.QueueMovementPhase(round.PlayerMovement.ToDictionary(kvp => kvp.Key, m => m.Value));
                    Kernel.QueueActionPhase(round.PlayerAction.ToDictionary(kvp => kvp.Key, a => a.Value));
                    return Kernel.RunRound();
                })
                .ContinueWith(() => Server.WaitForSyncState(MatchState.ExecuteRoundFinished))
                .ContinueWith(() =>
                {
                    GlobalState.Map.ClearPath(PlayerRole.Challenger);
                    GlobalState.Map.ClearPath(PlayerRole.Defender);
                })
                .ContinueWith(() => StateMachine.SwapState(new StateEndRound()));
        }
    }
}