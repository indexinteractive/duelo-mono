using System;
using Duelo.Common.Component;
using Duelo.Common.Model;

namespace Duelo.Common.Kernel
{
    public class CloseRangeAttackDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override int ActionId => AttackActionId.CloseRange;
        public override Type BehaviorType => typeof(CloseRangeAttackAction);

        public override object[] InitializationParams()
        {
            return new object[] { };
        }
        #endregion
    }
}