namespace Duelo.Common.Model
{
    using System;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    [Serializable]
    public enum ConnectionStatus
    {
        [EnumMember(Value = "default")]
        Unknown,
        Offline,
        Online,
        Disconnected
    }

    [Serializable]
    public enum PlayerRole
    {
        [JsonProperty("defender")]
        Defender,
        [JsonProperty("challenger")]
        Challenger,
        [JsonProperty("unknown")]
        [EnumMember(Value = "default")]
        Unknown
    }

    [Serializable]
    public class MatchClockConfigurationDto
    {
        /// <summary>
        /// The initial time allowed for each round, in milliseconds.
        /// </summary>
        [JsonProperty("initialTimeAllowedMs")]
        public uint InitialTimeAllowedMs;

        /// <summary>
        /// The minimum time allowed for each round, in milliseconds. The action states will never be less than this.
        /// </summary>
        [JsonProperty("minTimeAllowedMs")]
        public uint MinTimeAllowedMs;

        /// <summary>
        /// The number of rounds after which the time will start decreasing.
        /// </summary>
        [JsonProperty("freeRounds")]
        public int FreeRounds;

        /// <summary>
        /// Used to calculate the timer scaling. After this many rounds, the timer will be at its minimum.
        /// </summary>
        [JsonProperty("expectedRounds")]
        public int ExpectedRounds;
    }

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

        [JsonProperty("clockConfig")]
        public MatchClockConfigurationDto ClockConfig;
        // [JsonProperty("mapConfig")]
        // public MatchMapConfigurationDto MapConfig;

        // [JsonProperty("challenger")]
        // public MatchPlayerProfileDto Challenger;
        // [JsonProperty("defender")]
        // public MatchPlayerProfileDto Defender;

        // public MatchPlayerProfileDto[] Players => new MatchPlayerProfileDto[] { Challenger, Defender };
    }
}