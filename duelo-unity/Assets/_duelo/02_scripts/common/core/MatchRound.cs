namespace Duelo.Common.Core
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
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
        /// listener to the movement node.
        /// </summary>
        /// <param name="callback"></param>
        public void OnMovement(Action<MovementPhaseDto> callback)
        {
            _onMovementReceived += callback;
            MovementRef.ValueChanged += MovementValueChanged;

            var update = new Dictionary<string, object>
            {
                { "timer", TimeAllowed },
                { "defender", null },
                { "challenger", null }
            };

            MovementRef.UpdateChildrenAsync(update);
        }

        public void OffMovement(Action<MovementPhaseDto> callback)
        {
            _onMovementReceived -= callback;
            MovementRef.ValueChanged -= MovementValueChanged;
        }

        public void MovementValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.Snapshot.Exists)
            {
                var defenderValue = e.Snapshot.Child("defender/position").Value;
                var challengerValue = e.Snapshot.Child("challenger/position").Value;

                if (defenderValue != null && challengerValue != null)
                {
                    var json = e.Snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<MovementPhaseDto>(json);
                        _onMovementReceived?.Invoke(data);
                    }
                }
            }
        }
        #endregion

        #region Action Methods
        public void OnActions(Action<ActionPhaseDto> onActionsReceived)
        {
            _onActionReceived += onActionsReceived;
            ActionRef.ValueChanged += ActionValueChanged;

            var update = new Dictionary<string, object>
            {
                { "challenger", null },
                { "defender", null }
            };

            ActionRef.UpdateChildrenAsync(update);
        }

        public void OffActions(Action<ActionPhaseDto> onActionsReceived)
        {
            _onActionReceived -= onActionsReceived;
            ActionRef.ValueChanged -= ActionValueChanged;
        }

        public void ActionValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (e.Snapshot.Exists)
            {
                var defenderValue = e.Snapshot.Child("defender/actionId").Value;
                var challengerValue = e.Snapshot.Child("challenger/actionId").Value;

                if (defenderValue != null && challengerValue != null)
                {
                    var json = e.Snapshot.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var data = JsonConvert.DeserializeObject<ActionPhaseDto>(json);
                        _onActionReceived?.Invoke(data);
                    }
                }
            }
        }
        #endregion
    }
}