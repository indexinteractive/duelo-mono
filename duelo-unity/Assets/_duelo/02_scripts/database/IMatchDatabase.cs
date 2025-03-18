namespace Duelo.Database
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;

    public interface IMatchDatabase
    {
        public UniTask PublishMatch(MatchDto dto);

        public UniTask PublishServerSyncState(SyncStateDto dto);
        public UniTask UpdateClientState(PlayerRole role, MatchState state);

        public UniTask BeginMovementPhase(int round, MovementPhaseDto dto);
        public UniTask BeginActionPhase(int round, ActionPhaseDto dto);

        public UniTask PublishRound(int round, MatchRoundDto dto);
        public UniTask UpdateRound(int round, Dictionary<string, object> updates);

        public UniTask UpdateConnectionStatus(PlayerRole role, string status);

        public UniTask DispatchMovement(int round, PlayerRole role, PlayerRoundMovementDto dto);
        public UniTask DispatchAction(int round, PlayerRole role, PlayerRoundActionDto dto);

        /// <summary>
        /// Subscribe to a path in the database. Resulting action should be a
        /// callback that will be used to unsubscribe from the path.
        /// </summary>
        /// <param name="path">Path in firebase relative to the match</param>
        public Action Subscribe(string path, Action<string> callback);

        public void Dispose();
    }
}