namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;
    using UnityEngine;

    public partial class WalkingMovement : MovementDescriptor
    {
        #region Initialization
        public WalkingMovement() : base(Vector3.zero) { }
        public WalkingMovement(Vector3 destination) : base(destination) { }
        #endregion

        #region Descriptor Implementation
        public override int ActionId => MovementActionId.Walk;

        public override Type BehaviorType => typeof(WalkComponent);

        public override object[] InitializationParams()
        {
            return new object[] { Destination };
        }
        #endregion
    }
}
