namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateExecuteRound : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateExecuteRound");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateEndRound());
            });
        }
    }
}