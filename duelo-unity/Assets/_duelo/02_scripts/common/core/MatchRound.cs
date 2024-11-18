namespace Duelo.Common.Core
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Model;

    public class MatchRound
    {
        #region Private Fields
        public readonly int RoundNumber;
        public readonly uint TimeAllowed;
        public readonly DateTime StartTime;

        public MovementPhaseDto Movement;
        #endregion

        #region Initialization
        public MatchRound(int currentRound, uint currentTimeAllowedMs)
        {
            RoundNumber = currentRound;
            TimeAllowed = currentTimeAllowedMs;
            StartTime = DateTime.UtcNow;
        }
        #endregion

        #region Serialization
        public MatchRoundDto ToDto()
        {
            return new MatchRoundDto()
            {
                RoundNumber = RoundNumber,
            };
        }
        #endregion

    }
}