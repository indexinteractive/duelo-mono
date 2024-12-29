namespace Duelo.Client.Match
{
    using System;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using Newtonsoft.Json;
    using UnityEngine;

    public class ClientMatch
    {
        #region Private Fields
        private DatabaseReference _ref;
        #endregion

        #region Public Properties
        public MatchDto CurrentDto { get; private set; }
        #endregion

        #region Actions
        /// <summary>
        /// Event that is triggered when the match state changes.
        /// The first parameter is the new state, the second is the previous state.
        /// </summary>
        public Action<MatchDto, MatchDto> OnStateChange;
        #endregion

        #region Initialization
        public ClientMatch(MatchDto match)
        {
            CurrentDto = match;
            _ref = FirebaseDatabase.DefaultInstance.GetReference(DueloCollection.Match.ToString().ToLower()).Child(match.MatchId);

            _ref.ValueChanged += OnMatchUpdate;
        }
        #endregion

        #region Data Update
        private void OnMatchUpdate(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.DatabaseError != null)
            {
                Debug.Log(eventArgs.DatabaseError.Message);
                return;
            }

            try
            {
                string jsonValue = eventArgs.Snapshot.GetRawJsonValue();

                MatchDto previousDto = CurrentDto;
                CurrentDto = JsonConvert.DeserializeObject<MatchDto>(jsonValue);

                if (previousDto.State != CurrentDto.State)
                {
                    OnStateChange?.Invoke(CurrentDto, previousDto);
                }
            }
            catch (System.Exception error)
            {
                Debug.Log("Error: " + error.Message);
            }
        }
        #endregion
    }
}