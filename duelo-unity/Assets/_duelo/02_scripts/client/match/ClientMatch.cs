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
        private MatchDto _previousDto;
        private DatabaseReference _ref;
        #endregion

        #region Actions
        public Action<MatchState, MatchState> OnStateChange;
        #endregion

        #region Initialization
        public ClientMatch(MatchDto match)
        {
            _previousDto = match;
            _ref = FirebaseDatabase.DefaultInstance.GetReference(DueloCollection.Match.ToString().ToLower()).Child(match.MatchId);

            _ref.ValueChanged += OnMatchUpdate;
        }
        #endregion

        #region Data Update
        private void OnMatchUpdate(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.DatabaseError != null)
            {
                Console.WriteLine(eventArgs.DatabaseError.Message);
                return;
            }

            try
            {
                string jsonValue = eventArgs.Snapshot.GetRawJsonValue();
                MatchDto update = JsonConvert.DeserializeObject<MatchDto>(jsonValue);

                if (update.State != _previousDto.State)
                {
                    Console.WriteLine("Match state changed: " + update.State);
                    OnStateChange?.Invoke(update.State, _previousDto.State);
                }

                _previousDto = update;
            }
            catch (System.Exception error)
            {
                Debug.Log("Error: " + error.Message);
            }
        }
        #endregion
    }
}