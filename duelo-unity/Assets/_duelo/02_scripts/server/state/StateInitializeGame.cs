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
            Match.SetState(MatchState.Initialize)
                .ContinueWith(() => Match.WaitForSyncState())
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateBeginRound());
                });
        }
    }
}