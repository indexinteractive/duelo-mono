namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Component;
    using Duelo.Common.Match;
    using Duelo.Common.Player;
    using UnityEngine;

    /// <summary>
    /// Base class for actions that are instantiated by a <see cref="Model.ActionId"/>. Any
    /// class that inherits <see cref="ActionDescriptor"/> returns a <see cref="BehaviorType"/> that
    /// is used to instantiate a <see cref="Component.ActionComponent"/> that will run directly on an entity.
    /// </summary>
    public abstract class ActionDescriptor : ScriptableObject
    {
        #region ScriptableObject Properties
        [Header("Ui Properties")]
        public string Label;
        public Sprite Icon;

        [Tooltip("The action id that this item represents. Will be used to later instantiate the corresponding ActionDescriptor.")]
        [Common.Model.ActionId]
        public int ActionId;
        #endregion

        #region Action Properties
        public abstract Type BehaviorType { get; }

        /// <summary>
        /// Returns the parameters that will be passed to the <see cref="Duelo.Common.Component.GameAction"/> constructor.
        /// </summary>
        public object[] InitializationParams { get; set; } = new object[] { };

        /// <summary>
        /// Returns all tiles that can be chosen during the movement phase.
        /// </summary>
        public virtual IEnumerable<Vector3> GetMovablePositions(PlayerTraits traits, Vector3 origin) => new List<Vector3>();

        /// <summary>
        /// Returns all tiles that will be within range of the attack.
        /// </summary>
        public virtual IEnumerable<Vector3> GetAttackRangeTiles() => new List<Vector3>();
        #endregion

        #region Initialization
        public ActionDescriptor() { }
        public virtual GameAction AddComponentTo(MatchPlayer target)
        {
            var action = target.gameObject.AddComponent(BehaviorType) as GameAction;
            action.Initialize(InitializationParams);
            return action;
        }
        #endregion
    }
}