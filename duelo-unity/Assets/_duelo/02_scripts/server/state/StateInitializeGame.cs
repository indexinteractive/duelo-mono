namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;

    public class StateInitializeGame : ServerMatchState
    {
        public override void OnEnter()
        {
            Match.SetState(MatchState.InGame).Save().ContinueWith(success =>
            {
                StateMachine.SwapState(new StateBeginRound());
            });
        }
    }
}