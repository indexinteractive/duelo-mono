namespace Duelo.Common.Core
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using Firebase.Database;
    using Newtonsoft.Json;

    public class MatchRound
    {
        #region Public Properties
        public readonly int RoundNumber;
        public readonly uint TimeAllowed;
        public readonly DateTime StartTime;
        #endregion

        #region Firebase
        private readonly DatabaseReference _roundRef;
        #endregion

        #region Movement Phase
        public DatabaseReference MovementRef => _roundRef.Child("movement");
        /// <summary>
        /// Movement data received from the players
        /// </summary>
        public MovementPhaseDto PlayerMovement;
        private Action<MovementPhaseDto> _onMovementReceived;
        #endregion

        #region Action Phase
        public DatabaseReference ActionRef => _roundRef.Child("action");
        /// <summary>
        /// Action data received from the players
        /// </summary>
        public ActionPhaseDto PlayerAction;
        private Action<ActionPhaseDto> _onActionReceived;
        #endregion

        #region Initialization
        public MatchRound(ServerMatch match)
        {
            RoundNumber = match.Clock.CurrentRound;
            TimeAllowed = match.Clock.CurrentTimeAllowedMs;
            StartTime = DateTime.UtcNow;

            _roundRef = match.MatchRef.Child("rounds").Child(RoundNumber.ToString());

            MovementRef.ValueChanged += MovementValueChanged;
            ActionRef.ValueChanged += ActionValueChanged;
        }
        #endregion

        #region Movement Methods
        /// <summary>
        /// Kicks off the movement phase by setting the timer and adds a
        /// callback to the movement node listener.
        /// </summary>
        public UniTask KickoffMovement(Action<MovementPhaseDto> callback)
        {
            _onMovementReceived = callback;

            PlayerMovement = new MovementPhaseDto()
            {
                Timer = TimeAllowed
            };

            var update = JsonConvert.SerializeObject(PlayerMovement);
            return MovementRef.SetRawJsonValueAsync(update).AsUniTask();
        }

        public void EndMovement()
        {
            _onMovementReceived = null;
        }

        public void MovementValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_onMovementReceived != null && e.Snapshot.Exists)
            {
                var defenderValue = e.Snapshot.Child("defender/actionId").Value;
                var challengerValue = e.Snapshot.Child("challenger/actionId").Value;

                if (defenderValue != null || challengerValue != null)
                {
                    var json = e.Snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<MovementPhaseDto>(json);
                        PlayerMovement = data;
                        _onMovementReceived.Invoke(data);
                    }
                }
            }
        }
        #endregion

        #region Action Methods
        /// <summary>
        /// Kicks off the action phase by setting the timer and adds a
        /// callback to the action node listener.
        /// </summary>
        public UniTask KickoffActions(Action<ActionPhaseDto> onActionsReceived)
        {
            _onActionReceived += onActionsReceived;

            PlayerAction = new ActionPhaseDto()
            {
                Timer = TimeAllowed
            };

            var update = JsonConvert.SerializeObject(PlayerAction);
            return ActionRef.SetRawJsonValueAsync(update).AsUniTask();
        }

        public void EndActions(Action<ActionPhaseDto> onActionsReceived)
        {
            _onActionReceived -= onActionsReceived;
        }

        public void ActionValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_onActionReceived != null && e.Snapshot.Exists)
            {
                var defenderValue = e.Snapshot.Child("defender/actionId").Value;
                var challengerValue = e.Snapshot.Child("challenger/actionId").Value;

                if (defenderValue != null && challengerValue != null)
                {
                    var json = e.Snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<ActionPhaseDto>(json);
                        PlayerAction = data;
                        _onActionReceived.Invoke(data);
                    }
                }
            }
        }

        /// <summary>
        /// Called when the round ends to clean up the listeners in <see cref="ServerMatch.NewRound"/>
        /// </summary>
        public void End()
        {
            MovementRef.ValueChanged -= MovementValueChanged;
            ActionRef.ValueChanged -= ActionValueChanged;
        }

        public async UniTask Publish()
        {
            var data = new MatchRoundDto()
            {
                RoundNumber = RoundNumber,
                Action = null,
                Movement = null,
            };

            var json = JsonConvert.SerializeObject(data);
            await _roundRef.SetRawJsonValueAsync(json);
        }
        #endregion
    }
}