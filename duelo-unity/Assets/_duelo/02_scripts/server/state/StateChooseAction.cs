namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;

    public class StateChooseAction : ServerMatchState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            UniTask
                .Delay(2000)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateLateActions());
                });
        }
    }
}