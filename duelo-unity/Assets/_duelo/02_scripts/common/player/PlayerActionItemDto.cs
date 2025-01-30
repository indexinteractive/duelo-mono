namespace Duelo.Common.Player
{
    using System;
    using Duelo.Common.Model;

    // TODO: Can this be a scriptable object?
    [Serializable]
    public class PlayerActionItemDto
    {
        #region Ui Properties
        public string Name;
        public string Icon;
        #endregion

        #region Action Properties
        /// <summary>
        /// The action id that this item represents.
        /// Will be used to later instantiate the corresponding <see cref="Common.Kernel.ActionDescriptor"/>.
        /// </summary>
        public ActionDropdownItem ActionId;
        #endregion
    }
}