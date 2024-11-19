namespace Duelo.Common.Core
{
    using System;
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
        public MovementPhaseDto Movement;
        private Action<MovementPhaseDto> _onMovementReceived;
        #endregion

        #region Action Phase
        public DatabaseReference ActionRef => _roundRef.Child("action");
        public ActionPhaseDto Action;
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

        #region Serialization
        public MatchRoundDto ToDto()
        {
            return new MatchRoundDto()
            {
                RoundNumber = RoundNumber,
                Movement = Movement,
                Action = Action
            };
        }
        #endregion

        #region Movement Methods
        /// <summary>
        /// Kicks off the movement phase by setting the timer and adds a
        /// callback to the movement node listener.
        /// </summary>
        public void OnMovement(Action<MovementPhaseDto> callback)
        {
            _onMovementReceived += callback;

            var update = JsonConvert.SerializeObject(new { timer = TimeAllowed });
            MovementRef.SetRawJsonValueAsync(update);
        }

        public void OffMovement(Action<MovementPhaseDto> callback)
        {
            _onMovementReceived -= callback;
        }

        public void MovementValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (_onMovementReceived != null && e.Snapshot.Exists)
            {
                var defenderValue = e.Snapshot.Child("defender/position").Value;
                var challengerValue = e.Snapshot.Child("challenger/position").Value;

                if (defenderValue != null && challengerValue != null)
                {
                    var json = e.Snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<MovementPhaseDto>(json);
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
        public void OnActions(Action<ActionPhaseDto> onActionsReceived)
        {
            _onActionReceived += onActionsReceived;

            var update = JsonConvert.SerializeObject(new { timer = TimeAllowed });
            ActionRef.SetRawJsonValueAsync(update);
        }

        public void OffActions(Action<ActionPhaseDto> onActionsReceived)
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
                        _onActionReceived.Invoke(data);
                    }
                }
            }
        }
        #endregion
    }
}