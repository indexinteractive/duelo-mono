namespace Duelo.Server.State
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Util;
    using Ind3x.State;
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
            Match.CurrentRound.KickoffMovement(OnMovementReceived)
                .ContinueWith(() => Match.WaitForSyncState(MatchState.ChooseMovement))
                .ContinueWith(() =>
                {
                    _countdown = new Countdown();
                    _countdown.OnCountdownUpdated += OnCountdownUpdated;
                    _countdown.OnCountdownFinished += OnCountdownFinished;
                    _countdown.StartTimer(Match.Clock.CurrentTimeAllowedMs);
                });
        }

        public override StateExitValue OnExit()
        {
            _countdown.OnCountdownFinished -= OnCountdownFinished;
            _countdown.OnCountdownUpdated -= OnCountdownUpdated;

            return null;
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

                var origin = Match.Players[PlayerRole.Challenger].transform.position;
                Map.PaintPath(origin, movement.Challenger.TargetPosition);
            }

            if (movement?.Defender?.ActionId != null)
            {
                Debug.Log($"[StateChooseMovement] Received movement from defender: " + movement.Defender.ActionId.ToString());
                _playerMovements[PlayerRole.Defender] = movement.Defender;

                var origin = Match.Players[PlayerRole.Defender].transform.position;
                Map.PaintPath(origin, movement.Defender.TargetPosition);
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
            Match.CurrentRound.EndMovement();
            StateMachine.SwapState(new StateChooseAction());
        }
        #endregion
    }
}