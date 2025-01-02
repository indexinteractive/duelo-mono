namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Player;
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

        /// <summary>
        /// Returns the parameters that will be passed to the <see cref="Duelo.Common.Component.GameAction"/> constructor.
        /// </summary>
        public abstract object[] InitializationParams();

        /// <summary>
        /// Returns all tiles that can be chosen during the movement phase.
        /// </summary>
        public virtual IEnumerable<Vector3> GetMovableTiles(PlayerTraits traits, Vector3 origin) => new List<Vector3>();

        /// <summary>
        /// Returns all tiles that will be within range of the attack.
        /// </summary>
        public virtual IEnumerable<Vector3> GetAttackRangeTiles() => new List<Vector3>();

        public ActionDescriptor() { }
    }
}