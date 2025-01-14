namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Screen that allows the player to manage their profiles.
    /// Uses <see cref="UI.ProfilesUi"/> view.
    /// </summary>
    public class ProfilesScreen : GameScreen
    {
        #region Components
        public ProfilesUi View { get; private set; }
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[ProfilesScreen] OnEnter");
            View = SpawnUI<ProfilesUi>(UIViewPrefab.Profiles);

            if (GameData.PlayerData.Profiles?.Count > 1)
            {
                View.BtnChangeProfile.Disabled = false;
            }
            else
            {
                View.BtnChangeProfile.Disabled = true;
            }

            View.LabelGamertag.text = GameData.PlayerData.ActiveProfile?.Gamertag ?? "No Profile Selected";
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
            else if (source == View.BtnChangeProfile.gameObject)
            {
                StateMachine.SwapState(new SelectProfileScreen());
            }
            else if (source == View.BtnCreateProfile.gameObject)
            {
                StateMachine.SwapState(new CreateProfileScreen());
            }
        }
        #endregion
    }
}