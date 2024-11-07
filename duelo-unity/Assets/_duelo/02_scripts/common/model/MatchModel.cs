namespace Duelo.Common.Model
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class MatchDto
    {
        [JsonProperty("id")]
        public string MatchId;

        [JsonProperty("startTime")]
        public long StartTime;
    }
}