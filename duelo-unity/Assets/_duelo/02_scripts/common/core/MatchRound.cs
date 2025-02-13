namespace Duelo.Common.Core
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using Firebase.Database;
    using Newtonsoft.Json;
    using UnityEngine;

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
        public MatchRound(int roundNumber, uint timeAllowed, DatabaseReference matchRef)
        {
            RoundNumber = roundNumber;
            TimeAllowed = timeAllowed;
            StartTime = DateTime.UtcNow;

            if (matchRef != null)
            {
                _roundRef = matchRef.Child("rounds").Child(RoundNumber.ToString());
            }
        }
        #endregion

        #region Movement Methods
        /// <summary>
        /// Kicks off the movement phase by setting the timer and adds a
        /// callback to the movement node listener.
        /// </summary>
        public UniTask KickoffMovement(Action<MovementPhaseDto> callback)
        {
            MovementRef.ValueChanged += MovementValueChanged;
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
            Debug.Log($"[MatchRound] MovementValueChanged: {e.Snapshot.GetRawJsonValue()}");
            if (_onMovementReceived != null && e.Snapshot.Exists)
            {
                var json = e.Snapshot.GetRawJsonValue();
                try
                {
                    var data = JsonConvert.DeserializeObject<MovementPhaseDto>(json);

                    if (data.Challenger != null || data.Defender != null)
                    {
                        if (!string.IsNullOrEmpty(json))
                        {
                            PlayerMovement = data;
                            _onMovementReceived.Invoke(data);
                        }
                    }
                }
                catch
                {
                    Debug.LogError($"[MatchRound] Error parsing movement data: {json}");
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
            ActionRef.ValueChanged += ActionValueChanged;
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
                var json = e.Snapshot.GetRawJsonValue();
                try
                {
                    var data = JsonConvert.DeserializeObject<ActionPhaseDto>(json);

                    if (data.Challenger != null || data.Defender != null)
                    {
                        if (!string.IsNullOrEmpty(json))
                        {
                            PlayerAction = data;
                            _onActionReceived.Invoke(data);
                        }
                    }
                }
                catch
                {
                    Debug.LogError($"[MatchRound] Error parsing action data: {json}");
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
            if (_roundRef == null)
            {
                return;
            }

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