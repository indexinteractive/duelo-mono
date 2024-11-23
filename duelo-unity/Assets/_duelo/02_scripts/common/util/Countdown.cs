namespace Duelo.Util
{
    using System;
    using UnityEngine;

    public class Countdown
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
        #endregion

        #region Public Properties
        public Action<float> OnCountdownUpdated;
        public Action OnCountdownFinished;
        #endregion

        #region Unity Lifecycle
        public void Update()
        {
            if (_run)
            {
                if (_runningTime > 0)
                {
                    _runningTime = Math.Max(_runningTime - Time.deltaTime, 0);
                    OnCountdownUpdated?.Invoke(_runningTime);
                }
                else
                {
                    StopTimer();
                    OnCountdownFinished?.Invoke();
                }
            }
        }
        #endregion

        #region Public Methods
        public void StartTimer(uint timeMs)
        {
            _runningTime = (float)timeMs / 1000;
            Debug.Log("Starting with " + _runningTime + " seconds");
            _run = true;
        }

        public void StopTimer()
        {
            _runningTime = 0;
            _run = false;
        }
        #endregion
    }
}