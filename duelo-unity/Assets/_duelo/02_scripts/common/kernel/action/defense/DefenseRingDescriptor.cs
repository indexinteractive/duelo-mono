namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DefenseRing", menuName = "Duelo/Actions/Defense/DefenseRing")]
    public class DefenseRingDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(DefenseRingAction);
        #endregion
    }
}