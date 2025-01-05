namespace Duelo.Common.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [Serializable]
    public class PlayerProfileDto
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("gamertag")]
        public string Gamertag;
        [JsonProperty("characterUnitId")]
        public string CharacterUnitId;
    }

    /// <summary>
    /// This is the profile a Duelo user will log in with
    /// </summary>
    [Serializable]
    public class DueloPlayerDto
    {
        [JsonProperty("playerId")]
        public string PlayerId;
        [JsonProperty("activeProfileId")]
        public string ActiveProfileId;
        [JsonProperty("profiles")]
        public Dictionary<string, PlayerProfileDto> Profiles;

        public PlayerProfileDto ActiveProfile => Profiles != null && ActiveProfileId != null ? Profiles[ActiveProfileId] : null;
    }
}