namespace Duelo.Client.UI
{
    using Duelo.Common.Player;
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

        public PlayerActionItemDto Action { get; private set; }
        #endregion

        #region Public Methods
        public void SetAction(PlayerActionItemDto action)
        {
            IdText = ((int)action.ActionId).ToString();
            Action = action;
        }
        #endregion
    }
}