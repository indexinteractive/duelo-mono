namespace Duelo.Client.Screen
{
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Match;
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
        private ActionDescriptor _selectedMovement;
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

            PopulateMovementChoices(_player.Traits.Movements);

            _ui.CountdownTimer.StartTimer(Match.CurrentRound.CurrentValue.MovementTimer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;

            _tileLayerMask = LayerMask.GetMask(Layers.TileMap);

            // TODO: There should be a default movement id set by a player traits
            ChangeMovementType(_player.Traits.Movements.FirstOrDefault());

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
        private void ChangeMovementType(ActionDescriptor descriptor)
        {
            if (descriptor != null)
            {
                _selectedMovement = descriptor;
                var positions = descriptor.GetMovablePositions(_player.Traits, _player.Position);

                GlobalState.Map.SetMovableTiles(positions);
                GlobalState.Map.ClearMovableTiles();
                GlobalState.Map.PaintMovableTiles(positions);
            }
        }
        #endregion

        #region Ui
        private void PopulateMovementChoices(IEnumerable<ActionDescriptor> movements)
        {
            foreach (var movement in movements)
            {
                var instance = GameObject.Instantiate(_ui.PanelItemPrefab, _ui.SpeedChoiceGrid.transform);
                var panelItem = instance.GetComponent<UI.UiActionPanelItem>();
                panelItem.SetAction(movement);
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
                        SelectMovement(_player, _selectedMovement.ActionId, targetPosition);
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
            var actionInfo = source.GetComponent<UI.UiActionPanelItem>();
            if (actionInfo != null)
            {
                Debug.Log($"[ChooseMovementPhase] Selected movement: {actionInfo.Action.ActionId}");
                ChangeMovementType(actionInfo.Action);
            }
        }
        #endregion

        #region Helpers
        public void SelectMovement(MatchPlayer player, int actionId, Vector3 targetPosition)
        {
            Debug.Log($"[ChooseMovementPhase] Selected movement: {actionId}");

            GlobalState.Map.PaintPath(player.Role, player.Position, targetPosition);
            Client.DispatchMovement(actionId, targetPosition);

            player.SetGhost(targetPosition);
        }
        #endregion
    }
}