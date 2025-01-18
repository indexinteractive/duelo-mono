namespace Duelo.Common.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents the current state of the server during a match
    /// </summary>
    [Serializable]
    public enum MatchState
    {
        /// <summary>
        /// The default state when a match has been created in <see cref="Server.Match.ServerMatch.ServerMatch(MatchDto)"/>.
        /// No db values have been set at this point.
        /// </summary>
        [JsonProperty("startup")]
        Startup,

        /// <summary>
        /// Match has been created, set db values, but is still initializing.
        /// Set in <see cref="Server.State.StateMatchStartup.OnEnter"/>
        /// </summary>
        [JsonProperty("pending")]
        Pending,

        /// <summary>
        /// Match is waiting for players to join.
        /// Set in <see cref="Server.State.StateMatchLobby.OnEnter"/>
        /// </summary>
        [JsonProperty("lobby")]
        Lobby,

        /// <summary>
        /// Players have joined, Match is about to begin.
        /// Set in <see cref="Server.State.StateInitializeGame.OnEnter"/>
        /// </summary>
        [JsonProperty("initialize")]
        Initialize,

        /// <summary>
        /// A new round has started, update clients with timer values, etc.
        /// Set in <see cref="Server.State.StateBeginRound.OnEnter"/>
        /// </summary>
        [JsonProperty("beginround")]
        BeginRound,

        /// <summary>
        /// Wait for player movements. Move on if movements are not chosen in time.
        /// Set in <see cref="Server.State.StateChooseMovement.OnEnter"/>
        /// </summary>
        [JsonProperty("choosemovement")]
        ChooseMovement,

        /// <summary>
        /// Wait for player actions. Move on if actions are not chosen in time.
        /// Set in <see cref="Server.State.StateChooseAction.OnEnter"/>
        /// </summary>
        [JsonProperty("chooseaction")]
        ChooseAction,

        /// <summary>
        /// Any actions that depend on movement or opponent actions are resolved here.
        /// Set in <see cref="Server.State.StateLateActions.OnEnter"/>
        /// </summary>
        [JsonProperty("lateactions")]
        LateActions,

        /// <summary>
        /// Stored movements and action play out in <see cref="Kernel.MatchKernel"/>.
        /// Set in <see cref="Server.State.StateExecuteRound.OnEnter"/>
        /// </summary>
        [JsonProperty("executeround")]
        ExecuteRound,

        /// <summary>
        /// Round is over, set final health, movement, etc in db.
        /// Set in <see cref="Server.State.StateEndRound.OnEnter"/>
        /// </summary>
        [JsonProperty("endround")]
        EndRound,

        /// <summary>
        /// Match has been paused due to connection issues or other reasons
        /// </summary>
        [JsonProperty("paused")]
        Paused,

        /// <summary>
        /// Match has finished, results have been posted to the database
        /// </summary>
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

        [JsonProperty("clockConfig")]
        public MatchClockConfigurationDto ClockConfig;

        [JsonProperty("players")]
        public MatchPlayersDto Players;

        [JsonProperty("rounds")]
        public IEnumerable<MatchRoundDto> Rounds;

        [JsonProperty("sync")]
        public SyncStateDto SyncState;

        /// <summary>
        /// The key for the map that will be loaded
        /// </summary>
        [JsonProperty("mapId")]
        public string MapId;

        [JsonProperty("matchmaker")]
        public Unity.Services.Matchmaker.Models.MatchmakingResults MatchmakerDto;

        public static bool IsMatchLoopState(MatchState state)
        {
            return state == MatchState.BeginRound
                || state == MatchState.ChooseMovement
                || state == MatchState.ChooseAction
                || state == MatchState.LateActions
                || state == MatchState.ExecuteRound
                || state == MatchState.EndRound;
        }

    }
}