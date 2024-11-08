namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Service;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Base class for all states in the server match flow:
    ///
    /// → Startup - <see cref="StateMatchStartup"/>
    /// → Lobby - <see cref="StateMatchLobby"/>
    /// → Initialize Game - <see cref="StateInitializeGame"/>
    /// → Begin Round - <see cref="StateBeginRound"/>
    /// → Choose Movement - <see cref="StateChooseMovement"/>
    /// → Choose Action - <see cref="StateChooseAction"/>
    /// → Late Actions - <see cref="StateLateActions"/>
    /// → Execute Actions - <see cref="StateExecuteRound"/>
    /// → End Round - <see cref="StateEndRound"/>
    ///
    /// </summary>
    public class ServerMatchState : GameState
    {
        public override void OnEnter()
        {
            Debug.Log(GetType().Name);
            UpdateDbState();
        }

        protected virtual UniTask<bool> UpdateDbState()
        {
            string matchId = ServerData.MatchDto.MatchId;
            return MatchService.Instance.UpdateMatchState(matchId, this);
        }
    }
}