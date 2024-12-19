namespace Duelo.Client.UI
{
    using System.Collections.Generic;
    using Duelo.Client.Screen;
    using Duelo.Common.Core;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        #region Static Fields
        /// <summary>
        /// List of gameobjects in hover state
        /// </summary>
        public static List<GameObject> allActiveButtons = new();
        #endregion

        #region Public Properties
        public bool hover = false;
        public bool press = false;
        #endregion

        #region Private Fields
        private float hoverStartTime;
        // How much the scale oscilates in either direction while the button hovers.
        const float ButtonScaleRange = 0.15f;
        // The frequency of the oscilations, in oscilations-per-2Pi seconds.
        const float ButtonScaleFrequency = 6.0f;
        // How the scale increase when the button is being pressed.
        const float ButtonScalePressed = 0.5f;
        // How fast the scale transitions when changing states, in %-per-frame.
        const float transitionSpeed = 0.09f;

        private float currentScale = 1.0f;
        private Vector3 startingScale;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            startingScale = transform.localScale;
        }

        private void Update()
        {
            float targetScale = 1.0f;
            if (press)
            {
                targetScale = 1.0f + ButtonScalePressed;
            }
            else if (hover)
            {
                targetScale = 1.0f + ButtonScaleRange + Mathf.Cos((hoverStartTime - Time.realtimeSinceStartup) * ButtonScaleFrequency) * ButtonScaleRange;
            }
            currentScale = currentScale * (1.0f - transitionSpeed) + targetScale * transitionSpeed;
            transform.localScale = startingScale * currentScale;
        }

        private void OnDestroy()
        {
            allActiveButtons.Remove(gameObject);
        }
        #endregion

        #region Pointer Implementations
        public void OnPointerClick(PointerEventData eventData)
        {
            if (ClientData.StateMachine.CurrentState is GameScreen state)
            {
                state.HandleUIEvent(gameObject, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            press = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverStartTime = Time.realtimeSinceStartup;
            hover = true;
            allActiveButtons.Add(gameObject);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hover = false;
            allActiveButtons.Remove(gameObject);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            hover = false;
            press = false;
        }
        #endregion
    }
}