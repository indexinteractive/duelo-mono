namespace Duelo.Client.Screen
{
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;

    /// <summary>
    /// This screen will allow the match to proceed by displaying the correct UI panels
    /// for the player to interact with at each phase of the match.
    /// Entities will be updated during the execution phase according to the server data.
    /// </summary>
    public class PlayMatchScreen : GameScreen
    {
        #region Private Fields
        private DatabaseReference _ref;
        #endregion

        #region Initialization
        public PlayMatchScreen(MatchDto match)
        {
            _ref = FirebaseDatabase.DefaultInstance.GetReference(DueloCollection.Match.ToString()).Child(match.MatchId);
        }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
        }

        public override void HandleUIEvent(UnityEngine.GameObject source, object eventData)
        {
        }
        #endregion
    }
}