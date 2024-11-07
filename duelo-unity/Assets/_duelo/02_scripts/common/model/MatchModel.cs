namespace Duelo.Common.Model
{
    using System;
    using Newtonsoft.Json;

    [Serializable]
    public class MatchDto
    {
        [JsonProperty("id")]
        public string MatchId;

        // [JsonProperty("servers")]
        // public MatchServersDto Servers;

        [JsonProperty("startTime")]
        public long StartTime;
        // [JsonProperty("state")]
        // [JsonConverter(typeof(StringEnumConverter))]
        // public MatchState State;
        // [JsonProperty("clockConfig")]
        // public MatchClockConfigurationDto ClockConfig;
        // [JsonProperty("mapConfig")]
        // public MatchMapConfigurationDto MapConfig;

        // [JsonProperty("challenger")]
        // public MatchPlayerProfileDto Challenger;
        // [JsonProperty("defender")]
        // public MatchPlayerProfileDto Defender;

        // public MatchPlayerProfileDto[] Players => new MatchPlayerProfileDto[] { Challenger, Defender };
    }
}