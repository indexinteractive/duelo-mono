namespace Duelo.Client.UI
{
    /// <summary>
    /// UI Elements that belong to <see cref="Duelo.Client.Screen.CreateProfileScreen"/>
    /// </summary>
    public class ProfileCreate : BaseView
    {
        public UiButton BtnBack;
        public UiButton BtnNext;
        public UiButton BtnPreviousCharacter;
        public UiButton BtnNextCharacter;
        public UnityEngine.Transform CharacterSpawnPoint;
        public UnityEngine.UI.Text LabelCharacterName;
        public UnityEngine.UI.Text LabelTraitStrength;
        public UnityEngine.UI.Text LabelTraitSpeed;
        public UnityEngine.UI.Text LabelTraitMoveRange;
        public UnityEngine.UI.Text LabelPerkName;
        public UnityEngine.UI.Text LabelPerkDescription;
    }
}