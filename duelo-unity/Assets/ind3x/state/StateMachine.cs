namespace Ind3x.State
{
    using System.Collections.Generic;

    public class StateMachine
    {
        #region Private Fields
        private Stack<GameState> _states;
        #endregion

        #region Public Properties
        public GameState CurrentState { get => _states.Peek(); }
        #endregion

        #region Initialization
        public StateMachine()
        {
            _states = new Stack<GameState>();
            _states.Push(new GameState());
        }
        #endregion

        #region Unity Lifecycle
        public void Update()
        {
            CurrentState.Update();
        }

        public void FixedUpdate()
        {
            CurrentState.FixedUpdate();
        }
        #endregion

        #region State Management
        /// <summary>
        /// Suspends the current state and pushes a new state onto the stack
        /// </summary>
        public void PushState(GameState newState)
        {
            newState.StateMachine = this;
            CurrentState.Suspend();
            _states.Push(newState);
            newState.OnEnter();
        }

        public void PopState()
        {
            StateExitValue result = CurrentState.OnExit();
            _states.Pop();
            CurrentState.Resume(result);
        }

        /// <summary>
        /// Clears existing states and pushes <paramref name="newState"/> onto the stack
        /// </summary>
        public void ClearStack(GameState newState)
        {
            while (_states.Count > 1)
            {
                PopState();
            }
            SwapState(newState);
        }

        /// <summary>
        /// Switches the current state for <paramref name="newState"/>.
        /// Unlike a combination of <see cref="PopState"/> and <see cref="PushState"/>,
        /// states below the current state are not resumed or suspended
        /// </summary>
        public void SwapState(GameState newState)
        {
            newState.StateMachine = this;
            CurrentState.OnExit();
            _states.Pop();
            _states.Push(newState);
            CurrentState.OnEnter();
        }
        #endregion
    }

}
