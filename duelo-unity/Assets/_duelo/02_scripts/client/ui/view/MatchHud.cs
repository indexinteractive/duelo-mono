namespace Duelo.Client.UI
{
    /// <summary>
    /// Gameplay HUD that is spawned by <see cref="Screen.PlayMatchScreen"/>
    /// </summary>
    public class MatchHud : BaseView
    {
        #region Debugging
        public UnityEngine.UI.Text TxtMatchState;
        #endregion

        #region Round Panel
        public UnityEngine.UI.Text TxtRoundNumber;
        #endregion
    }
}