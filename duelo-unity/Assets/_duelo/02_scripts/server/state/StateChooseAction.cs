namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateChooseAction : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateChooseAction");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateLateActions());
            });
        }
    }
}