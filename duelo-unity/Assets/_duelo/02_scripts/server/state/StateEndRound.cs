namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateEndRound : ServerMatchState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("StateEndRound");

            UniTask
                .Delay(2000)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateBeginRound());
                });
        }
    }
}