namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Util;
    using UnityEngine;

    /// <summary>
    /// The <see cref="DueloMap" /> class is a representation single game map.
    /// It is serializable and contains a list of all objects and their starting positions.
    /// </summary>
    public class DueloMap
    {
        #region Map Definition
        public string Name = Strings.DefaultMapName;
        public Dictionary<string, MapElement> Elements = new Dictionary<string, MapElement>();
        #endregion
    }

    [System.Serializable]
    public class MapElement
    {
        public string type;
        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 position = new Vector3(0, 0, 0);

        /// <summary>
        /// Tile orientation in 90° steps:
        /// 1 = 90°,
        /// 2 = 180°,
        /// etc.
        /// </summary>
        public int orientation = 0;

        public string GetStringKey()
        {
            return GetKeyForPosition(position);
        }

        public static string GetKeyForPosition(Vector3 position)
        {
            return "obj_" + position.ToString();
        }
    }
}