namespace Duelo.Server.State
{
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Mounted by <see cref="GameMain"/> in server mode and kicks off a match flow
    /// </summary>
    public class StateRunServerMatch : GameState
    {
        private StateMachine _matchStateMachine;

        public override void OnEnter()
        {
            Debug.Log("StateRunServerMatch");

            _matchStateMachine = new StateMachine();
            _matchStateMachine.PushState(new StateMatchStartup());
        }

        public override void Update()
        {
            _matchStateMachine.Update();
        }

        public override void FixedUpdate()
        {
            _matchStateMachine.FixedUpdate();
        }
    }
}