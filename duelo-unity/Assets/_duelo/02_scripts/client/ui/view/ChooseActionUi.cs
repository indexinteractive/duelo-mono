namespace Duelo.Client.UI
{
    /// <summary>
    /// Partial view GUI for <see cref="Duelo.Client.Screen.ChooseActionPhase". Loaded when
    /// the match reaches <see cref="Duelo.Common.Model.MatchState.ChooseAction"/>
    /// Loaded on top of the <see cref="MatchHud"/>.
    /// </summary>
    public class ChooseActionUi : BaseUi
    {
        public Util.UiTimer CountdownTimer;
        public UnityEngine.GameObject PanelItemPrefab;
        public UnityEngine.GameObject AttackPanelGrid;
        public UnityEngine.GameObject DefensePanelGrid;
    }
}