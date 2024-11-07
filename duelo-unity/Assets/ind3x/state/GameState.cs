namespace Ind3x.State
{
    using System;
    using Ind3x.Ui;
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
        #region Constants
        const float UI_Z_DEPTH = 6.0f;
        #endregion

        #region Public / Inherited Properties
        public StateMachine StateMachine;
        protected GameObject _gui;
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

        #region UI
        /// <summary>
        /// Called once per frame
        /// </summary>
        public virtual void OnGUI() { }
        public virtual void HandleUIEvent(GameObject source, object eventData) { }

        protected void ShowUI()
        {
            if (_gui != null)
            {
                _gui.SetActive(true);
            }
        }

        protected void HideUI()
        {
            if (_gui != null)
            {
                _gui.SetActive(false);
            }
            UiButton.allActiveButtons.Clear();
        }

        protected void DestroyUI()
        {
            if (_gui != null)
            {
                GameObject.Destroy(_gui);
                _gui = null;
            }
        }
        #endregion

        protected virtual void FitToScreen(Camera camera, GameObject gui)
        {
            RectTransform rt = gui.GetComponent<RectTransform>();
            Vector2 lowerLeft = camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMin + rt.offsetMin));
            Vector2 upperRight = camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMax + rt.offsetMax));

            float totalWidth = Mathf.Abs(upperRight.x - lowerLeft.x);
            float totalHeight = Mathf.Abs(upperRight.y - lowerLeft.y);
            float guiScale = 1.0f;
            if (totalWidth > Screen.width)
            {
                guiScale = Screen.width / totalWidth;
            }
            if (totalHeight > Screen.height)
            {
                guiScale = Math.Min(Screen.height / totalHeight, guiScale);
            }

            gui.transform.localScale *= guiScale;
        }
    }
}