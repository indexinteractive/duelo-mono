namespace Duelo.Client.Screen
{
    using System.Linq;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Initial screen in the profile creation flow. Allows the player to choose from
    /// a selection of different characters.
    /// Uses <see cref="UI.ProfileCreateUi"/> view.
    /// </summary>
    public class CreateProfileScreen : GameScreen
    {
        #region UI
        public ProfileCreateUi View { get; private set; }
        #endregion

        #region Private Fields
        private int _currentUnitIndex = -1;
        private GameObject _characterInstance;

        private GameObject[] _availableCharacters => GlobalState.Prefabs.CharacterLookup.Values.ToArray();
        #endregion

        #region Initialization
        public override void OnEnter()
        {
            Debug.Log("[CreateProfileScreen] OnEnter");
            View = SpawnUI<ProfileCreateUi>(UIViewPrefab.ProfileCreate);

            _currentUnitIndex = 0;
            UpdateUi(_availableCharacters[_currentUnitIndex]);
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

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            if (source == View.BtnBack.gameObject)
            {
                StateMachine.SwapState(new ProfilesScreen());
            }
            else if (source == View.BtnNext.gameObject)
            {
                var character = _availableCharacters[_currentUnitIndex];
                var traits = character.GetComponent<Common.Player.PlayerTraits>();
                StateMachine.SwapState(new ChooseGamertagScreen(traits));
            }
            else if (source == View.BtnNextCharacter.gameObject)
            {
                _currentUnitIndex++;
                if (_currentUnitIndex >= _availableCharacters.Length)
                {
                    _currentUnitIndex = 0;
                }
            }
            else if (source == View.BtnPreviousCharacter.gameObject)
            {
                _currentUnitIndex--;
                if (_currentUnitIndex < 0)
                {
                    _currentUnitIndex = _availableCharacters.Length - 1;
                }
            }

            UpdateUi(_availableCharacters[_currentUnitIndex]);
        }
        #endregion

        #region Private Helpers
        private void InstantiateCharacter(GameObject prefab)
        {
            // TODO: Until the ui is implemented in world space, we can't spawn the character in the scene
            if (_characterInstance != null)
            {
                GameObject.Destroy(_characterInstance.gameObject);
            }

            _characterInstance = GameObject.Instantiate(prefab, View.CharacterSpawnPoint);
        }

        private void UpdateUi(GameObject character)
        {
            InstantiateCharacter(character);
            var traits = character.GetComponent<Common.Player.PlayerTraits>();

            View.LabelCharacterName.text = traits.CharacterName;
            View.LabelTraitStrength.text = traits.BaseStrength.ToString();
            View.LabelTraitSpeed.text = traits.BaseSpeed.ToString();
            View.LabelTraitMoveRange.text = traits.BaseMovementRange.ToString();
            View.LabelPerkName.text = traits.Perk.Name;
            View.LabelPerkDescription.text = traits.Perk.Description;
        }
        #endregion
    }
}