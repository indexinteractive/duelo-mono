namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;
    using UnityEngine;

    public class WalkDescriptor : ActionDescriptor
    {
        #region Initialization
        public WalkDescriptor() : base() { }
        public WalkDescriptor(Vector3 destination) : base()
        {
            Destination = destination;
        }
        #endregion

        #region WalkDescriptor Properties
        public readonly Vector3 Destination;
        #endregion

        #region ActionDescriptor Implementation
        public override int ActionId => MovementActionId.Walk;
        public override Type BehaviorType => typeof(WalkGameAction);

        public override object[] InitializationParams()
        {
            return new object[] { Destination };
        }
        #endregion
    }
}
