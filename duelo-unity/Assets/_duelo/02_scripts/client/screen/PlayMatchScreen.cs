namespace Duelo.Client.Screen
{
    using System.Linq;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// This screen will allow the match to proceed by displaying the correct UI panels
    /// for the player to interact with at each phase of the match.
    /// Entities will be updated during the execution phase according to the server data.
    /// </summary>
    public class PlayMatchScreen : GameScreen
    {
        #region Private Fields
        public MatchHud Hud;
        #endregion

        #region Initialization
        public PlayMatchScreen(MatchDto match)
        {
            GameData.ClientMatch.OnStateChange += OnMatchStateChange;
        }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[PlayMatchScreen] OnEnter");
            Hud = SpawnUI<MatchHud>(UIViewPrefab.MatchHud);
            UpdateUiValues(GameData.ClientMatch.CurrentDto);
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return null;
        }
        #endregion

        #region Match Events
        private void OnMatchStateChange(MatchDto newState, MatchDto previousState)
        {
            UpdateUiValues(newState);
        }
        #endregion

        #region Ui
        private void UpdateUiValues(MatchDto match)
        {
            MatchRoundDto currentRound = match.Rounds.Last();

            Hud.TxtMatchState.text = match.State.ToString();
            Hud.TxtRoundNumber.text = currentRound.RoundNumber.ToString();

            switch (match.State)
            {
                // TODO: Implement timer class
                case MatchState.ChooseMovement:
                    Hud.TxtTimerClock.text = currentRound.Movement.Timer.ToString();
                    break;
                case MatchState.ChooseAction:
                    Hud.TxtTimerClock.text = currentRound.Action.Timer.ToString();
                    break;
                default:
                    Hud.TxtTimerClock.text = "00.00";
                    break;
            }
        }
        #endregion
    }
}