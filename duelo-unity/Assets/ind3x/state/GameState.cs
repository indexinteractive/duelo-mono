namespace Ind3x.State
{
    using System;
    using UnityEngine;

    #region StateExitValue Class
    /// <summary>
    /// Optional return value for states that are popped off the stack
    /// i.e. Yes/No for a dialog, etc.
    /// </summary>
    public class StateExitValue
    {
        public Type sourceState;
        public object data;

        public StateExitValue(Type sourceState, object data = null)
        {
            this.data = data;
            this.sourceState = sourceState;
        }
    }
    #endregion

    public class GameState
    {
        #region Public / Inherited Properties
        public StateMachine StateMachine;
        #endregion

        #region State Lifecycle
        public virtual void OnEnter() { }

        /// <summary>
        /// Called just before the state is removed from <see cref="StateMachine._states"/> stack.
        /// </summary>
        public virtual StateExitValue OnExit()
        {
            return null;
        }

        public virtual void Update() { }

        /// <summary>
        /// Note that FixedUpdate will not be called for UI states
        /// </summary>
        public virtual void FixedUpdate() { }

        public virtual void Suspend() { }
        public virtual void Resume(StateExitValue results) { }
        #endregion
    }
}