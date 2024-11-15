namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateBeginRound : ServerMatchState
    {
        public override void OnEnter()
        {
            Match.NewRound().ContinueWith(result =>
            {
                Debug.Log("[StateBeginRound] Round started. Transitioning to choose movement.");
                StateMachine.SwapState(new StateChooseMovement());
            });
        }
    }
}