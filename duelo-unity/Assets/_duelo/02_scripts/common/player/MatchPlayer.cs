namespace Duelo.Common.Match
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Component;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Player;
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
        private MatchPlayerDto _dto;
        private GameObject _ghostInstance;
        #endregion

        #region Public Properties
        public string UnityPlayerId => _dto.UnityPlayerId;
        public bool IsDevicePlayer => _dto.UnityPlayerId == GlobalState.PlayerData.UnityPlayerId;
        public PlayerProfileDto ProfileDto => _dto.Profile;
        public PlayerRole Role { get; private set; }

        public ConnectionStatus Status;

        public Vector3 Position => transform.position;
        #endregion

        #region Components
        public ActionQueueComponent ActionQueue { get; private set; }
        public PlayerTraits Traits { get; private set; }
        public HealthComponent HealthComponent { get; private set; }
        #endregion

        #region Db Refs
        private DatabaseReference _connectionRef;
        #endregion

        #region Events
        public event Action<PlayerStatusChangedEvent> OnStatusChanged;
        #endregion

        #region Initialization
        public void Initialize(DatabaseReference playerRef, PlayerRole role, MatchPlayerDto dto)
        {
            Role = role;
            _dto = dto;

            if (playerRef != null)
            {
                _connectionRef = playerRef.Child("connection");
                _connectionRef.ValueChanged += OnConnectionChanged;
            }
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
            if (_connectionRef != null)
            {
                _connectionRef.ValueChanged -= OnConnectionChanged;
            }
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
        /// Called by <see cref="Client.Match.ClientMatchFirebase.OnMatchUpdate"/> when the MatchDto changes
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

        #region Firebase
        private void OnConnectionChanged(object sender, ValueChangedEventArgs args)
        {
            Enum.TryParse(args.Snapshot.Value?.ToString(), ignoreCase: true, out Status);
            OnStatusChanged?.Invoke(new PlayerStatusChangedEvent(UnityPlayerId, Status));
        }

        /// <summary>
        /// Called by <see cref="HealthComponent.Damage"/>
        /// </summary>
        public async UniTask PublishHealth(float health)
        {
            // Only the server should publish information to the database schema.
            if (GlobalState.ServerMatch != null)
            {
                var stateRef = GlobalState.ServerMatch.CurrentRound.PlayerStateRef;
                if (stateRef != null)
                {
                    await stateRef.Child(Role.ToString().ToLower()).Child("health").SetValueAsync(health);
                }
            }
        }
        #endregion
    }
}