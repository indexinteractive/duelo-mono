namespace Duelo.Server.State
{
    using Ind3x.State;
    using UnityEngine;
    using Duelo.Common.Core;
    using Duelo.Server.Match;

    /// <summary>
    /// Mounted by <see cref="GameMain"/> in server mode and kicks off a match flow
    /// </summary>
    public class StateRunServerMatch : GameState
    {
        private StateMachine _matchStateMachine;

        public override void OnEnter()
        {
            Debug.Log("StateRunServerMatch");

            ServerData.MatchClock = new MatchClock(ServerData.MatchDto.ClockConfig);
            ServerData.Match = new FirebaseMatch(ServerData.MatchDto.MatchId);

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