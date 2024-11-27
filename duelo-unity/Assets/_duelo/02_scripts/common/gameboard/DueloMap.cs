namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Core;
    using Duelo.Server.GameWorld;
    using UnityEngine;

    public class DueloMap : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// Tracks items that have already been instantiated
        /// </summary>
        private List<GameObject> _sceneObjects;
        #endregion

        #region Public Properties
        public DueloMapDto Map;
        public Vector3 ElementOffset = new Vector3(0.0f, -0.5f, 0.0f);
        #endregion

        #region Map Loading
        public void Load(DueloMapDto map)
        {
            Map = new DueloMapDto();
            _sceneObjects = new List<GameObject>();

            foreach (GridTileDto element in map.Tiles)
            {
                PlaceTile(element);
            }
        }

        private GameObject PlaceTile(GridTileDto element)
        {
            if (string.IsNullOrEmpty(element.Type))
            {
                return null;
            }

            GameObject obj = SpawnElement(element);
            if (obj != null)
            {
                Map.Tiles.Add(element);
                _sceneObjects.Add(obj);
            }
            return obj;
        }

        private GameObject SpawnElement(GridTileDto element)
        {
            GameObject obj = null;
            PrefabEntry entry;

            if (ServerData.Prefabs.PrefabLookup.TryGetValue(element.Type, out entry))
            {
                if (entry.prefab != null)
                {
                    Quaternion orientation = Quaternion.Euler(new Vector3(0.0f, element.Orientation * 90.0f, 0.0f));
                    obj = Instantiate(entry.prefab, element.Position + ElementOffset, orientation);
                }
            }
            else
            {
                throw new System.Exception($"[GameWorld]: Could not find prefab: {element.Type}");
            }

            return obj;
        }
        #endregion
    }
}