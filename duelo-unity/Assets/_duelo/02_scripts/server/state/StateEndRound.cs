namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateEndRound : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateEndRound");
            Match.WaitForSyncState(MatchState.EndRound)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateBeginRound());
                });
        }
    }
}