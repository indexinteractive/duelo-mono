namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;

    public class StateMatchLobby : ServerMatchState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            UniTask
                .Delay(2000)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateInitializeGame());
                });
        }
    }
}