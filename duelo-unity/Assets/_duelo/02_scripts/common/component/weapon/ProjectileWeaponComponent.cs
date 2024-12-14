namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Model;
    using UnityEngine.Assertions;
    using Duelo.Common.Player;

    public class ProjectileWeaponComponent : MonoBehaviour
    {
        #region Private Fields
        private PlayerTraits _traits;
        #endregion

        #region Public Properties
        [Header("Projectile Settings")]
        public GameObject ProjectilePrefab;
        public Transform FirePoint;
        public float ProjectileSpeed = 10f;

        [Tooltip("Power of the attack. This value will be added to the base strength of the player.")]
        public float Power = 1f;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _traits = GetComponentInParent<PlayerTraits>();
            Assert.IsNotNull(ProjectilePrefab, "[ProjectileWeaponComponent] ProjectilePrefab is not assigned.");
            Assert.IsNotNull(FirePoint, "[ProjectileWeaponComponent] FirePoint is not assigned.");
        }
        #endregion

        #region Projectile Implementation
        public void Fire()
        {
            Debug.Log("[ProjectileWeaponComponent] Fire");

            if (ProjectilePrefab == null || FirePoint == null)
            {
                Debug.LogError("[ProjectileComponent] ProjectilePrefab or FirePoint is not assigned.");
                return;
            }

            GameObject projectileInstance = Instantiate(ProjectilePrefab, FirePoint.position, FirePoint.rotation);
            Projectile projectile = projectileInstance.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.Initialize(FirePoint, ProjectileSpeed, new AttackData(_traits.BaseStrength + Power));
            }
        }
        #endregion
    }
}
