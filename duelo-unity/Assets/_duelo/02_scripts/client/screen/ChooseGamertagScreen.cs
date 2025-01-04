namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Player;
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Screen shown during profile creation that allows a player to give their profile a unique name.
    /// Uses <see cref="UI.ChooseGamertag"/> view.
    /// </summary>
    public class ChooseGamertagScreen : GameScreen
    {
        #region Fields
        public ChooseGamertag View { get; private set; }
        private readonly PlayerTraits _selectedCharacter;
        #endregion

        #region Initialization
        public ChooseGamertagScreen(PlayerTraits traits)
        {
            _selectedCharacter = traits;
        }

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
                string gamertag = View.InputGamertag.text;

                if (!string.IsNullOrWhiteSpace(gamertag) && _selectedCharacter.CharacterId != null)
                {
                    DeviceService.Instance.CreateProfile(GameData.PlayerData.PlayerId, gamertag, _selectedCharacter.CharacterId)
                        .ContinueWith(async (PlayerProfileDto result) =>
                        {
                            if (result != null)
                            {
                                GameData.PlayerData = await DeviceService.Instance.GetDevicePlayer();
                                if (GameData.ActiveProfile == null)
                                {
                                    GameData.ActiveProfile = result;
                                }
                            }
                            else
                            {
                                // TODO: Show error message
                            }
                        })
                        .ContinueWith(() =>
                        {
                            StateMachine.SwapState(new ProfilesScreen());
                        });
                }
                else
                {
                    Debug.Log("[ProfilesScreen] No gamertag specified to create a profile");
                }
            }
        }
        #endregion
    }
}