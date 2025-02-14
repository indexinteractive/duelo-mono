namespace Duelo.Client.UI
{
    /// <summary>
    /// Gameplay HUD that is spawned by <see cref="Screen.PlayMatchScreen"/>
    /// </summary>
    public class MatchHudUi : BaseUi
    {
        #region Debugging
        public UnityEngine.UI.Text TxtMatchState;
        #endregion

        #region Round Panel
        public UnityEngine.UI.Text TxtRoundNumber;
        #endregion

        #region Player Panels
        public PlayerStatusBar ChallengerHealthBar;
        public PlayerStatusBar DefenderHealthBar;
        #endregion
    }
}