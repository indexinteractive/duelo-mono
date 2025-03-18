namespace Duelo.Common.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using UnityEngine;

    /// <summary>
    /// This class helps maintain sync between server and client.
    /// Each time the server changes to a different <see cref="Server.State.ServerMatchState"/>,
    /// it will pause execution by means of <see cref="Server.Match.ServerMatch.WaitForSyncState"/>
    /// until both clients have confirmed the state change.
    /// </summary>
    [Serializable]
    public class SyncStateDto
    {
        [JsonProperty("server")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchState Server;
        [JsonProperty("defender")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchState? Defender;
        [JsonProperty("challenger")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchState? Challenger;
    }

    /// <summary>
    /// The state of a player during a given round, including
    /// health, position, etc
    /// Used for both players in <see cref="RoundStatesDto"/>
    /// </summary>
    [Serializable]
    public class PlayerRoundStateDto
    {
        [JsonProperty("health")]
        public float Health;
        [JsonProperty("position")]
        [JsonConverter(typeof(Ind3x.Serialization.Vector3Converter))]
        public Vector3 Position;
    }

    /// <summary>
    /// Contains both individual player states for a given round,
    /// defined by <see cref="PlayerRoundStateDto"/>
    /// </summary>
    [Serializable]
    public class RoundStatesDto
    {
        [JsonProperty("defender")]
        public PlayerRoundStateDto Defender;
        [JsonProperty("challenger")]
        public PlayerRoundStateDto Challenger;
    }

    [Serializable]
    public class MatchRoundDto
    {
        [JsonProperty("roundNumber")]
        public int RoundNumber;
        [JsonProperty("playerState")]
        public RoundStatesDto PlayerState;
        [JsonProperty("movement")]
        public MovementPhaseDto Movement;
        [JsonProperty("action")]
        public ActionPhaseDto Action;
    }

    [Serializable]
    public class MovementPhaseDto
    {
        [JsonProperty("timer")]
        public uint Timer;
        [JsonProperty("defender")]
        public PlayerRoundMovementDto Defender;
        [JsonProperty("challenger")]
        public PlayerRoundMovementDto Challenger;
    }

    [Serializable]
    public class PlayerRoundMovementDto
    {
        /// <summary>
        /// The <see cref="Model.MovementActionId"/> associated with the movement sent by the player
        /// </summary>
        [JsonProperty("actionId")]
        public int ActionId;

        /// <summary>
        /// Intended position for the player to move to
        /// </summary>
        [JsonProperty("targetPosition")]
        [JsonConverter(typeof(Ind3x.Serialization.Vector3Converter))]
        public Vector3 TargetPosition;
    }

    [Serializable]
    public class ActionPhaseDto
    {
        [JsonProperty("timer")]
        public uint Timer;
        [JsonProperty("defender")]
        public PlayerRoundActionDto Defender;
        [JsonProperty("challenger")]
        public PlayerRoundActionDto Challenger;
    }

    [Serializable]
    public class PlayerRoundActionDto
    {
        [JsonProperty("actionId")]
        public int ActionId;
    }
}