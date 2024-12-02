namespace Duelo.Common.Component
{
    using UnityEngine;

    public abstract class ActionComponent : MonoBehaviour
    {
        public abstract bool IsFinished { get; }
        public abstract void OnActionRemoved();
        public abstract void Initialize(params object[] args);
    }
}