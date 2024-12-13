namespace Duelo.Common.Model
{
    using UnityEngine;

    /// <summary>
    /// Attack data sent by a <see cref="Common.Component.HurtboxComponent"/> when it collides with a <see cref="Common.Component.HitboxComponent"/>.
    /// </summary>
    public class AttackData
    {
        public float AttackDamage;
        public Vector3 AttackPosition;
        public float StunTime;

        public AttackData(float damage)
        {
            AttackDamage = damage;
        }
    }
}
