namespace Duelo
{
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
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
            GameData.Prefabs = FindAnyObjectByType<PrefabList>();
            GameData.Map = FindAnyObjectByType<DueloMap>();
            GameData.Kernel = new MatchKernel();
            GameData.Camera = FindAnyObjectByType<DueloCamera>();

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
            GameData.Map.Load(mapDto);
            GameData.Camera.SetMapCenter(GameData.Map.MapCenter);

            SpawnPlayer(PlayerRole.Challenger, Challenger);
            SpawnPlayer(PlayerRole.Defender, Defender);

            GameData.Camera.FollowPlayers(Players);
            GameData.Kernel.RegisterEntities(Players.Values.ToArray());
        }

        private async UniTask SimulateAsyncPlayerActions()
        {
            Debug.Log("Simulating player actions");
            await UniTask.Delay(200);

            GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, AttackActionId.CannonFire);

            QueueMovement(Players[PlayerRole.Challenger], new Vector3(1, 0, 1));
            GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, AttackActionId.CloseRange);

            QueueMovement(Players[PlayerRole.Challenger], new Vector3(5, 0, 5));
            GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, AttackActionId.CannonFire);

            QueueMovement(Players[PlayerRole.Challenger], new Vector3(2, 0, 2));
            GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, AttackActionId.CloseRange);
        }

        private async UniTask SimulatePlayerExecute()
        {
            Debug.Log("Simulating player execute");
            await UniTask.Delay(200);

            await GameData.Kernel.RunRound();
            Debug.Log("Round finished");
        }
        #endregion

        #region Board
        private DueloMapDto GenerateBoardMap()
        {
            var map = new DueloMapDto();
            map.DecoratorClass = "BasicMapDecorator";
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

            var spawnPoint = GameData.Map.SpawnPoints[role];
            var gameObject = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();
            matchPlayer.Initialize("matchId", role, playerDto);
            Players.Add(role, matchPlayer);
        }
        #endregion

        #region Helpers
        private void QueueMovement(MatchPlayer player, Vector3 target)
        {
            var origin = player.Position;
            GameData.Map.PaintPath(origin, target);
            GameData.Kernel.QueuePlayerAction(player.Role, MovementActionId.Walk, target);
        }
        #endregion
    }
}