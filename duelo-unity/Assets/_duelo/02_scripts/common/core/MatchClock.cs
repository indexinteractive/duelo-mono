namespace Duelo.Common.Core
{
    using Duelo.Common.Model;
    using UnityEngine;

    public class MatchClock
    {
        #region Private Fields
        private MatchClockConfigurationDto _config;
        #endregion

        #region Public Properties
        /// <summary>
        /// The current round
        /// </summary>
        public int CurrentRound { get; private set; }

        /// <summary>
        /// The current time available for action states, in milliseconds.
        /// </summary>
        public uint CurrentTimeAllowedMs { get; private set; }
        #endregion

        #region Initialization
        public MatchClock(MatchClockConfigurationDto config)
        {
            _config = config;
            Reset();
        }

        private void Reset()
        {
            CurrentRound = -1;
            CurrentTimeAllowedMs = _config.InitialTimeAllowedMs;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Should be called at the start of every round to calculate new timers
        /// </summary>
        public void NewRound()
        {
            CurrentRound++;
            UpdateTimers();
        }
        #endregion

        #region Timer Updates
        private void UpdateTimers()
        {
            CurrentTimeAllowedMs = CalculateNonLinearScale(CurrentRound - 1);
            CurrentTimeAllowedMs = (uint)Mathf.Max(CurrentTimeAllowedMs, _config.MinTimeAllowedMs);
        }

        private uint CalculateNonLinearScale(int round)
        {
            float t = Mathf.Clamp((float)(round - _config.FreeRounds) / _config.ExpectedRounds, 0.0f, 1.0f);
            return (uint)(_config.InitialTimeAllowedMs * (1.0f - Mathf.Pow(t, 2)));
        }

        public MatchClockConfigurationDto ToDto()
        {
            return _config;
        }
        #endregion
    }
}