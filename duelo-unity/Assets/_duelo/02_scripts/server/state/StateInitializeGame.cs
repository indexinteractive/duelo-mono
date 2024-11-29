namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;

    public class StateInitializeGame : ServerMatchState
    {
        public override void OnEnter()
        {
            Match.SetState(MatchState.InGame).Save().ContinueWith(() =>
            {
                StateMachine.SwapState(new StateBeginRound());
            });
        }
    }
}