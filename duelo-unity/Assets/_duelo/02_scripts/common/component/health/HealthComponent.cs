namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Player;
    using Duelo.Common.Model;
    using Duelo.Common.Match;
    using Duelo.Common.Core;
    using Cysharp.Threading.Tasks;
    using R3;

    [RequireComponent(typeof(PlayerTraits))]
    public class HealthComponent : MonoBehaviour
    {
        #region Components
        private MatchPlayer _player;

        private ObservableMatch _match;
        private readonly CompositeDisposable _dataSubs = new();
        #endregion

        #region Public Properties
        public float Health { get; private set; }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _player = GetComponent<MatchPlayer>();
            _match = _player.Match;

            // Initialize health based on player traits
            Health = _player.Traits.BaseHealth;
            Debug.Log($"[HealthComponent] {name} initialized with {Health} health");

            _match.CurrentRound
                .WhereNotNull()
                .Subscribe(OnRoundsUpdated)
                .AddTo(_dataSubs);
        }

        private void OnDestroy()
        {
            _dataSubs?.Dispose();
        }
        #endregion

        #region Public Methods
        public void Damage(AttackData attack)
        {
            Health -= attack.AttackDamage;
            Debug.Log($"[HealthComponent] {name} took {attack.AttackDamage} damage and is now at {Health} health");

            _player.UpdateRoundHealth(Health);

            if (Health <= 0)
            {
                // TODO: Broadcast event to notify other components of death
                // Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Data Updates
        private void OnRoundsUpdated(MatchRound round)
        {
            Health = round.PlayerState[_player.Role].Health.CurrentValue;
            Debug.Log($"[HealthComponent] Updated health to {Health}");
        }
        #endregion
    }
}
