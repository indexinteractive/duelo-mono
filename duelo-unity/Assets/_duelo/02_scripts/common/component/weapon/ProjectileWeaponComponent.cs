namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Model;
    using UnityEngine.Assertions;

    public class ProjectileWeaponComponent : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public GameObject ProjectilePrefab;
        public Transform FirePoint;
        public float ProjectileSpeed = 10f;
        public float AttackDamage = 10f;

        #region Unity Lifecycle
        private void Awake()
        {
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
                projectile.Initialize(FirePoint, ProjectileSpeed, new AttackData(AttackDamage));
            }
        }
        #endregion
    }
}
