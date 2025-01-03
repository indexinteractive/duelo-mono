namespace Duelo.Client.Screen
{
    using Duelo.Client.UI;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Screen shown during profile creation that allows a player to give their profile a unique name.
    /// Uses <see cref="UI.ChooseGamertag"/> view.
    /// </summary>
    public class ChooseGamertagScreen : GameScreen
    {
        #region Components
        public ChooseGamertag View { get; private set; }
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[ChooseGamertagScreen] OnEnter");
            View = SpawnUI<ChooseGamertag>(UIViewPrefab.ChooseGamertag);
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
                StateMachine.SwapState(new CreateProfileScreen());
            }
            else if (source == View.BtnFinish.gameObject)
            {
                Debug.Log("FINISH PROFILE CREATE");
            }
        }
        #endregion
    }
}