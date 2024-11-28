namespace Duelo.Server.State
{
    using Duelo.Common.Core;
    using Duelo.Gameboard;
    using Duelo.Server.Match;
    using Ind3x.State;

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
        public ServerMatch Match => ServerData.Match;
        public DueloMap Map => ServerData.Map;
    }
}