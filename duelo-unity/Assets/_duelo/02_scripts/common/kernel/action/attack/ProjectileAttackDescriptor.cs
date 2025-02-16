namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using UnityEngine;

    [CreateAssetMenu(fileName = "ProjectileAttack", menuName = "Duelo/Actions/Attack/ProjectileAttack")]
    public class ProjectileAttackDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(ProjectileAttackAction);
        #endregion
    }
}