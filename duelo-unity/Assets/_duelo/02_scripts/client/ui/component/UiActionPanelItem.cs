namespace Duelo.Client.UI
{
    using Duelo.Common.Kernel;
    using UnityEngine;

    public class UiActionPanelItem : UiButton
    {
        #region Ui Elements
        [SerializeField]
        private UnityEngine.UI.Text _textId;

        [SerializeField]
        private UnityEngine.UI.Image _icon;
        #endregion

        #region Properties
        public string IdText
        {
            get => _textId.text;
            set => _textId.text = value;
        }

        public ActionDescriptor Action { get; private set; }
        #endregion

        #region Public Methods
        public void SetAction(ActionDescriptor action)
        {
            IdText = ((int)action.ActionId).ToString();
            _icon.sprite = action.Icon;
            Action = action;
        }
        #endregion
    }
}