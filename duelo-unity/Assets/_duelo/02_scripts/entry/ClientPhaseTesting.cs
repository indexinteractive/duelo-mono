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

            GameData.StateMachine = new Ind3x.State.StateMachine();
            GameData.Prefabs = FindAnyObjectByType<PrefabList>();
            GameData.Map = FindAnyObjectByType<DueloMap>();
            GameData.Kernel = new MatchKernel();
            GameData.Camera = FindAnyObjectByType<DueloCamera>();

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

            GameData.StartupOptions = new StartupOptions(StartupMode.Client, new string[] {
                "--playerId", MatchDto.Players.Challenger.UnityPlayerId
            });

            var player = await DeviceService.Instance.GetDevicePlayer();
            Debug.Log($"[ClientPhaseTesting] Player data fetched: \nplayerId: {player.UnityPlayerId}\nunityPlayerId: {player.UnityPlayerId}");
            GameData.PlayerData = player;

            DueloMapDto mapDto = await MapService.Instance.GetMap(MatchDto.MapId);
            GameData.Map.Load(mapDto);
            GameData.Camera.SetMapCenter(GameData.Map.MapCenter);

            GameData.ClientMatch = new ClientMatch(MatchDto);
            GameData.ClientMatch.LoadAssets();

            GameData.Camera.FollowPlayers(GameData.ClientMatch.Players);
            GameData.Kernel.RegisterEntities(GameData.ClientMatch.Players.Values.ToArray());
        }
        #endregion

        #region Phases
        private void SimulatePlayMatchScreen()
        {
            var camera = GameData.Camera.GetComponentInChildren<Camera>();

            // TODO: This will break once the UI rendering is changed to world space
            var ui = GameObject.Instantiate(GameData.Prefabs.MenuLookup[Duelo.Common.Util.UIViewPrefab.MatchHud]);
            ui.transform.SetParent(camera.transform, false);

            Hud = ui.GetComponent<Client.UI.MatchHudUi>();
        }

        private void StartPhase()
        {
            switch (MatchDto.SyncState.Server)
            {
                case MatchState.ChooseMovement:
                    GameData.StateMachine.PushState(new Client.Screen.ChooseMovementPartial());
                    break;
            }

            Hud.TxtMatchState.text = MatchDto.SyncState.Server.ToString();
            Hud.TxtRoundNumber.text = GameData.ClientMatch.CurrentRound.RoundNumber.ToString();
        }
        #endregion
    }
}