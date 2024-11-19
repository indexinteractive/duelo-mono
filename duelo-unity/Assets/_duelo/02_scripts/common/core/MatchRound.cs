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
                Movement = Movement
            };
        }
        #endregion

        #region Firebase
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
    }
}