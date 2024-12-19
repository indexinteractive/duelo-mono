namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Util;
    using UnityEngine;

    /// <summary>
    /// Main Menu Screen state. Uses <see cref="UI.MainMenu"/> as the main UI.
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        #region Public Properties
        public GameObject StartButton;
        public MainMenu UiElements;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MainMenuScreen] OnEnter");
            UiElements = SpawnUI<MainMenu>(UIMenuPrefab.MainMenu);
        }
        #endregion

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            Debug.Log("[MainMenuScreen] HandleUIEvent");
            Debug.Log("input text" + UiElements.InputGameId?.text);
        }
        #endregion
    }
}