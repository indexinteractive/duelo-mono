namespace Duelo.Client.UI
{
    using UnityEngine;

    public class UiActionPanelItem : UiButton
    {
        #region Ui Elements
        [SerializeField]
        private UnityEngine.UI.Text _textId;
        #endregion

        #region Properties
        public string IdText
        {
            get => _textId.text;
            set => _textId.text = value;
        }
        #endregion
    }
}