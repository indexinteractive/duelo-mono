namespace Duelo.Client.UI
{
    /// <summary>
    /// Partial view GUI for <see cref="Duelo.Client.Screen.ChooseActionView". Loaded when
    /// the match reaches <see cref="Duelo.Common.Model.MatchState.ChooseAction"/>
    /// Loaded on top of the <see cref="MatchHud"/>.
    /// </summary>
    public class ChooseActionPartial : BaseView
    {
        public Util.UiTimer CountdownTimer;
    }
}