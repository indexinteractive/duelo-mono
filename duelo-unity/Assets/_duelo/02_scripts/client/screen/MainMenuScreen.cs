namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Main Menu Screen state. Uses <see cref="UI.MainMenuUi"/> as the main UI.
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        #region Public Properties
        public MainMenuUi View;
        #endregion

        #region Private Fields
        private GameObject _backgroundWorld;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MainMenuScreen] OnEnter");
            View = SpawnUI<MainMenuUi>(UIViewPrefab.MainMenu);

            _backgroundWorld = GameObject.Instantiate(View.BackgroundWorldPrefab);

            View.TextUnityPlayerId.text = GlobalState.PlayerData.UnityPlayerId;
            View.TextGamertag.text = GlobalState.PlayerData.ActiveProfile?.Gamertag ?? "No Profile Selected";
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
#if DUELO_LOCAL
                StateMachine.SwapState(new DebugMatchScreen());
#else
                StateMachine.SwapState(new MatchmakingScreen());
#endif
            }
            else if (source == View.BtnProfiles.gameObject)
            {
                StateMachine.SwapState(new ProfilesScreen());
            }
        }
        #endregion
    }
}