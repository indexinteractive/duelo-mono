namespace Duelo.Common.Model
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class RoundDto
    {
        public int RoundNumber;
        public int TimeAllowedMs;

        public RoundDto(int roundNumber, uint timeAllowedMs)
        {
            RoundNumber = roundNumber;
            TimeAllowedMs = (int)timeAllowedMs;
        }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "roundNumber", RoundNumber },
                { "timeAllowedMs", TimeAllowedMs }
            };
        }
    }
}