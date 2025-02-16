namespace Duelo.Client.Screen
{
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    public class ChooseMovementPhase : GameScreen
    {
        #region Private Fields
        private readonly float _raycastDistance = 50f;
        private int _selectedMovementId;
        private LayerMask _tileLayerMask;
        #endregion

        #region Ui Elements
        private UI.ChooseMovementUi _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseMovementPhase] OnEnter");
            _ui = SpawnUI<UI.ChooseMovementUi>(UIViewPrefab.ChooseMovementPartial);

            _ui.CountdownTimer.StartTimer(_match.CurrentRound.Movement.Timer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;

            _tileLayerMask = LayerMask.GetMask(Layers.TileMap);

            // TODO: There should be a default movement id set by a player traits
            ChangeMovementType(MovementActionId.Walk);

            GlobalState.Input.Player.Fire.performed += OnTapPerformed;
        }

        public override StateExitValue OnExit()
        {
            _ui.CountdownTimer.TimerElapsed = null;
            GlobalState.Map.ClearMovableTiles();
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

        #region Map Decorator
        private void ChangeMovementType(int newMovementId)
        {
            _selectedMovementId = newMovementId;

            var descriptor = _player.Traits.Movements.FirstOrDefault(x => (int)x.ActionId == _selectedMovementId);
            if (descriptor != null)
            {
                var positions = descriptor.GetMovablePositions(_player.Traits, _player.Position);

                GlobalState.Map.SetMovableTiles(positions);
                GlobalState.Map.ClearMovableTiles();
                GlobalState.Map.PaintMovableTiles(positions);
            }
        }
        #endregion

        #region Input
        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            Vector2 position = Pointer.current.position.ReadValue();

            if (PositionIsUIElement(position))
            {
                Debug.Log("[ChooseMovementPhase] Raycast hit UI element");
                return;
            }

            var camera = GlobalState.Camera.RootCamera;
            Ray ray = camera.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastDistance, _tileLayerMask))
            {
                if (hit.collider.TryGetComponent(out MapTile tile))
                {
                    if (tile.IsMovable)
                    {
                        Vector3 targetPosition = tile.transform.position;
                        GlobalState.Map.PaintPath(_player.Role, _player.Position, targetPosition);
                        _match.DispatchMovement(_selectedMovementId, targetPosition);

                        _player.SetGhost(targetPosition);
                    }
                }
            }
        }

        private bool PositionIsUIElement(Vector2 position)
        {
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = position;
            var raycastResults = new List<RaycastResult>();

            // Note: wehn using VisualElement, element must have `picking mode` set to `position` to be found
            EventSystem.current.RaycastAll(pointer, raycastResults);

            if (raycastResults.Count > 0)
            {
                foreach (RaycastResult result in raycastResults)
                {
                    if (result.distance == 0 && result.isValid)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == _ui.BtnSpeedX1.gameObject)
            {
                Debug.Log($"[ChooseMovementPhase] Speed selected: {MovementActionId.Walk}");
                ChangeMovementType(MovementActionId.Walk);
            }
            else if (source == _ui.BtnSpeedX2.gameObject)
            {
                Debug.Log($"[ChooseMovementPhase] Speed selected: {MovementActionId.Run}");
                ChangeMovementType(MovementActionId.Run);
            }
        }
        #endregion
    }
}