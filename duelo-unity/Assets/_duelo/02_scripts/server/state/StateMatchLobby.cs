namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateMatchLobby : GameState
    {
        public override void OnEnter()
        {
            Debug.Log("StateMatchLobby");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateInitializeGame());
            });
        }
    }
}