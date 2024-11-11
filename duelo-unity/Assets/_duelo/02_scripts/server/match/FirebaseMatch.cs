using System;
using Duelo.Common.Model;
using Firebase.Database;
using UnityEngine;

namespace Duelo.Server.Match
{
    public class ConnectionChangedEventArgs
    {
        public ConnectionStatus ChallengerStatus { get; set; }
        public ConnectionStatus DefenderStatus { get; set; }
    }

    public class FirebaseMatch
    {
        #region Constants
        public const string COLLECTION_MATCH = "match";
        public const string MATCH_FIELD_PLAYERS = "players";
        #endregion

        #region Match Properties
        public string MatchId { get; private set; }

        public DatabaseReference PlayersRef => FirebaseDatabase.DefaultInstance.GetReference(COLLECTION_MATCH).Child(MatchId).Child(MATCH_FIELD_PLAYERS);
        #endregion

        #region Player Properties
        private ConnectionStatus _challengerStatus;
        private ConnectionStatus _defenderStatus;

        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged;
        #endregion

        #region Initialization
        public FirebaseMatch(string matchId)
        {
            MatchId = matchId;

            GetConnectionRef(PlayerRole.Challenger).ValueChanged += OnChallengerConnectionChanged;
            GetConnectionRef(PlayerRole.Defender).ValueChanged += OnDefenderConnectionChanged;
        }
        #endregion

        #region Player Connections
        private void OnChallengerConnectionChanged(object sender, ValueChangedEventArgs args)
        {
            Enum.TryParse(args.Snapshot.Value?.ToString(), ignoreCase: true, out _challengerStatus);
            UpdatePlayersConnection(sender, args);
        }

        private void OnDefenderConnectionChanged(object sender, ValueChangedEventArgs args)
        {
            Enum.TryParse(args.Snapshot.Value?.ToString(), ignoreCase: true, out _defenderStatus);
            UpdatePlayersConnection(sender, args);
        }

        private void UpdatePlayersConnection(object sender, ValueChangedEventArgs args)
        {
            try
            {
                Debug.Log($"[FirebaseMatch] Player status changed: challenger={_challengerStatus}, defender={_defenderStatus}");

                var eventArgs = new ConnectionChangedEventArgs
                {
                    ChallengerStatus = _challengerStatus,
                    DefenderStatus = _defenderStatus
                };

                OnPlayersConnectionChanged?.Invoke(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseMatch] Error checking player online status: {ex.Message}");
            }
        }
        #endregion

        #region Ref Methods
        public DatabaseReference GetConnectionRef(PlayerRole role)
        {
            return PlayersRef.Child(role == PlayerRole.Challenger ? "challenger" : "defender").Child("connection");
        }
        #endregion
    }


}