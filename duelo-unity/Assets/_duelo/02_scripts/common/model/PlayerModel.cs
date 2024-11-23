namespace Duelo.Common.Model
{
    using Newtonsoft.Json;

    public partial class PlayerProfileDto
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("gamertag")]
        public string Gamertag;
        [JsonProperty("characterUnitId")]
        public string CharacterUnitId;
    }
}