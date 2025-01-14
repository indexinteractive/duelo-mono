namespace Ind3x.Util
{
    using System;
    using System.Threading;
    using UnityEngine;

    public class AppQuitTimer
    {
        #region Static Initialization
        /// <summary>
        /// A convenience method that creates an instance of <see cref="AppQuitTimer"/> and starts it immediately.
        /// </summary>
        public static AppQuitTimer RunInstance(int expirationTimeSec)
        {
            var timer = new AppQuitTimer(expirationTimeSec);
            timer.Start();
            return timer;
        }
        #endregion

        #region Private Fields
        private Timer _timer;
        private readonly TimeSpan _expirationTime;
        #endregion

        #region Initialization
        public AppQuitTimer(int expirationTimeSec)
        {
            _expirationTime = expirationTimeSec > 0 ? TimeSpan.FromSeconds(expirationTimeSec) : Timeout.InfiniteTimeSpan;
        }

        public AppQuitTimer(TimeSpan expirationTime)
        {
            _expirationTime = expirationTime > TimeSpan.Zero ? expirationTime : Timeout.InfiniteTimeSpan;
        }
        #endregion

        #region Public Methods
        public void Start()
        {
            if (_expirationTime != Timeout.InfiniteTimeSpan)
            {
                Debug.Log($"[AppQuitTimer] Application will self destruct in {_expirationTime.TotalSeconds} seconds.");
                _timer = new Timer(QuitApplication, null, _expirationTime, Timeout.InfiniteTimeSpan);
            }
        }

        public void Cancel()
        {
            Debug.Log("[AppQuitTimer] Canceling timer.");
            _timer?.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Timer Execution
        private void QuitApplication(object state)
        {
            Debug.Log("[AppQuitTimer] No players joined within the expiration time. Quitting application.");
            Application.Quit(ExitCode.TimerExpired);
        }
        #endregion
    }
}