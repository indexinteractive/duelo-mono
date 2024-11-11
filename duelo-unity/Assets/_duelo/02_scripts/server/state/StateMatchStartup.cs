namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Ind3x.State;

    /// <summary>
    /// One time startup state for matches
    /// </summary>
    public class StateMatchStartup : ServerMatchState
    {
        public override void OnEnter()
        {
            UpdateDbState().ContinueWith(_ =>
            {
                StateMachine.SwapState(new StateMatchLobby());
            });
        }
    }
}