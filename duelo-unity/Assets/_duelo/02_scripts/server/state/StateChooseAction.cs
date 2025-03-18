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

    public class StateChooseAction : ServerMatchState
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
            Debug.Log("StateChooseAction");
            Server.KickoffActionsPhase()
                .ContinueWith(() =>
                {
                    Match.CurrentRound.CurrentValue.PlayerAction.ObserveChanged()
                        .Subscribe(ActionValueChanged)
                        .AddTo(_subscriptions);
                })
                .ContinueWith(() => Server.WaitForSyncState(MatchState.ChooseAction))
                .ContinueWith(() =>
                {
                    _countdown = new Countdown();
                    _countdown.OnCountdownUpdated += OnCountdownUpdated;
                    _countdown.OnCountdownFinished += OnCountdownFinished;
                    _countdown.StartTimer(Match.CurrentRound.CurrentValue.ActionTimer);
                });
        }

        public override StateExitValue OnExit()
        {
            if (_countdown != null)
            {
                _countdown.OnCountdownFinished -= OnCountdownFinished;
                _countdown.OnCountdownUpdated -= OnCountdownUpdated;
            }

            return null;
        }

        public override void Update()
        {
            _countdown?.Update();
        }
        #endregion

        #region Events
        private void ActionValueChanged(CollectionChangedEvent<KeyValuePair<PlayerRole, PlayerRoundActionDto>> update)
        {
            var playerRole = update.NewItem.Key;
            if (playerRole != PlayerRole.Unknown)
            {
                var dto = update.NewItem.Value;
                Debug.Log($"[StateChooseAction] ActionValueChanged: {playerRole} - {dto}");
            }
        }

        private void OnCountdownUpdated(float timeLeft)
        {
            if (Mathf.FloorToInt(timeLeft) != Mathf.FloorToInt(_lastLoggedTime))
            {
                _lastLoggedTime = timeLeft;
                Debug.Log("[StateChooseAction] Time left: " + timeLeft.ToString(DISPLAY_FORMAT));
            }
        }

        private void OnCountdownFinished()
        {
            Server.EndActionsPhase();
            StateMachine.SwapState(new StateLateActions());
        }
        #endregion
    }
}