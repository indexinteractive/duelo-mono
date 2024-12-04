namespace Duelo.Common.Component
{
    using UnityEngine;

    /// <summary>
    /// Base class for all actions that can be added to the <see cref="ActionQueueComponent"/>
    /// </summary>
    public abstract class GameAction : MonoBehaviour
    {
        public abstract bool IsFinished { get; }
        public abstract void OnActionRemoved();
        public abstract void Initialize(params object[] args);
    }
}