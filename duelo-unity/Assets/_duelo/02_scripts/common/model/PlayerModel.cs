namespace Duelo.Common.Model
{
    using System;
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
}