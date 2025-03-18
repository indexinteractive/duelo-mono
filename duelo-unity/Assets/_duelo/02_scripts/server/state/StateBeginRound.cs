namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateBeginRound : ServerMatchState
    {
        public override void OnEnter()
        {
            Server.NewRound()
                .ContinueWith(_ => Server.WaitForSyncState(MatchState.BeginRound))
                .ContinueWith(() =>
                {
                    Debug.Log("[StateBeginRound] Round started. Transitioning to choose movement.");
                    StateMachine.SwapState(new StateChooseMovement());
                });
        }
    }
}