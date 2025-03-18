namespace Duelo.Client.Screen
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    public class ChooseActionPhase : GameScreen
    {
        #region Ui Elements
        private UI.ChooseActionUi _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseActionPhase] OnEnter");
            _ui = SpawnUI<UI.ChooseActionUi>(UIViewPrefab.ChooseActionPartial);

            PopulateAttackPanel(_player.Traits.Attacks);
            PopulateDefensePanel(_player.Traits.Defenses);

            _ui.CountdownTimer.StartTimer(Match.CurrentRound.CurrentValue.ActionTimer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;
        }

        public override StateExitValue OnExit()
        {
            _ui.CountdownTimer.TimerElapsed = null;
            DestroyUI();
            return null;
        }
        #endregion

        #region Ui
        private void AdjustPanelHeightToRows(GameObject panelGrid, int actionsCount)
        {
            var gridLayoutGroup = panelGrid.GetComponent<UnityEngine.UI.GridLayoutGroup>();
            int columnCount = gridLayoutGroup.constraintCount;
            int rowCount = Mathf.CeilToInt(actionsCount / (float)columnCount);

            float itemHeight = gridLayoutGroup.cellSize.y;
            float spacing = gridLayoutGroup.spacing.y;
            float padding = gridLayoutGroup.padding.top + gridLayoutGroup.padding.bottom;

            RectTransform parentRectTransform = panelGrid.transform.parent.GetComponent<RectTransform>();
            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, rowCount * (itemHeight + spacing) - spacing + padding);
        }

        private void PopulateAttackPanel(IEnumerable<ActionDescriptor> actions)
        {
            int actionsCount = 0;
            foreach (var action in actions)
            {
                var instance = GameObject.Instantiate(_ui.PanelItemPrefab, _ui.AttackPanelGrid.transform);
                var panelItem = instance.GetComponent<UI.UiActionPanelItem>();
                panelItem.SetAction(action);
                actionsCount++;
            }

            AdjustPanelHeightToRows(_ui.AttackPanelGrid, actionsCount);
        }

        private void PopulateDefensePanel(IEnumerable<ActionDescriptor> defenses)
        {
            int defensesCount = 0;

            foreach (var defense in defenses)
            {
                var instance = GameObject.Instantiate(_ui.PanelItemPrefab, _ui.DefensePanelGrid.transform);
                var panelItem = instance.GetComponent<UI.UiActionPanelItem>();
                panelItem.SetAction(defense);
                defensesCount++;
            }

            AdjustPanelHeightToRows(_ui.DefensePanelGrid, defensesCount);
        }
        #endregion

        #region Timer Events
        private void OnTimerElapsed()
        {
            StateMachine.PopState();
        }
        #endregion

        #region Buttons
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            var actionInfo = source.GetComponent<UI.UiActionPanelItem>();
            if (actionInfo != null)
            {
                SelectAttack(actionInfo.Action);
            }
        }
        #endregion

        #region Helpers
        public void SelectAttack(ActionDescriptor action)
        {
            Debug.Log($"[ChooseActionPhase] Selected action: {action.ActionId}");

            var targetPosition = _player.Role == Common.Model.PlayerRole.Challenger
                ? Match.CurrentRound.CurrentValue.PlayerMovement[PlayerRole.Challenger].TargetPosition
                : Match.CurrentRound.CurrentValue.PlayerMovement[PlayerRole.Defender].TargetPosition;

            var attackTiles = action.GetAttackRangeTiles(_player.Traits, targetPosition);
            GlobalState.Map.ClearActionTiles();
            GlobalState.Map.PaintActionTiles(attackTiles);

            Client.DispatchAttack((int)action.ActionId);
        }
        #endregion
    }
}