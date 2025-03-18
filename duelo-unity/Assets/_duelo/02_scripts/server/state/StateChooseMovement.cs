namespace Duelo.Server.State
{
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Util;
    using Ind3x.State;
    using ObservableCollections;
    using R3;
    using UnityEngine;

    public class StateChooseMovement : ServerMatchState
    {
        #region Private Fields
        private const string DISPLAY_FORMAT = "00.00";
        private Countdown _countdown;
        private float _lastLoggedTime = -1f;
        private readonly CompositeDisposable _subscriptions = new();
        #endregion

        #region GameState Implementation
        public override void OnEnter()
        {
            Debug.Log("StateChooseMovement");
            Server.KickoffMovementPhase()
                .ContinueWith(() =>
                {
                    Match.CurrentRound.CurrentValue.PlayerMovement.ObserveChanged()
                        .Subscribe(MovementValueChanged)
                        .AddTo(_subscriptions);
                })
                .ContinueWith(() => Server.WaitForSyncState(MatchState.ChooseMovement))
                .ContinueWith(() =>
                {
                    _countdown = new Countdown();
                    _countdown.OnCountdownUpdated += OnCountdownUpdated;
                    _countdown.OnCountdownFinished += OnCountdownFinished;
                    _countdown.StartTimer(Match.CurrentRound.CurrentValue.MovementTimer);
                });
        }

        public override StateExitValue OnExit()
        {
            if (_countdown != null)
            {
                _countdown.OnCountdownFinished -= OnCountdownFinished;
                _countdown.OnCountdownUpdated -= OnCountdownUpdated;
            }

            _subscriptions.Dispose();

            return null;
        }

        public override void Update()
        {
            _countdown?.Update();
        }
        #endregion

        #region Events
        public void MovementValueChanged(CollectionChangedEvent<KeyValuePair<PlayerRole, PlayerRoundMovementDto>> update)
        {
            var playerRole = update.NewItem.Key;
            if (playerRole != PlayerRole.Unknown)
            {
                var actionId = update.NewItem.Value.ActionId;
                var targetPosition = update.NewItem.Value.TargetPosition;
                Debug.Log($"[StateChooseMovement] Received movement from {playerRole}: {actionId} - {targetPosition}");

                var origin = Match.Players[playerRole].transform.position;
                Map.PaintPath(playerRole, origin, targetPosition);
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
            Server.EndMovementPhase();
            StateMachine.SwapState(new StateChooseAction());
        }
        #endregion
    }
}