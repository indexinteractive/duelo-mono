namespace Duelo.Client.UI
{
    using System.Collections.Generic;
    using Duelo.Client.Screen;
    using Duelo.Common.Core;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UiButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        #region Static Fields
        /// <summary>
        /// List of gameobjects in hover state
        /// </summary>
        public static List<GameObject> AllActiveButtons = new();
        #endregion

        #region Public Properties
        [Tooltip("Indicates whether the mouse pointer is currently hovering over the button")]
        public bool IsHovered = false;
        [Tooltip("Indicates whether the button is currently being pressed")]
        public bool IsPressed = false;
        #endregion

        #region Private Fields
        private float _currentScale = 1.0f;
        private Button _buttonComponent;
        private Vector3 _startingScale;
        private float _hoverStartTime;

        /// <summary>
        /// How much the scale oscilates in either direction while the button hovers.
        /// </summary>
        public float ButtonScaleRange = 0.15f;

        /// <summary>
        /// The frequency of the oscillations, in cycles per second.
        /// </summary>
        public float ButtonScaleFrequency = 6.0f;

        /// <summary>
        /// The scale increase when the button is pressed.
        /// </summary>
        public float ButtonScalePressed = 0.5f;

        /// <summary>
        /// How fast the scale transitions when changing states, in %-per-frame.
        /// </summary>
        public float TransitionSpeed = 0.09f;
        #endregion

        #region Public Properties
        public bool Disabled
        {
            get => !_buttonComponent.interactable;
            set => _buttonComponent.interactable = !value;
        }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _buttonComponent = GetComponent<Button>();
            _startingScale = transform.localScale;
        }

        private void Update()
        {
            float targetScale = 1.0f;
            if (IsPressed)
            {
                targetScale = 1.0f + ButtonScalePressed;
            }
            else if (IsHovered)
            {
                targetScale = 1.0f + ButtonScaleRange + Mathf.Cos((_hoverStartTime - Time.realtimeSinceStartup) * ButtonScaleFrequency) * ButtonScaleRange;
            }
            _currentScale = _currentScale * (1.0f - TransitionSpeed) + targetScale * TransitionSpeed;
            transform.localScale = _startingScale * _currentScale;
        }

        private void OnDestroy()
        {
            AllActiveButtons.Remove(gameObject);
        }
        #endregion

        #region Pointer Implementations
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Disabled && GlobalState.StateMachine.CurrentState is GameScreen state)
            {
                state.HandleUIEvent(gameObject, eventData);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!Disabled)
            {
                IsPressed = true;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Disabled)
            {
                _hoverStartTime = Time.realtimeSinceStartup;
                IsHovered = true;
                AllActiveButtons.Add(gameObject);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Disabled)
            {
                IsHovered = false;
                AllActiveButtons.Remove(gameObject);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!Disabled)
            {
                IsHovered = false;
                IsPressed = false;
            }
        }
        #endregion
    }
}