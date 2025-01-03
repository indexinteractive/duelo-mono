namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Initial screen in the profile creation flow. Allows the player to choose from
    /// a selection of different characters.
    /// Uses <see cref="UI.ProfileCreate"/> view.
    /// </summary>
    public class CreateProfileScreen : GameScreen
    {
        #region Components
        public ProfileCreate View { get; private set; }
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[CreateProfileScreen] OnEnter");
            View = SpawnUI<ProfileCreate>(UIViewPrefab.ProfileCreate);
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
                StateMachine.SwapState(new ProfilesScreen());
            }
            else if (source == View.BtnNext.gameObject)
            {
                StateMachine.SwapState(new ChooseGamertagScreen());
            }
            else if (source == View.BtnNextCharacter.gameObject)
            {
                Debug.Log("CHARACTER RIGHT");
            }
            else if (source == View.BtnPreviousCharacter.gameObject)
            {
                Debug.Log("CHARACTER LEFT");
            }
        }
        #endregion
    }
}