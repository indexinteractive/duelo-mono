namespace Duelo.Client.Screen
{
    using System;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    public class ChooseMovementView : GameScreen
    {
        #region Ui Elements
        private ChooseMovementPartial _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseMovementPartial] OnEnter");
            _ui = SpawnUI<ChooseMovementPartial>(UIViewPrefab.ChooseMovementPartial);

            _ui.CountdownTimer.StartTimer(GameData.ClientMatch.CurrentRound.Movement.Timer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;
        }

        public override StateExitValue OnExit()
        {
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