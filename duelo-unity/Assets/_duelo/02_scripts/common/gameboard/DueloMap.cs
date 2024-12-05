namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Duelo.Server.GameWorld;
    using UnityEngine;

    public class DueloMap : MonoBehaviour
    {
        #region Private Fields
        /// <summary>
        /// Tracks items that have already been instantiated
        /// </summary>
        private List<GameObject> _sceneObjects;

        /// <summary>
        /// Tile lookup used to access tiles by their position
        /// </summary>
        public Dictionary<string, MapTile> _tiles = new();
        #endregion

        #region Public Properties
        public DueloMapDto Map;
        #endregion

        #region Special Tiles
        public Dictionary<PlayerRole, GameObject> SpawnPoints = new();
        #endregion

        #region Map Loading
        public void Load(DueloMapDto map)
        {
            Map = new DueloMapDto();
            _tiles.Clear();
            _sceneObjects = new List<GameObject>();

            foreach (GridTileDto element in map.Tiles)
            {
                var obj = PlaceTile(element);

                if (element.Type == SpecialTiles.ChallengerSpawn)
                {
                    SpawnPoints[PlayerRole.Challenger] = obj;
                }
                else if (element.Type == SpecialTiles.DefenderSpawn)
                {
                    SpawnPoints[PlayerRole.Defender] = obj;
                }
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

            if (ServerData.Prefabs.TileLookup.TryGetValue(element.Type, out entry))
            {
                if (entry.prefab != null)
                {
                    Quaternion orientation = Quaternion.Euler(new Vector3(0.0f, element.Orientation * 90.0f, 0.0f));
                    obj = Instantiate(entry.prefab, element.Position, orientation);

                    var tile = obj.GetComponent<MapTile>();

                    var id = GetTileId(tile.transform.position);
                    _tiles.Add(id, tile);
                }
            }
            else
            {
                throw new System.Exception($"[GameWorld]: Could not find prefab: {element.Type}");
            }

            return obj;
        }
        #endregion

        #region Pathfinding
        /// <summary>
        /// Generates a unique integer id for a cell based on its position
        /// </summary>
        public string GetTileId(Vector3 position)
        {
            return $"{position.x}{position.y}{position.z}";
        }

        public MapTile GetTile(Vector3 targetPosition)
        {
            var key = GetTileId(targetPosition);

            if (_tiles.ContainsKey(key))
            {
                return _tiles[key];
            }

            return null;
        }
        #endregion
    }
}