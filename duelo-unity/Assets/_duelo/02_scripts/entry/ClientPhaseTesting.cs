namespace Duelo
{
    using System.Collections;
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

    public class ClientPhaseTesting : MonoBehaviour
    {
        #region Public Properties
        [Header("Initialization Settings")]
        [Tooltip("The firebase MatchDto data that would come from firebase during a game")]
        public MatchDto MatchDto;

        [Header("Spawn Points")]
        public Vector3 ChallengerSpawnPoint;
        public Vector3 DefenderSpawnPoint;
        #endregion

        #region Private Fields
        private Client.UI.MatchHudUi Hud;
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            Debug.Log("[ClientPhaseTesting] Starting client phase testing scene");
            yield return Ind3x.Util.FirebaseInstance.Instance.Initialize("PHASE_TESTING", false);

            GlobalState.StateMachine = new Ind3x.State.StateMachine();
            GlobalState.Prefabs = FindAnyObjectByType<PrefabList>();
            GlobalState.Map = FindAnyObjectByType<DueloMap>();
            GlobalState.Kernel = new MatchKernel();
            GlobalState.Camera = FindAnyObjectByType<DueloCamera>();
            GlobalState.Input = new UnityEngine.InputSystem.DueloInput();
            GlobalState.Input.Enable();

            UniTask.Delay(1)
                .ContinueWith(LoadMatchData)
                .ContinueWith(SimulatePlayMatchScreen)
                .ContinueWith(StartPhase);
        }
        #endregion

        #region Loading
        private async UniTask LoadMatchData()
        {
            Debug.Log("[ClientPhaseTesting] Loading db data");

            GlobalState.StartupOptions = new StartupOptions(StartupMode.Client, new string[] {
                "--playerId", MatchDto.Players.Challenger.UnityPlayerId
            });

            var player = await DeviceService.Instance.GetDevicePlayer();
            Debug.Log($"[ClientPhaseTesting] Player data fetched: \nplayerId: {player.UnityPlayerId}\nunityPlayerId: {player.UnityPlayerId}");
            GlobalState.PlayerData = player;

            DueloMapDto mapDto = await MapService.Instance.GetMap(MatchDto.MapId);
            GlobalState.Map.Load(mapDto);
            GlobalState.Camera.SetMapCenter(GlobalState.Map.MapCenter);

            GlobalState.ClientMatch = new ClientMatch(MatchDto);
            GlobalState.ClientMatch.LoadAssets();

            GlobalState.Camera.FollowPlayers(GlobalState.ClientMatch.Players);
            GlobalState.Kernel.RegisterEntities(GlobalState.ClientMatch.Players.Values.ToArray());
        }
        #endregion

        #region Phases
        private void SimulatePlayMatchScreen()
        {
            var camera = GlobalState.Camera.GetComponentInChildren<Camera>();

            // TODO: This will break once the UI rendering is changed to world space
            var ui = GameObject.Instantiate(GlobalState.Prefabs.MenuLookup[Duelo.Common.Util.UIViewPrefab.MatchHud]);
            ui.transform.SetParent(camera.transform, false);

            Hud = ui.GetComponent<Client.UI.MatchHudUi>();
        }

        private void StartPhase()
        {
            switch (MatchDto.SyncState.Server)
            {
                case MatchState.ChooseMovement:
                    GlobalState.StateMachine.PushState(new Client.Screen.ChooseMovementPhase());
                    break;
                case MatchState.ChooseAction:
                    GlobalState.StateMachine.PushState(new Client.Screen.ChooseActionPhase());
                    break;
            }

            Hud.TxtMatchState.text = MatchDto.SyncState.Server.ToString();
            Hud.TxtRoundNumber.text = GlobalState.ClientMatch.CurrentRound.RoundNumber.ToString();
        }
        #endregion
    }
}