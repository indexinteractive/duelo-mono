namespace Duelo.Util
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class UiTimer : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// Current time on the clock
        /// </summary>
        private float _runningTime;

        /// <summary>
        /// Controls the internal timer operation
        /// </summary>
        private bool _run = false;

        /// <summary>
        /// Reference to the UI Text element
        /// </summary>
        [SerializeField]
        private Text _uiText;
        #endregion

        #region Public Properties
        public Action TimerElapsed;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (_run)
            {
                if (_runningTime > 0)
                {
                    _runningTime = Mathf.Max(_runningTime - Time.deltaTime, 0);
                    UpdateUIText(_runningTime);
                }
                else
                {
                    StopTimer();
                    TimerElapsed?.Invoke();
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Starts the countdown timer.
        /// </summary>
        /// <param name="timeMs">Initial time in milliseconds</param>
        public void StartTimer(uint timeMs)
        {
            _runningTime = (float)timeMs / 1000;
            _run = true;
        }

        /// <summary>
        /// Stops the countdown timer.
        /// </summary>
        public void StopTimer()
        {
            _runningTime = 0;
            _run = false;
            UpdateUIText(_runningTime);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Updates the UI Text to display the current countdown time in "MM:SS" format
        /// </summary>
        /// <param name="time">Time in seconds</param>
        private void UpdateUIText(float time)
        {
            if (_uiText != null)
            {
                int seconds = Mathf.FloorToInt(time);
                int milliseconds = Mathf.FloorToInt((time - seconds) * 100);
                _uiText.text = $"{seconds:00}:{milliseconds:00}";
            }
        }
        #endregion
    }
}