namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;

    public class DefenseRingDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override int ActionId => DefenseActionId.DefenseRing;
        public override Type BehaviorType => typeof(DefenseRingAction);

        public override object[] InitializationParams()
        {
            return new object[] { };
        }
        #endregion
    }
}