namespace Duelo.Common.Match
{
    using Duelo.Common.Component;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Player;
    using R3;
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
        public ObservableMatch Match { get; private set; }
        private MatchPlayerDto _dto;
        private GameObject _ghostInstance;
        #endregion

        #region Public Properties
        public string UnityPlayerId => _dto.UnityPlayerId;
        public bool IsDevicePlayer => _dto.UnityPlayerId == GlobalState.PlayerData.UnityPlayerId;
        public PlayerProfileDto ProfileDto => _dto.Profile;
        public PlayerRole Role { get; private set; }

        public ReactiveProperty<ConnectionStatus> Status = new(ConnectionStatus.Unknown);

        public Vector3 Position => transform.position;
        #endregion

        #region Components
        public ActionQueueComponent ActionQueue { get; private set; }
        public PlayerTraits Traits { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        #endregion

        #region Initialization
        public void Initialize(ObservableMatch match, PlayerRole role, MatchPlayerDto dto)
        {
            Role = role;
            Match = match;
            _dto = dto;
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ActionQueue = gameObject.AddComponent<ActionQueueComponent>();
            Traits = gameObject.GetComponent<PlayerTraits>();
            HealthComponent = GetComponent<HealthComponent>();
        }

        private void OnDestroy()
        {
        }
        #endregion

        #region IExecuteEntity Implementation
        public bool IsRunning => ActionQueue.HasActions;

        public void Begin()
        {
            Debug.Log($"[MatchPlayer] Player {UnityPlayerId} is allowed to begin round");
            ActionQueue.Run = true;
        }

        public void End()
        {
            ActionQueue.Run = false;
        }

        public void Enqueue(ActionDescriptor descriptor)
        {
            var action = descriptor.AddComponentTo(this);
            action.Initialize(descriptor.InitializationParams);

            ActionQueue.QueueAction(action);
        }
        #endregion

        #region Match Events
        /// <summary>
        /// Called by <see cref="Client.Match.ClientMatch.OnMatchUpdate"/> when the MatchDto changes
        /// </summary>
        public void OnMatchStateChanged(MatchState state) { }

        public void SetGhost(Vector3 targetPosition)
        {
            DestroyGhost();
            var direction = targetPosition - transform.position;
            _ghostInstance = GameObject.Instantiate(Traits.GhostPrefab, targetPosition, Quaternion.LookRotation(direction, Vector3.up));
        }

        public void DestroyGhost()
        {
            if (_ghostInstance != null)
            {
                GameObject.Destroy(_ghostInstance);
            }
        }
        #endregion

        #region Data Events
        public void UpdateFromDto(MatchPlayerDto dto)
        {
            if (dto.UnityPlayerId == UnityPlayerId)
            {
                Status.Value = dto.Connection;
            }
        }

        /// <summary>
        /// Called by <see cref="HealthComponent.Damage"/>
        /// </summary>
        public void UpdateRoundHealth(float health)
        {
            Match.CurrentRound.CurrentValue.PlayerState[Role].Health.Value = health;
        }
        #endregion

        #region Dto Methods
        public PlayerRoundStateDto GetRoundStateDto()
        {
            return new PlayerRoundStateDto
            {
                Health = HealthComponent.Health,
                Position = Position
            };
        }
        #endregion
    }
}