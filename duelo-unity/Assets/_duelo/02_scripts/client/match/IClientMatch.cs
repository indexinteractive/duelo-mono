namespace Duelo.Client.Match
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using UnityEngine;

    public interface IClientMatch : IDisposable
    {
        #region Properties
        public string MatchId { get; }
        public string MapId { get; }
        public Dictionary<PlayerRole, MatchPlayer> Players { get; }

        public MatchDto CurrentDto { get; }
        public MatchRoundDto CurrentRound { get; }

        public MatchPlayer DevicePlayer { get; }

        /// <summary>
        /// Event that is triggered when the match state changes.
        /// The first parameter is the new state, the second is the previous state.
        /// </summary>
        public Action<MatchDto, MatchDto> OnStateChange { get; set; }
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
        public UniTask PublishSyncState(string state);

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