namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateLateActions : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateLateActions");
            Match.WaitForSyncState(MatchState.LateActions)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateExecuteRound());
                });
        }
    }
}