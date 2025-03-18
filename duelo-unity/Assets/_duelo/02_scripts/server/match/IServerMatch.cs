namespace Duelo.Server.Match
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;

    public interface IServerMatch : IDisposable
    {
        #region Methods
        /// <summary>
        /// Responsible for loading and spawning character assets before the match starts.
        /// </summary>
        public void LoadAssets();

        /// <summary>
        /// Creates a new round
        /// </summary>
        public UniTask<MatchRound> NewRound();

        public UniTask KickoffMovementPhase();
        public void EndMovementPhase();

        public UniTask KickoffActionsPhase();
        public void EndActionsPhase();

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