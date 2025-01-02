namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Player;
    using Duelo.Common.Model;

    [RequireComponent(typeof(PlayerTraits))]
    public class HealthComponent : MonoBehaviour
    {
        #region Components
        private PlayerTraits _traits;
        #endregion

        #region Public Properties
        public float Health { get; private set; }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _traits = GetComponent<PlayerTraits>();

            // Initialize health based on player traits
            Health = _traits.BaseHealth;
        }
        #endregion

        #region Public Methods
        public void Damage(AttackData attack)
        {
            Health -= attack.AttackDamage;
            Debug.Log($"[HealthComponent] {name} took {attack.AttackDamage} damage and is now at {Health} health");

            // TODO: Broadcast event to notify other components of health change
            // if (EventManager.Instance != null)
            // {
            //     EventManager.Instance.Broadcast(new PlayerHealthChangedEvent(Player.PlayerId, Health));
            // }

            // Check if health drops below or equal to zero
            if (Health <= 0)
            {
                // TODO: Broadcast event to notify other components of death
                Destroy(gameObject); // Destroys the GameObject to which this component is attached
            }
        }
        #endregion
    }
}
