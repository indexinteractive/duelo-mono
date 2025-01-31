namespace Duelo.Common.Model
{
    using System;
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
        [JsonProperty("round")]
        public int? Round;
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

    [Serializable]
    public class MatchRoundDto
    {
        [JsonProperty("roundNumber")]
        public int RoundNumber;
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