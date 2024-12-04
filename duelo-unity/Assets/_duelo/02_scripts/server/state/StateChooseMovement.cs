namespace Duelo.Server.State
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Util;
    using UnityEngine;

    public class StateChooseMovement : ServerMatchState
    {
        #region Private Fields
        private const string DISPLAY_FORMAT = "00.00";
        private Countdown _countdown;
        private float _lastLoggedTime = -1f;
        private Dictionary<PlayerRole, PlayerRoundMovementDto> _playerMovements = new();
        #endregion

        #region GameState Implementation
        public override void OnEnter()
        {
            Debug.Log("StateChooseMovement");

            Match.CurrentRound.KickoffMovement(OnMovementReceived).ContinueWith(() =>
            {
                _countdown = new Countdown();
                _countdown.OnCountdownUpdated += OnCountdownUpdated;
                _countdown.OnCountdownFinished += OnCountdownFinished;
                _countdown.StartTimer(Match.Clock.CurrentTimeAllowedMs);
            });
        }

        public override void Update()
        {
            _countdown?.Update();
        }
        #endregion

        #region Events
        private void OnMovementReceived(MovementPhaseDto movement)
        {
            if (movement?.Challenger?.ActionId != null)
            {
                Debug.Log($"[StateChooseMovement] Received movement from challenger: " + movement.Challenger.ActionId.ToString());
                _playerMovements[PlayerRole.Challenger] = movement.Challenger;
            }

            if (movement?.Defender?.ActionId != null)
            {
                Debug.Log($"[StateChooseMovement] Received movement from defender: " + movement.Defender.ActionId.ToString());
                _playerMovements[PlayerRole.Defender] = movement.Defender;
            }
        }

        private void OnCountdownUpdated(float timeLeft)
        {
            if (Mathf.FloorToInt(timeLeft) != Mathf.FloorToInt(_lastLoggedTime))
            {
                _lastLoggedTime = timeLeft;
                Debug.Log("[StateChooseMovement] Time left: " + timeLeft.ToString(DISPLAY_FORMAT));
            }
        }

        private void OnCountdownFinished()
        {
            Match.CurrentRound.EndMovement(OnMovementReceived);

            foreach (var movement in _playerMovements)
            {
                var args = new object[] { movement.Value.TargetPosition };
                Kernel.QueuePlayerAction(movement.Key, movement.Value.ActionId, args);
            }

            StateMachine.SwapState(new StateChooseAction());
        }
        #endregion
    }
}