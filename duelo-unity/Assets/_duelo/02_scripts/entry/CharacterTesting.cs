namespace Duelo
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using Duelo.Server.GameWorld;
    using Duelo.Server.Match;
    using UnityEditor;
    using UnityEngine;

    public class CharacterTesting : MonoBehaviour
    {
        #region Public Properties
        public Dictionary<PlayerRole, MatchPlayer> Players => new();

        [Header("Challenger")]
        public Vector3 ChallengerSpawnPoint;
        public MatchPlayerDto Challenger;

        [Header("Defender")]
        public Vector3 DefenderSpawnPoint;
        public MatchPlayerDto Defender;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            ServerData.Prefabs = FindAnyObjectByType<PrefabList>();
            ServerData.Map = FindAnyObjectByType<DueloMap>();

            UniTask.Delay(1)
                .ContinueWith(SimulateAsyncDbLoad)
                .ContinueWith(SimulateAsyncPlayerLoad);
        }
        #endregion

        #region Loading
        private void SimulateAsyncDbLoad()
        {
            DueloMapDto mapDto = GenerateBoardMap();
            ServerData.Map.Load(mapDto);
        }

        private void SimulateAsyncPlayerLoad()
        {
            SpawnPlayer(PlayerRole.Challenger, Challenger);
            SpawnPlayer(PlayerRole.Defender, Defender);
        }
        #endregion

        #region Board
        private DueloMapDto GenerateBoardMap()
        {
            var map = new DueloMapDto();
            int size = 12;

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    map.Tiles.Add(new GridTileDto
                    {
                        Position = new Vector3(i, 0, j),
                        Scale = Vector3.one,
                        Type = "devtile"
                    });
                }
            }

            map.Tiles.Add(new GridTileDto
            {
                Position = ChallengerSpawnPoint,
                Scale = Vector3.one,
                Type = "spawn_challenger"
            });

            map.Tiles.Add(new GridTileDto
            {
                Position = DefenderSpawnPoint,
                Scale = Vector3.one,
                Type = "spawn_defender"
            });

            return map;
        }
        #endregion

        #region Players
        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            string prefabPath = $"Assets/_duelo/03_character/{playerDto.Profile.CharacterUnitId}.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                Application.Quit();
            }

            var spawnPoint = ServerData.Map.SpawnPoints[role];
            var gameObject = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, gameObject.GetComponent<MatchPlayer>());
        }
        #endregion
    }
}