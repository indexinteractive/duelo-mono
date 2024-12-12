namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Server.Match;
    using Duelo.Common.Player;
    using System;
    using Duelo.Common.Model;

    /// <summary>
    /// Should be attached to any entity that can damage another entity through a collision
    /// with a <see cref="HitboxComponent"/>.
    /// </summary>
    public class HurtboxComponent : MonoBehaviour
    {
        #region Private Fields
        private int _attackDamage;
        #endregion

        #region Components
        [Header("Player Components")]
        [Tooltip("PlayerTraits component of the player this hurtbox belongs to")]
        public PlayerTraits Traits;
        #endregion

        #region Public Properties
        [Header("Hurtbox Properties")]
        [Tooltip("Additional strength on top of Player's base strength if assigned")]
        public int AdditionalStrength = 0;

        public Action OnHit;
        #endregion

        #region Initialization
        void Start()
        {
            _attackDamage = Traits.BaseStrength + AdditionalStrength;
        }
        #endregion

        #region Events
        private void OnTriggerEnter(Collider other)
        {
            HitboxComponent hitBox = other.GetComponent<HitboxComponent>();
            if (hitBox != null && hitBox.gameObject != gameObject)
            {
                AttackData attack = new AttackData(_attackDamage);
                hitBox.Damage(attack);

                OnHit?.Invoke();
            }
        }
        #endregion
    }
}