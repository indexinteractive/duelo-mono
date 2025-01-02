namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Model;
    using UnityEngine;

    public class HoverDescriptor : ActionDescriptor
    {
        #region Initialization
        public HoverDescriptor() : base() { }
        public HoverDescriptor(Vector3 destination) : base()
        {
            Destination = destination;
        }
        #endregion

        #region HoverDescriptor Properties
        public readonly Vector3 Destination;
        #endregion

        #region Descriptor Implementation
        public override int ActionId => MovementActionId.Hover;
        public override Type BehaviorType => typeof(HoverGameAction);

        public override object[] InitializationParams()
        {
            return new object[] { Destination };
        }
        #endregion
    }
}
