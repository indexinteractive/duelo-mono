namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConnectionChangedEventArgs
    {
        public ConnectionStatus ChallengerStatus { get; set; }
        public ConnectionStatus DefenderStatus { get; set; }
    }

    /// <summary>
    /// Object that represents a match in the server. Communicates directly
    /// with firebase to perform updates on the match state.
    /// </summary>
    public class ServerMatch
    {
        #region Constants
        // TODO: Move to service probably
        public const string COLLECTION_MATCH = "match";
        public const string MATCH_FIELD_PLAYERS = "players";
        #endregion

        #region Match Properties
        public string MatchId { get; private set; }

        public MatchState State { get; private set; }
        public MatchClock Clock { get; private set; }

        public readonly MatchPlayer Defender;
        public readonly MatchPlayer Challenger;

        public readonly Dictionary<int, RoundDto> Rounds;

        public MatchPlayer[] Players => new[] { Challenger, Defender };
        #endregion

        #region Db References
        public DatabaseReference PlayersRef => FirebaseDatabase.DefaultInstance.GetReference(COLLECTION_MATCH).Child(MatchId).Child(MATCH_FIELD_PLAYERS);
        #endregion

        #region Player Properties
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged;
        #endregion

        #region Initialization
        public ServerMatch(MatchDto dbData)
        {
            MatchId = dbData.MatchId;
            State = MatchState.Startup;
            Clock = new MatchClock(dbData.ClockConfig);

            Defender = new MatchPlayer(MatchId, PlayerRole.Defender, dbData.Players.Defender);
            Challenger = new MatchPlayer(MatchId, PlayerRole.Challenger, dbData.Players.Challenger);

            Rounds = new Dictionary<int, RoundDto>();

            Defender.OnStatusChanged += UpdatePlayersConnection;
            Challenger.OnStatusChanged += UpdatePlayersConnection;
        }
        #endregion

        #region Player Connections
        private void UpdatePlayersConnection(PlayerStatusChangedEvent e)
        {
            try
            {
                Debug.Log($"[FirebaseMatch] Player status changed: challenger={Challenger.Status}, defender={Defender.Status}");

                var eventArgs = new ConnectionChangedEventArgs
                {
                    ChallengerStatus = Challenger.Status,
                    DefenderStatus = Defender.Status
                };

                OnPlayersConnectionChanged?.Invoke(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseMatch] Error checking player online status: {ex.Message}");
            }
        }
        #endregion

        #region Data Updates
        public UniTask<bool> UpdateState(MatchState state, bool updateDb = true)
        {
            State = state;

            if (updateDb)
            {
                return MatchService.Instance.UpdateMatchState(MatchId, state.ToString());
            }

            return UniTask.FromResult(true);
        }

        public UniTask<bool> PartialUpdate(Dictionary<string, object> update)
        {
            return MatchService.Instance.PartialUpdate(MatchId, update);
        }

        public UniTask<bool> NewRound()
        {
            Clock.NewRound();
            Rounds[Clock.CurrentRound] = new RoundDto(Clock.CurrentRound, Clock.CurrentTimeAllowedMs);
            return MatchService.Instance.PushRound(MatchId, Rounds[Clock.CurrentRound]);
        }
        #endregion
    }


}