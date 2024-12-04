namespace Duelo.Common.Model
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

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
        /// The <see cref="Model.ActionId"/> associated with the movement sent by the player
        /// </summary>
        [JsonProperty("actionId")]
        public int ActionId;

        /// <summary>
        /// Intended position for the player to move to
        /// </summary>
        [JsonProperty("targetPosition")]
        public Vector3 TargetPosition;
    }

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
        public int ActionId;
    }
}