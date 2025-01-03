namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Screen that allows the player to choose a profile from a list of existing profiles.
    /// Uses <see cref="UI.ProfileSelect"/> view.
    /// </summary>
    public class SelectProfileScreen : GameScreen
    {
        #region Components
        public ProfileSelect View { get; private set; }
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[SelectProfileScreen] OnEnter");
            View = SpawnUI<ProfileSelect>(UIViewPrefab.ProfileSelect);
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return null;
        }
        #endregion

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == View.BtnBack.gameObject)
            {
                StateMachine.SwapState(new MainMenuScreen());
            }
            else if (source == View.BtnSelectProfile.gameObject)
            {
                Debug.Log("SELECT PROFILE");
            }
            else if (source == View.BtnPreviousProfile)
            {
                Debug.Log("PREVIOUS PROFILE");
            }
            else if (source == View.BtnNextProfile)
            {
                Debug.Log("NEXT PROFILE");
            }
        }
        #endregion
    }
}