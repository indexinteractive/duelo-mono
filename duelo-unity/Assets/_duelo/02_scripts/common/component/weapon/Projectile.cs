namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Model;

    public class Projectile : MonoBehaviour
    {
        #region Properties
        public float LifespanSec = 2f;
        #endregion

        #region Projectile Implementation
        private AttackData _attackData;

        public void Initialize(Transform firePoint, float initialSpeed, AttackData attackData)
        {
            _attackData = attackData;

            transform.rotation = firePoint.rotation;

            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = transform.forward * initialSpeed;
                Debug.Log("[Projectile] Velocity set to: " + rb.linearVelocity);

                Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green, 2);

            }

            Debug.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 5, Color.red, 5);


            Destroy(gameObject, LifespanSec);
        }
        #endregion

        #region Collider
        private void OnTriggerEnter(Collider other)
        {
            HitboxComponent hitBox = other.GetComponent<HitboxComponent>();
            if (hitBox != null)
            {
                hitBox.Damage(_attackData);
                Destroy(gameObject); // Destroy the projectile upon collision
            }
        }
        #endregion
    }
}