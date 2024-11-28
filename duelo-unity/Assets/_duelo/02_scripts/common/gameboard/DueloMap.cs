namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Duelo.Server.GameWorld;
    using Duelo.Server.Match;
    using UnityEditor;
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

        #region Special Tiles
        public GameObject DefenderSpawn;
        public GameObject ChallengerSpawn;
        #endregion

        #region Map Loading
        public void Load(DueloMapDto map)
        {
            Map = new DueloMapDto();
            _sceneObjects = new List<GameObject>();

            foreach (GridTileDto element in map.Tiles)
            {
                var obj = PlaceTile(element);

                if (element.Type == SpecialTiles.ChallengerSpawn)
                {
                    ChallengerSpawn = obj;
                }
                else if (element.Type == SpecialTiles.DefenderSpawn)
                {
                    DefenderSpawn = obj;
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

        #region Players
        public void SpawnPlayer(MatchPlayer player)
        {
            string prefabPath = $"Assets/_duelo/03_character/{player.ProfileDto.CharacterUnitId}.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                Application.Quit();
            }

            GameObject obj = null;

            if (player.Role == PlayerRole.Challenger)
            {
                obj = Instantiate(prefab, ChallengerSpawn.transform.position, ChallengerSpawn.transform.rotation);
            }
            else if (player.Role == PlayerRole.Defender)
            {
                obj = Instantiate(prefab, DefenderSpawn.transform.position, DefenderSpawn.transform.rotation);
            }

            Debug.Log($"Character spawned for {player.Role} at {obj.transform.position}");
        }
        #endregion
    }
}