namespace Duelo.Client.Screen
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Player;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    public class ChooseActionPartial : GameScreen
    {
        #region Ui Elements
        private UI.ChooseActionUi _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseActionPartial] OnEnter");
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
            var panelItems = _ui.gameObject.GetComponentsInChildren<UI.UiActionPanelItem>();

            int index = 0;
            foreach (var action in actions)
            {
                panelItems[index++].IdText = ((int)action.ActionId).ToString();
            }
        }
        #endregion

        #region Timer Events
        private void OnTimerElapsed()
        {
            StateMachine.PopState();
        }
        #endregion
    }
}