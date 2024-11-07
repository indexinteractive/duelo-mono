namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateChooseMovement : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateChooseMovement");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateChooseAction());
            });
        }
    }
}