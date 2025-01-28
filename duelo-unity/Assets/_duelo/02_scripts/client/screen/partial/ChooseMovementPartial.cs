namespace Duelo.Client.Screen
{
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class ChooseMovementPartial : GameScreen
    {
        #region Public Properties
        private float _raycastDistance = 50f;
        private LayerMask _tileLayerMask;
        #endregion

        #region Ui Elements
        private UI.ChooseMovementUi _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseMovementPartial] OnEnter");
            _ui = SpawnUI<UI.ChooseMovementUi>(UIViewPrefab.ChooseMovementPartial);

            _ui.CountdownTimer.StartTimer(GlobalState.ClientMatch.CurrentRound.Movement.Timer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;

            _tileLayerMask = LayerMask.GetMask(Layers.TileMap);

            // TODO: There should be a default movement id set by a player traits
            var player = GlobalState.ClientMatch.DevicePlayer;

            var descriptor = ActionFactory.Instance.GetDescriptor(MovementActionId.Walk);
            var tiles = descriptor.GetMovableTiles(player.Traits, player.Position);
            GlobalState.Map.PaintMovableTiles(tiles);

            GlobalState.Input.Player.Fire.performed += OnTapPerformed;
        }

        public override StateExitValue OnExit()
        {
            _ui.CountdownTimer.TimerElapsed = null;
            GlobalState.Input.Player.Fire.performed -= OnTapPerformed;
            DestroyUI();

            return null;
        }
        #endregion

        #region Timer Events
        private void OnTimerElapsed()
        {
            StateMachine.PopState();
        }
        #endregion

        #region Input
        private void OnTapPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            Vector2 position = Pointer.current.position.ReadValue();
            Debug.Log($"[ChooseMovementPartial] OnTapPerformed: {position}");

            // if (PositionIsUIElement(position))
            // {
            //     return;
            // }

            var camera = GlobalState.Camera.RootCamera;
            Ray ray = camera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _tileLayerMask))
            {
                if (hit.collider.TryGetComponent(out MapTile tile))
                {
                    Debug.Log($"[ChooseMovementPartial] Tile hit: {tile.name}");
                    var devicePosition = GlobalState.ClientMatch.DevicePlayer.Position;
                    GlobalState.Map.PaintPath(devicePosition, tile.transform.position);
                }
            }
        }

        #endregion
    }
}