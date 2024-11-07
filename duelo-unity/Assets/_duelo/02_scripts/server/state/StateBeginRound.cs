namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateBeginRound : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateBeginRound");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateChooseMovement());
            });
        }
    }
}