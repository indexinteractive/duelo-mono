namespace Duelo
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using Duelo.Server.GameWorld;
    using Duelo.Server.Match;
    using UnityEditor;
    using UnityEngine;

    public class CharacterTesting : MonoBehaviour
    {
        #region Public Properties
        private Dictionary<PlayerRole, MatchPlayer> Players = new();

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
            ServerData.Kernel = new MatchKernel();

            UniTask.Delay(1)
                .ContinueWith(SimulateAsyncDbLoad)
                .ContinueWith(SimulateAsyncPlayerActions)
                .ContinueWith(SimulatePlayerExecute);
        }
        #endregion

        #region Loading
        private async UniTask SimulateAsyncDbLoad()
        {
            Debug.Log("Loading data");
            await UniTask.Delay(200);

            DueloMapDto mapDto = GenerateBoardMap();
            ServerData.Map.Load(mapDto);

            SpawnPlayer(PlayerRole.Challenger, Challenger);
            SpawnPlayer(PlayerRole.Defender, Defender);

            ServerData.Kernel.RegisterEntities(Players.Values.ToArray());
        }

        private async UniTask SimulateAsyncPlayerActions()
        {
            Debug.Log("Simulating player actions");
            await UniTask.Delay(200);

            var origin = Players[PlayerRole.Challenger].Position;
            var target = new Vector3(1, 0, 1);
            ServerData.Map.PaintPath(origin, target);

            ServerData.Kernel.QueuePlayerAction(PlayerRole.Challenger, MovementActionId.Walk, target);
            // TODO: when additional actions are queued, the position used is the original one
            ServerData.Kernel.QueuePlayerAction(PlayerRole.Challenger, MovementActionId.Walk, new Vector3(5, 0, 5));
            ServerData.Kernel.QueuePlayerAction(PlayerRole.Challenger, MovementActionId.Walk, new Vector3(2, 0, 2));
        }

        private async UniTask SimulatePlayerExecute()
        {
            Debug.Log("Simulating player execute");
            await UniTask.Delay(200);

            await ServerData.Kernel.RunRound();
            Debug.Log("Round finished");
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
                    var position = new Vector3(i, 0, j);
                    if (position == ChallengerSpawnPoint || position == DefenderSpawnPoint)
                    {
                        continue;
                    }

                    map.Tiles.Add(new GridTileDto
                    {
                        Position = position,
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

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();
            matchPlayer.Initialize("matchId", role, playerDto);
            Players.Add(role, matchPlayer);
        }
        #endregion
    }
}