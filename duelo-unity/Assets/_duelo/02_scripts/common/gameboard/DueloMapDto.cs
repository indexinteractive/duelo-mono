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

        /// <summary>
        /// This class must be a subclass of <see cref="Duelo.Gameboard.MapDecorator"/>, in the same namespace
        /// </summary>
        [JsonProperty(PropertyName = "decoratorClass")]
        public string DecoratorClass;
        [JsonProperty(PropertyName = "tiles")]
        public List<GridTileDto> Tiles = new();
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
    }
}