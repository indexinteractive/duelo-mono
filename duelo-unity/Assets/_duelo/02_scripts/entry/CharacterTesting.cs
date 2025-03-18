#define DUELO_LOCAL
#define UNITY_SERVER

namespace Duelo
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
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
        [ActionId]
        public int ActionId;
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
        [Header("Match Settings")]
        [Tooltip("The firebase MatchDto data that would come from firebase during a game")]
        public MatchDto MatchDto;
        public GameObject[] StaticEnemies;

        [Header("Challenger")]
        public ActionEntry[] ChallengerActions;

        [Header("Defender")]
        public ActionEntry[] DefenderActions;
        #endregion

        #region Private Fields
        private ClientMatch _match => GlobalState.Match as ClientMatch;
        private MockService _services;
        #endregion

        #region Unity Lifecycle
        public void Awake()
        {
            _services = new MockService(MatchDto);

            GlobalState.StartupOptions = new StartupOptions(StartupMode.Client, new string[] { });
            GlobalState.Prefabs = FindAnyObjectByType<PrefabList>();
            GlobalState.Map = FindAnyObjectByType<DueloMap>();
            GlobalState.Kernel = new MatchKernel();
            GlobalState.Camera = FindAnyObjectByType<DueloCamera>();

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

            DueloMapDto mapDto = await _services.GetMap(MatchDto.MapId);
            GlobalState.Map.Load(mapDto);
            GlobalState.Camera.SetMapCenter(GlobalState.Map.MapCenter);

            GlobalState.Match = new MockMatch(MatchDto);

            _match.LoadAssets();

            GlobalState.Camera.FollowPlayers(_match.Players.ToDictionary(x => x.Key, kvp => kvp.Value));
            GlobalState.Kernel.RegisterEntities(_match.Players[PlayerRole.Challenger], _match.Players[PlayerRole.Defender]);

            foreach (var enemy in StaticEnemies)
            {
                enemy.SetActive(true);
            }
        }

        private async UniTask SimulateAsyncPlayerActions()
        {
            void _queueActions(PlayerRole role, ActionEntry[] actions)
            {
                foreach (var entry in actions)
                {
                    if (ActionId.IsMovementAction((int)entry.ActionId))
                    {
                        GlobalState.Kernel.QueuePlayerAction(role, (int)entry.ActionId, traits => traits.Movements, entry.target);
                    }
                    else
                    {
                        GlobalState.Kernel.QueuePlayerAction(role, (int)entry.ActionId, traits => traits.Attacks.Concat(traits.Defenses));
                    }
                }
            }

            Debug.Log("Simulating player actions");
            await UniTask.Delay(200);

            _queueActions(PlayerRole.Challenger, ChallengerActions);
            _queueActions(PlayerRole.Defender, DefenderActions);
        }

        private async UniTask SimulatePlayerExecute()
        {
            Debug.Log("Simulating player execute");
            await UniTask.Delay(200);

            await GlobalState.Kernel.RunRound();
            Debug.Log("Round finished");
        }
        #endregion
    }
}