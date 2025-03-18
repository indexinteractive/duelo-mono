namespace Duelo.Client.Match
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using UnityEngine;

    public interface IClientMatch : IDisposable
    {
        #region Properties
        public MatchPlayer DevicePlayer { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Responsible for loading and spawning character assets before the match starts.
        /// </summary>
        public void LoadAssets();

        /// <summary>
        /// Informs the server that this client is joining the match.
        /// </summary>
        public UniTask JoinMatch();

        /// <summary>
        /// Responds to server state changes by publishing the state that
        /// the client has just received.
        /// </summary>
        public UniTask PublishSyncState(MatchState state);

        /// <summary>
        /// Publishes client movement choices to the server.
        /// </summary>
        public void DispatchMovement(int actionId, Vector3 position);

        /// <summary>
        /// Publishes client attack choices to the server.
        /// </summary>
        public void DispatchAttack(int actionId);
        #endregion
    }
}