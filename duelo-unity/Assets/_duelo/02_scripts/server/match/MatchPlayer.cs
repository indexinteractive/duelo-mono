namespace Duelo.Server.Match
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using UnityEngine;

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

    public class MatchPlayer : MonoBehaviour, IExecuteEntity
    {
        #region Private Fields
        private string _matchId;
        private MatchPlayerDto _dto;
        #endregion

        #region Public Properties
        public string Id => _dto.PlayerId;
        public string DeviceId => _dto.DeviceId;
        public PlayerProfileDto ProfileDto => _dto.Profile;
        public PlayerRole Role { get; private set; }

        public ConnectionStatus Status;
        #endregion

        #region Components
        public ActionQueueComponent ActionQueue { get; private set; }
        #endregion

        #region Db Refs
        public DatabaseReference DbRef => MatchService.Instance.GetRef(DueloCollection.Match, _matchId, "players", Role.ToString().ToLower());
        #endregion

        #region Events
        public event Action<PlayerStatusChangedEvent> OnStatusChanged;
        #endregion

        #region Initialization
        public void Initialize(string matchId, PlayerRole role, MatchPlayerDto dto)
        {
            _matchId = matchId;
            Role = role;
            _dto = dto;
            DbRef.Child("connection").ValueChanged += OnConnectionChanged;
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ActionQueue = gameObject.AddComponent<ActionQueueComponent>();
        }
        #endregion

        #region IExecuteEntity Implementation
        public bool IsRunning => ActionQueue.HasActions;

        public void Begin()
        {
            Debug.Log($"[MatchPlayer] Player {Id} is allowed to begin round");
            ActionQueue.Run = true;
        }

        public void End()
        {
            ActionQueue.Run = false;
        }

        public void Enqueue(ActionDescriptor descriptor)
        {
            var action = (ActionComponent)gameObject.AddComponent(descriptor.BehaviorType);
            action.Initialize(descriptor.InitializationParams());

            ActionQueue.QueueAction(action);
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