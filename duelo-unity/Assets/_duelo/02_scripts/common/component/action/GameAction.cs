namespace Duelo.Common.Component
{
    using UnityEngine;

    /// <summary>
    /// Base class for all actions that can be added to the <see cref="ActionQueueComponent"/>.
    /// Inherits from <see cref="MonoBehaviour"/>, so it can be attached to a <see cref="GameObject"/>.
    /// </summary>
    public abstract class GameAction : MonoBehaviour
    {
        public abstract bool IsFinished { get; }

        /// <summary>
        /// Allows the action to be initialized before mounting as current action
        /// </summary>
        public virtual void Initialize(params object[] args) { }

        /// <summary>
        /// Called when this action transitions into an active state by <see cref="ActionQueueComponent.Update"/>
        /// </summary>
        public abstract void OnActionMounted();

        /// <summary>
        /// Called once this action sets <see cref="IsFinished"/> to true, before it is removed by <see cref="ActionQueueComponent.Update"/>
        /// </summary>
        public abstract void OnActionRemoved();
    }
}