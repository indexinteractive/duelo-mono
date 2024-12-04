namespace Duelo.Common.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [Serializable]
    public enum MatchState
    {
        /**
        * Waiting for dependencies
        */
        [JsonProperty("startup")]
        Startup,

        /**
         * Match has been created but still initializing
         */
        [JsonProperty("pending")]
        Pending,

        /**
         * Match is in the lobby and players can join
         */
        [JsonProperty("lobby")]
        Lobby,

        /**
         * Match is in progress
         */
        [JsonProperty("ingame")]
        InGame,

        /**
         * Match has been paused due to connection issues or other reasons
         */
        [JsonProperty("paused")]
        Paused,

        /**
         * Match has finished, results have been posted to the database
         */
        [JsonProperty("finished")]
        Finished,

        [JsonProperty("error")]
        Error
    }

    [Serializable]
    public enum ConnectionStatus
    {
        [JsonProperty("unknown")]
        Unknown,
        [JsonProperty("offline")]
        Offline,
        [JsonProperty("online")]
        Online,
        [JsonProperty("disconnected")]
        Disconnected
    }

    [Serializable]
    public enum PlayerRole
    {
        [JsonProperty("unknown")]
        Unknown,
        [JsonProperty("defender")]
        Defender,
        [JsonProperty("challenger")]
        Challenger,
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
    public class MatchPlayerDto
    {
        [JsonProperty("playerId")]
        public string PlayerId;
        [JsonProperty("deviceId")]
        public string DeviceId;
        [JsonProperty("joinDate")]
        public DateTime? JoinDate;
        [JsonProperty("connection")]
        [JsonConverter(typeof(Ind3x.Util.DefaultStringEnumConverter))]
        public ConnectionStatus Connection;
        [JsonProperty("profile")]
        public PlayerProfileDto Profile;
    }

    public class MatchPlayersDto
    {
        [JsonProperty("challenger")]
        public MatchPlayerDto Challenger;
        [JsonProperty("defender")]
        public MatchPlayerDto Defender;
    }

    [Serializable]
    public class MatchDto
    {
        [JsonProperty("id")]
        public string MatchId;

        [JsonProperty("createdTime")]
        public DateTime? CreatedTime;

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchState State;

        [JsonProperty("clockConfig")]
        public MatchClockConfigurationDto ClockConfig;

        [JsonProperty("players")]
        public MatchPlayersDto Players;

        [JsonProperty("rounds")]
        public IEnumerable<MatchRoundDto> Rounds;

        /// <summary>
        /// The key for the map that will be loaded
        /// </summary>
        [JsonProperty("mapId")]
        public string MapId;
    }
}