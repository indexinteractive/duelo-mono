namespace Duelo.Common.Model
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents the current state of the server during a match
    /// </summary>
    [Serializable]
    public enum MatchState
    {
        /// <summary>
        /// The default state when a match has been created in <see cref="Server.Match.ServerMatch"/>.
        /// At this point, the match has not been published to the database.
        /// </summary>
        [JsonProperty("startup")]
        Startup,

        /// <summary>
        /// Match has been created and published, but is still initializing.
        /// Should load the map and spawn players.
        /// Set in <see cref="Server.State.StateMatchStartup.OnEnter"/>.
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
        /// Wait for player movements to be written to rounds/:roundId/movement
        /// Move on to next phase even if movements are not chosen in time.
        /// Set in <see cref="Server.State.StateChooseMovement.OnEnter"/>
        /// </summary>
        [JsonProperty("choosemovement")]
        ChooseMovement,

        /// <summary>
        /// Wait for player actions to be written to rounds/:roundId/action
        /// Move on if actions are not chosen in time.
        /// Set in <see cref="Server.State.StateChooseAction.OnEnter"/>
        /// </summary>
        [JsonProperty("chooseaction")]
        ChooseAction,

        /// <summary>
        /// Any actions that depend on movement or opponent actions are resolved here.
        /// This should allow actions that depend on opponent actions to be implemented.
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
        /// Execute phase has finished, waiting for clients to sync
        /// Set in <see cref="Server.State.StateExecuteRound"/>
        /// </summary>
        [JsonProperty("executeroundfinished")]
        ExecuteRoundFinished,

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

    /// <summary>
    /// Represents a player in a match:
    ///  path: <match_id>.players.<role>
    /// </summary>
    [Serializable]
    public class MatchPlayerDto
    {
        [JsonProperty("unityPlayerId")]
        public string UnityPlayerId;
        [JsonProperty("joinDate")]
        public DateTime? JoinDate;
        [JsonProperty("connection")]
        [JsonConverter(typeof(Ind3x.Util.DefaultStringEnumConverter))]
        public ConnectionStatus Connection;
        [JsonProperty("profile")]
        public PlayerProfileDto Profile;
    }

    [Serializable]
    public class MatchPlayersDto
    {
        [JsonProperty("challenger")]
        public MatchPlayerDto Challenger;
        [JsonProperty("defender")]
        public MatchPlayerDto Defender;

        public static MatchPlayersDto FromMatchmakerData(Unity.Services.Matchmaker.Models.MatchmakingResults matchmakerData)
        {
            var matchPlayers = new MatchPlayersDto();

            var p1 = matchmakerData.MatchProperties.Players[0];
            var p2 = matchmakerData.MatchProperties.Players[1];

            var p1Team = matchmakerData.MatchProperties.Teams.Where(t => t.PlayerIds.Contains(p1.Id)).FirstOrDefault();
            var p2Team = matchmakerData.MatchProperties.Teams.Where(t => t.PlayerIds.Contains(p2.Id)).FirstOrDefault();

            if (p1Team.TeamName == "challenger")
            {
                matchPlayers.Challenger = new MatchPlayerDto
                {
                    UnityPlayerId = p1.Id,
                    Profile = p1.CustomData.GetAs<PlayerProfileDto>()
                };

                matchPlayers.Defender = new MatchPlayerDto
                {
                    UnityPlayerId = p2.Id,
                    Profile = p2.CustomData.GetAs<PlayerProfileDto>()
                };
            }
            else if (p2Team.TeamName == "challenger")
            {
                matchPlayers.Challenger = new MatchPlayerDto
                {
                    UnityPlayerId = p2.Id,
                    Profile = p2.CustomData.GetAs<PlayerProfileDto>()
                };

                matchPlayers.Defender = new MatchPlayerDto
                {
                    UnityPlayerId = p1.Id,
                    Profile = p1.CustomData.GetAs<PlayerProfileDto>()
                };
            }

            return matchPlayers;
        }
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
        public MatchRoundDto[] Rounds;

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