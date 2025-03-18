namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using UnityEngine;

    public class StateInitializeGame : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateInitializeGame] OnEnter");
            Server.WaitForSyncState(MatchState.Initialize)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateBeginRound());
                });
        }
    }
}