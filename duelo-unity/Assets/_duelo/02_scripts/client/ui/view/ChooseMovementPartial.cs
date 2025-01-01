namespace Duelo.Client.UI
{
    /// <summary>
    /// Partial view GUI for <see cref="Duelo.Client.Screen.ChooseMovementView". Loaded when
    /// the match reaches <see cref="Duelo.Common.Model.MatchState.ChooseMovement"/>
    /// Loaded on top of the <see cref="MatchHud"/>.
    /// </summary>
    public class ChooseMovementPartial : BaseView
    {
        public Util.UiTimer CountdownTimer;

        public UnityEngine.UI.Button BtnSpeedX1;
        public UnityEngine.UI.Button BtnSpeedX2;
    }
}