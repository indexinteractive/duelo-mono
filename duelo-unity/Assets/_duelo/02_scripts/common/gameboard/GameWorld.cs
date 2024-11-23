namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Server.GameWorld;
    using UnityEngine;

    public class GameWorld : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// Tracks items that have already been instantiated
        /// </summary>
        private Dictionary<string, GameObject> _sceneObjects;
        #endregion

        #region Public Properties
        public DueloMap Map;
        public Vector3 ElementOffset = new Vector3(0.0f, -0.5f, 0.0f);
        #endregion

        #region Map Loading
        public void Load(DueloMap map)
        {
            Map = new DueloMap();
            _sceneObjects = new Dictionary<string, GameObject>();

            foreach (MapElement element in map.Elements.Values)
            {
                PlaceTile(element);
            }
        }

        private GameObject PlaceTile(MapElement element)
        {
            if (string.IsNullOrEmpty(element.type))
            {
                return null;
            }

            string key = element.GetStringKey();

            GameObject obj = SpawnElement(element);
            if (obj != null)
            {
                Map.Elements.Add(key, element);
                _sceneObjects.Add(element.GetStringKey(), obj);
            }
            return obj;
        }

        private GameObject SpawnElement(MapElement element)
        {
            GameObject obj = null;
            PrefabEntry entry;

            if (ServerData.Prefabs.PrefabLookup.TryGetValue(element.type, out entry))
            {
                if (entry.prefab != null)
                {
                    Quaternion orientation = Quaternion.Euler(new Vector3(0.0f, element.orientation * 90.0f, 0.0f));
                    obj = Instantiate(entry.prefab, element.position + ElementOffset, orientation);
                }
            }
            else
            {
                throw new System.Exception($"[GameWorld]: Could not find prefab: {element.type}");
            }

            return obj;
        }
        #endregion
    }
}