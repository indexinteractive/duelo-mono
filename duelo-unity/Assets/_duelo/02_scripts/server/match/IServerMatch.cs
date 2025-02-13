namespace Duelo.Server.Match
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;

    public interface IServerMatch : IDisposable
    {
        #region Properties
        public string MatchId { get; }
        public string MapId { get; }
        public Dictionary<PlayerRole, MatchPlayer> Players { get; }
        public MatchRound CurrentRound { get; }

        /// <summary>
        /// Event that is triggered when the connection status of a player changes.
        /// Includes the status of both players.
        /// </summary>
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Responsible for loading and spawning character assets before the match starts.
        /// </summary>
        public void LoadAssets();

        /// <summary>
        /// Creates a new round
        /// </summary>
        public UniTask<MatchRound> NewRound();

        /// <summary>
        /// Publishes the current match state to the server so clients can respond with their state.
        /// </summary>
        public UniTask PublishSyncState(MatchState state);

        /// <summary>
        /// Publishes the current match state and waits for clients to respond with their state.
        /// </summary>
        public UniTask WaitForSyncState(MatchState newState);
        #endregion
    }
}