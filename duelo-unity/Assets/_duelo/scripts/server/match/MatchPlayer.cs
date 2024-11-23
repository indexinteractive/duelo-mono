namespace Duelo.Server.Match
{
    using System;
    using Duelo.Common.Model;
    using Firebase.Database;

    public class PlayerStatusChangedEvent
    {
        public string PlayerId { get; set; }
        public ConnectionStatus Status { get; set; }
        public PlayerStatusChangedEvent(string playerId, ConnectionStatus status)
        {
            PlayerId = playerId;
            Status = status;
        }
    }

    public class MatchPlayer
    {
        public const string COLLECTION_MATCH = "match";
        public const string MATCH_FIELD_PLAYERS = "players";

        #region Private Fields
        private readonly string _matchId;
        private readonly MatchPlayerDto _dto;
        #endregion

        #region Public Properties
        public readonly string Id;
        public readonly string DeviceId;
        public readonly PlayerRole Role;
        public readonly PlayerProfileDto ProfileDto;

        public ConnectionStatus Status;
        #endregion

        #region Db Refs
        public DatabaseReference DbRef => FirebaseDatabase.DefaultInstance.GetReference($"{COLLECTION_MATCH}/{_matchId}/{MATCH_FIELD_PLAYERS}/{Role.ToString().ToLower()}");
        #endregion

        #region Events
        public event Action<PlayerStatusChangedEvent> OnStatusChanged;
        #endregion

        #region Initializatin
        public MatchPlayer(string matchId, PlayerRole role, MatchPlayerDto dto)
        {
            _matchId = matchId;
            _dto = dto;

            Id = dto.PlayerId;
            DeviceId = dto.DeviceId;
            Role = role;

            Status = ConnectionStatus.Offline;

            DbRef.Child("connection").ValueChanged += OnConnectionChanged;
        }
        #endregion

        #region Database Events
        private void OnConnectionChanged(object sender, ValueChangedEventArgs args)
        {
            Enum.TryParse(args.Snapshot.Value?.ToString(), ignoreCase: true, out Status);
            OnStatusChanged?.Invoke(new PlayerStatusChangedEvent(Id, Status));
        }

        public MatchPlayerDto ToDto()
        {
            return new MatchPlayerDto
            {
                PlayerId = Id,
                DeviceId = DeviceId,
                Connection = Status,
                Profile = _dto.Profile
            };
        }
        #endregion
    }
}