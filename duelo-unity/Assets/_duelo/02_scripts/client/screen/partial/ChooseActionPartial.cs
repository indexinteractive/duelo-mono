namespace Duelo.Client.Screen
{
    using Duelo.Common.Core;
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

            _ui.CountdownTimer.StartTimer(GameData.ClientMatch.CurrentRound.Action.Timer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;
        }

        public override StateExitValue OnExit()
        {
            _ui.CountdownTimer.TimerElapsed = null;
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
    }
}