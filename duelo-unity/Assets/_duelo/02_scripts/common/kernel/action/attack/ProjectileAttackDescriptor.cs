namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;

    public class ProjectileAttackDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override int ActionId => AttackActionId.CannonFire;
        public override Type BehaviorType => typeof(ProjectileAttackAction);

        public override object[] InitializationParams()
        {
            return new object[] { };
        }
        #endregion
    }
}