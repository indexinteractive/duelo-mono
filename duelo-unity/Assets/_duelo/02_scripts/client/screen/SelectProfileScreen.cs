namespace Duelo.Client.Screen
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Screen that allows the player to choose a profile from a list of existing profiles.
    /// Uses <see cref="UI.ProfileSelectUi"/> view.
    /// </summary>
    public class SelectProfileScreen : GameScreen
    {
        #region Fields
        public ProfileSelectUi View { get; private set; }
        private int _currentProfileIndex = -1;
        private GameObject _characterInstance;

        private PlayerProfileDto[] _availableProfiles => GameData.PlayerData.Profiles.Values.ToArray();
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[SelectProfileScreen] OnEnter");
            View = SpawnUI<ProfileSelectUi>(UIViewPrefab.ProfileSelect);

            _currentProfileIndex = 0;
            UpdateUi(_availableProfiles[_currentProfileIndex]);

            if (GameData.PlayerData.Profiles.Count > 1)
            {
                View.BtnNextProfile.enabled = true;
                View.BtnPreviousProfile.enabled = true;
                View.BtnSelectProfile.enabled = true;
            }
            else if (GameData.PlayerData.ActiveProfile == null)
            {
                View.BtnSelectProfile.enabled = true;
            }
            else
            {
                // View._btnCreateProfile.enabled = true;
                View.BtnNextProfile.enabled = false;
                View.BtnPreviousProfile.enabled = false;
            }
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();

            if (_characterInstance != null)
            {
                GameObject.Destroy(_characterInstance.gameObject);
            }

            return null;
        }
        #endregion

        #region Private Helpers
        private void InstantiateCharacter(PlayerProfileDto profile)
        {
            // TODO: Until the ui is implemented in world space, we can't spawn the character in the scene
            if (_characterInstance != null)
            {
                GameObject.Destroy(_characterInstance.gameObject);
            }

            var characterPrefab = GameData.Prefabs.CharacterLookup[profile.CharacterUnitId];
            _characterInstance = GameObject.Instantiate(characterPrefab, View.CharacterSpawnPoint);
        }

        private void UpdateUi(PlayerProfileDto profile)
        {
            InstantiateCharacter(profile);
            var traits = _characterInstance.GetComponent<Common.Player.PlayerTraits>();

            View.LabelGamertag.text = profile.Gamertag;
            View.LabelCharacterName.text = traits.CharacterName;
            View.LabelTraitStrength.text = traits.BaseStrength.ToString();
            View.LabelTraitSpeed.text = traits.BaseSpeed.ToString();
            View.LabelTraitMoveRange.text = traits.BaseMovementRange.ToString();
            View.LabelPerkName.text = traits.Perk.Name;
            View.LabelPerkDescription.text = traits.Perk.Description;
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
                var profile = _availableProfiles[_currentProfileIndex];
                DeviceService.Instance.SetActiveProfile(GameData.PlayerData.PlayerId, profile.Id)
                    .ContinueWith(async () =>
                    {
                        GameData.PlayerData = await DeviceService.Instance.GetDevicePlayer();
                        StateMachine.SwapState(new MainMenuScreen());
                    });
            }
            else if (source == View.BtnPreviousProfile.gameObject)
            {
                _currentProfileIndex--;
                if (_currentProfileIndex < 0)
                {
                    _currentProfileIndex = _availableProfiles.Length - 1;
                }
            }
            else if (source == View.BtnNextProfile.gameObject)
            {
                _currentProfileIndex++;
                if (_currentProfileIndex >= _availableProfiles.Length)
                {
                    _currentProfileIndex = 0;
                }
            }

            UpdateUi(_availableProfiles[_currentProfileIndex]);
        }
        #endregion
    }
}