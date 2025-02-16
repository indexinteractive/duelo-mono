namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CloseRangeAttack", menuName = "Duelo/Actions/Attack/CloseRangeAttack")]
    public class CloseRangeAttackDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(CloseRangeAttackAction);
        #endregion
    }
}