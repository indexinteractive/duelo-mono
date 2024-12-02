namespace Duelo.Common.Kernel
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Base class for actions that are instantiated by a <see cref="Model.ActionId"/>. Any
    /// class that inherits <see cref="ActionDescriptor"/> returns a <see cref="BehaviorType"/> that
    /// is used to instantiate a <see cref="Component.ActionComponent"/> that will run directly on an entity.
    /// </summary>
    public abstract class ActionDescriptor
    {
        public abstract int ActionId { get; }
        public abstract Type BehaviorType { get; }
        public abstract object[] InitializationParams();

        public ActionDescriptor() { }
    }
}