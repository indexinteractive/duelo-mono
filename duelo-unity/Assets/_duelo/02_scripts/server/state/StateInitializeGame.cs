namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateInitializeGame : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateInitializeGame");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateBeginRound());
            });
        }
    }
}