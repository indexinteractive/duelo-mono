namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;
    using UnityEngine;

    public partial class HoverDescriptor : MovementDescriptor
    {
        #region Initialization
        public HoverDescriptor() : base(Vector3.zero) { }
        public HoverDescriptor(Vector3 destination) : base(destination) { }
        #endregion

        #region Descriptor Implementation
        public override int ActionId => MovementActionId.Hover;

        public override Type BehaviorType => typeof(HoverComponent);

        public override object[] InitializationParams()
        {
            return new object[] { Destination };
        }
        #endregion
    }
}
