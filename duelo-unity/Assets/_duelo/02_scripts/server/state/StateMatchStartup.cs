namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;
    using UnityEngine;

    public class StateMatchStartup : GameState
    {
        public override void OnEnter()
        {
            // TODO: Update state in firebase

            Debug.Log("StateMatchStartup");

            UniTask.Delay(2000).ContinueWith(() =>
            {
                StateMachine.PushState(new StateMatchLobby());
            });
        }
    }
}