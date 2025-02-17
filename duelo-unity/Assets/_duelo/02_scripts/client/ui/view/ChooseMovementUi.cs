namespace Duelo.Client.UI
{
    /// <summary>
    /// Partial view GUI for <see cref="Duelo.Client.Screen.ChooseMovementPhase". Loaded when
    /// the match reaches <see cref="Duelo.Common.Model.MatchState.ChooseMovement"/>
    /// Loaded on top of the <see cref="MatchHud"/>.
    /// </summary>
    public class ChooseMovementUi : BaseUi
    {
        public Util.UiTimer CountdownTimer;
        public UnityEngine.GameObject PanelItemPrefab;
        public UnityEngine.GameObject SpeedChoiceGrid;
    }
}