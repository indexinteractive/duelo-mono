namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Model;
    using UnityEngine.Assertions;

    /// <summary>
    /// Should be attached to any entity that can be damaged by another entity through a collision
    /// with a <see cref="HurtboxComponent"/>.
    /// </summary>
    public class HitboxComponent : MonoBehaviour
    {
        #region Components
        public HealthComponent Health;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            Assert.IsNotNull(Health, "[HitboxComponent] Health component was not found");
        }
        #endregion

        #region Public Methods
        public void Damage(AttackData attack)
        {
            Health?.Damage(attack);
        }
        #endregion
    }
}
