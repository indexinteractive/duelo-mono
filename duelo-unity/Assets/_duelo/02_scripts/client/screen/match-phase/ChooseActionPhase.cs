namespace Duelo.Client.Screen
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Player;
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
            PopulateAttackPanel(GlobalState.ClientMatch.DevicePlayer.Traits.Attacks);

            _ui.CountdownTimer.StartTimer(GlobalState.ClientMatch.CurrentRound.Action.Timer);
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
        private void PopulateAttackPanel(IEnumerable<PlayerActionItemDto> actions)
        {
            foreach (var action in actions)
            {
                var instance = GameObject.Instantiate(_ui.PanelItemPrefab, _ui.AttackPanelGrid.transform);
                var panelItem = instance.GetComponent<UI.UiActionPanelItem>();
                panelItem.SetAction(action);
            }
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
                Debug.Log($"[ChooseActionPhase] Selected action: {actionInfo.Action.ActionId}");
                GlobalState.ClientMatch.DispatchAttack((int)actionInfo.Action.ActionId);
            }
        }
        #endregion
    }
}