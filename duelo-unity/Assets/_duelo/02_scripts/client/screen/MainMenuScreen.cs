namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Util;
    using Ind3x.State;
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

        #region Private Fields
        private GameObject _backgroundWorld;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MainMenuScreen] OnEnter");
            UiElements = SpawnUI<MainMenu>(UIViewPrefab.MainMenu);

            _backgroundWorld = GameObject.Instantiate(UiElements.BackgroundWorldPrefab);
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            GameObject.Destroy(_backgroundWorld);
            return null;
        }
        #endregion

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == UiElements.BtnMatchMaking.gameObject)
            {
                StateMachine.SwapState(new MatchMakingScreen());
            }
            else if (source == UiElements.BtnProfiles.gameObject)
            {
                StateMachine.SwapState(new ProfilesScreen());
            }
        }
        #endregion
    }
}