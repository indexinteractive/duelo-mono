namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateLateActions : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateLateActions");

            UniTask
                .Delay(2000)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateExecuteRound());
                });
        }
    }
}