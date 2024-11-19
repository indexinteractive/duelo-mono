namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateChooseAction : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateChooseAction");

            UniTask
                .Delay(2000)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateLateActions());
                });
        }
    }
}