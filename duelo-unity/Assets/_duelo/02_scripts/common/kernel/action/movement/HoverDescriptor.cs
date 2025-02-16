namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using UnityEngine;

    [CreateAssetMenu(fileName = "HoverMovement", menuName = "Duelo/Actions/Movement/HoverMovement")]
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
        public override Type BehaviorType => typeof(HoverGameAction);
        #endregion
    }
}
