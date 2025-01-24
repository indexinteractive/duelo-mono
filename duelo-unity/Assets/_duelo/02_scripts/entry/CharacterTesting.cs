#define DUELO_LOCAL
#define UNITY_SERVER

namespace Duelo
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Gameboard;
    using UnityEngine;

    /// <summary>
    /// Helper struct to serialize actions in the editor
    /// </summary>
    [System.Serializable]
    public struct ActionEntry
    {
        #region Fields
        [Tooltip("The action id to execute")]
        public ActionDropdownItem ActionId;
        [Tooltip("Optional target position for actions that require it")]
        public Vector3 target;
        #endregion
    }

    /// <summary>
    /// Attached to Assets/_duelo/01_scenes/testing/CharacterTesting
    /// Used to test character actions in a pre scripted way
    /// </summary>
    public class CharacterTesting : MonoBehaviour
    {
        #region Public Properties
        private Dictionary<PlayerRole, MatchPlayer> Players = new();
        [Header("Match Settings")]
        [Tooltip("The firebase Id of the map to load")]
        public string MapId = "devmap";

        [Header("Challenger")]
        public Vector3 ChallengerSpawnPoint;
        public string ChallengerPlayerId = "TEST_PLAYER_1";
        public ActionEntry[] ChallengerActions;

        [Header("Defender")]
        public Vector3 DefenderSpawnPoint;
        public string DefenderPlayerId = "TEST_PLAYER_2";
        public ActionEntry[] DefenderActions;
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            Debug.Log("[CharacterTesting] Starting character testing scene");
            yield return Ind3x.Util.FirebaseInstance.Instance.Initialize("CHARACTER_TESTING", false);
        }

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

            DueloMapDto mapDto = await MapService.Instance.GetMap(MapId);
            GameData.Map.Load(mapDto);
            GameData.Camera.SetMapCenter(GameData.Map.MapCenter);

            await SpawnPlayer(PlayerRole.Challenger, ChallengerPlayerId);
            await SpawnPlayer(PlayerRole.Defender, DefenderPlayerId);

            GameData.Camera.FollowPlayers(Players);
            GameData.Kernel.RegisterEntities(Players.Values.ToArray());
        }

        private async UniTask SimulateAsyncPlayerActions()
        {
            Debug.Log("Simulating player actions");
            await UniTask.Delay(200);

            foreach (var entry in ChallengerActions)
            {
                if (ActionId.IsMovementAction((int)entry.ActionId))
                {
                    GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, MovementActionId.Walk, entry.target);
                }
                else
                {
                    GameData.Kernel.QueuePlayerAction(PlayerRole.Challenger, (int)entry.ActionId);
                }
            }

            foreach (var entry in DefenderActions)
            {
                if (ActionId.IsMovementAction((int)entry.ActionId))
                {
                    GameData.Kernel.QueuePlayerAction(PlayerRole.Defender, MovementActionId.Walk, entry.target);
                }
                else
                {
                    GameData.Kernel.QueuePlayerAction(PlayerRole.Defender, (int)entry.ActionId);
                }
            }
        }

        private async UniTask SimulatePlayerExecute()
        {
            Debug.Log("Simulating player execute");
            await UniTask.Delay(200);

            await GameData.Kernel.RunRound();
            Debug.Log("Round finished");
        }
        #endregion

        #region Players
        public async UniTask SpawnPlayer(PlayerRole role, string playerId)
        {
            var playerDto = await PlayerService.Instance.GetPlayerById(playerId);
            MatchPlayerDto matchPlayerDto = new MatchPlayerDto
            {
                UnityPlayerId = playerId,
                Profile = playerDto.Profiles.Values.FirstOrDefault()
            };

            GameObject prefab = GameData.Prefabs.CharacterLookup[matchPlayerDto.Profile.CharacterUnitId];

            var spawnPoint = GameData.Map.SpawnPoints[role];
            var gameObject = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"[CharacterTesting] Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();
            matchPlayer.Initialize("matchId", role, matchPlayerDto);
            Players.Add(role, matchPlayer);
        }
        #endregion
    }
}