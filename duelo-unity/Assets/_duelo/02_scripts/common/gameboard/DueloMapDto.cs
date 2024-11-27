namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Util;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    /// <summary>
    /// Represents a single game map.
    /// </summary>
    [System.Serializable]
    public class DueloMapDto
    {
        #region Map Definition
        [JsonProperty(PropertyName = "name")]
        public string Name = Strings.DefaultMapName;
        [JsonProperty(PropertyName = "tiles")]
        public Dictionary<string, GridTileDto> Tiles = new Dictionary<string, GridTileDto>();
        #endregion
    }

    [System.Serializable]
    public class GridTileDto
    {
        [JsonProperty(PropertyName = "type")]
        public string Type;
        [JsonProperty(PropertyName = "scale")]
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 Scale = new Vector3(1, 1, 1);
        [JsonProperty(PropertyName = "position")]
        [JsonConverter(typeof(Vector3Converter))]
        public Vector3 Position = new Vector3(0, 0, 0);

        /// <summary>
        /// Tile orientation in 90° steps:
        /// 1 = 90°,
        /// 2 = 180°,
        /// etc.
        /// </summary>
        [JsonProperty(PropertyName = "orientation")]
        public int Orientation = 0;

        public string GetStringKey()
        {
            string positionKey = GetKeyForPosition(Position);
            return $"{Type}_{positionKey}";
        }

        public static string GetKeyForPosition(Vector3 position)
        {
            string x = position.x.ToString("F2").Replace(".", "_");
            string y = position.y.ToString("F2").Replace(".", "_");
            string z = position.z.ToString("F2").Replace(".", "_");

            return $"{x}_{y}_{z}";
        }
    }
}