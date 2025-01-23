namespace Duelo.Client.Screen
{
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    public class ChooseMovementPartial : GameScreen
    {
        #region Ui Elements
        private UI.ChooseMovementUi _ui;
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[ChooseMovementPartial] OnEnter");
            _ui = SpawnUI<UI.ChooseMovementUi>(UIViewPrefab.ChooseMovementPartial);

            _ui.CountdownTimer.StartTimer(GameData.ClientMatch.CurrentRound.Movement.Timer);
            _ui.CountdownTimer.TimerElapsed += OnTimerElapsed;

            // TODO: There should be a default movement id set by a player traits
            var player = GameData.ClientMatch.DevicePlayer;

            var descriptor = ActionFactory.Instance.GetDescriptor(MovementActionId.Walk);
            var tiles = descriptor.GetMovableTiles(player.Traits, player.Position);
            GameData.Map.PaintMovableTiles(tiles);
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