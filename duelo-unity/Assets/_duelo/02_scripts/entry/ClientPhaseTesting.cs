namespace Duelo
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Client.Screen;
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
        #endregion

        #region Private Fields
        private IClientMatch _match => GlobalState.Match as IClientMatch;

        private MockService _services;
        private PlayMatchScreen _playMatchScreen;
        #endregion

        #region Unity Lifecycle
        public void Start()
        {
            _services = new MockService(MatchDto);

            GlobalState.StartupOptions = new StartupOptions(StartupMode.Client, new string[] { });
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

            var player = await _services.GetDevicePlayer();
            Debug.Log($"[ClientPhaseTesting] Player data fetched: \nplayerId: {player.UnityPlayerId}\nunityPlayerId: {player.UnityPlayerId}");
            GlobalState.PlayerData = player;

            DueloMapDto mapDto = await _services.GetMap(MatchDto.MapId);
            GlobalState.Map.Load(mapDto);
            GlobalState.Camera.SetMapCenter(GlobalState.Map.MapCenter);

            GlobalState.Match = new MockMatch(MatchDto);

            _match.LoadAssets();

            GlobalState.Camera.FollowPlayers(GlobalState.Match.Players.ToDictionary(x => x.Key, kvp => kvp.Value));
            GlobalState.Kernel.RegisterEntities(GlobalState.Match.Players[PlayerRole.Defender], GlobalState.Match.Players[PlayerRole.Challenger]);
        }
        #endregion

        #region Phases
        private void SimulatePlayMatchScreen()
        {
            var camera = GlobalState.Camera.GetComponentInChildren<Camera>();

            // TODO: This will break once the UI rendering is changed to world space
            var ui = GameObject.Instantiate(GlobalState.Prefabs.MenuLookup[Duelo.Common.Util.UIViewPrefab.MatchHud]);
            ui.transform.SetParent(camera.transform, false);

            _playMatchScreen = new Client.Screen.PlayMatchScreen();
            _playMatchScreen.Hud = ui.GetComponent<Client.UI.MatchHudUi>();

            _playMatchScreen.SetPlayerHealthbarInfo(GlobalState.Match.Players[PlayerRole.Challenger], _playMatchScreen.Hud.ChallengerHealthBar);
            _playMatchScreen.SetPlayerHealthbarInfo(GlobalState.Match.Players[PlayerRole.Defender], _playMatchScreen.Hud.DefenderHealthBar);
        }

        private void StartPhase()
        {
            switch (MatchDto.SyncState.Server)
            {
                case MatchState.ChooseMovement:
                    GlobalState.StateMachine.PushState(new Client.Screen.ChooseMovementPhase());
                    break;
                case MatchState.ChooseAction:
                    SetMovementsForPlayers();
                    GlobalState.StateMachine.PushState(new Client.Screen.ChooseActionPhase());
                    break;
            }

            _playMatchScreen.UpdateHudUi(GlobalState.Match.CurrentRound.CurrentValue);
            _playMatchScreen.UpdatePlayerHealthBars(GlobalState.Match.CurrentRound.CurrentValue);
        }
        #endregion

        #region Helpers
        private void SetMovementsForPlayers()
        {
            var movePhase = new Client.Screen.ChooseMovementPhase();

            void PaintMovements(PlayerRole role, PlayerRoundMovementDto move)
            {
                var player = GlobalState.Match.Players[role];
                movePhase.SelectMovement(player, move.ActionId, move.TargetPosition);
            }

            var round = GlobalState.Match.CurrentRound.CurrentValue;
            PaintMovements(PlayerRole.Challenger, round.PlayerMovement[PlayerRole.Challenger]);
            PaintMovements(PlayerRole.Defender, round.PlayerMovement[PlayerRole.Defender]);
        }
        #endregion
    }
}