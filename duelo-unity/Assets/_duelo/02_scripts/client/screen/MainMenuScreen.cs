namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Core;
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
        public MainMenu View;
        #endregion

        #region Private Fields
        private GameObject _backgroundWorld;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MainMenuScreen] OnEnter");
            View = SpawnUI<MainMenu>(UIViewPrefab.MainMenu);

            _backgroundWorld = GameObject.Instantiate(View.BackgroundWorldPrefab);

            View.TextPlayerId.text = GameData.PlayerData.PlayerId;
            View.TextGamertag.text = GameData.ActiveProfile?.Gamertag ?? "No Profile Selected";
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
            if (source == View.BtnMatchMaking.gameObject)
            {
                StateMachine.SwapState(new MatchMakingScreen());
            }
            else if (source == View.BtnProfiles.gameObject)
            {
                StateMachine.SwapState(new ProfilesScreen());
            }
        }
        #endregion
    }
}