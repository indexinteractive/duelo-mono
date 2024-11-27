namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Server.GameWorld;
    using UnityEngine;

    public class DueloMap : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// Tracks items that have already been instantiated
        /// </summary>
        private Dictionary<string, GameObject> _sceneObjects;
        #endregion

        #region Public Properties
        public DueloMapDto Map;
        public Vector3 ElementOffset = new Vector3(0.0f, -0.5f, 0.0f);
        #endregion

        #region Map Loading
        public void Load(DueloMapDto map)
        {
            Map = new DueloMapDto();
            _sceneObjects = new Dictionary<string, GameObject>();

            foreach (GridTileDto element in map.Tiles.Values)
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

            string key = element.GetStringKey();

            GameObject obj = SpawnElement(element);
            if (obj != null)
            {
                Map.Tiles.Add(key, element);
                _sceneObjects.Add(element.GetStringKey(), obj);
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