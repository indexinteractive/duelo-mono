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
            StateMachine.PushState(new LoadingPopup());
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
            if (newState.State == MatchState.Initialize || MatchDto.IsMatchLoopState(newState.State))
            {
                if (StateMachine.CurrentState is LoadingPopup)
                {
                    StateMachine.PopState();
                }

                if (Hud == null)
                {
                    Hud = SpawnUI<MatchHud>(UIViewPrefab.MatchHud);
                    UpdateUiValues(GameData.ClientMatch.CurrentDto);
                }
            }

            if (MatchDto.IsMatchLoopState(newState.State))
            {
                UpdateUiValues(newState);
            }
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
                case MatchState.ChooseMovement:
                    Hud.CountdownTimer.StartTimer(currentRound.Movement.Timer);
                    Hud.CountdownTimer.gameObject.SetActive(true);
                    break;
                case MatchState.ChooseAction:
                    Hud.CountdownTimer.StartTimer(currentRound.Action.Timer);
                    Hud.CountdownTimer.gameObject.SetActive(true);
                    break;
                default:
                    Hud.CountdownTimer.StopTimer();
                    Hud.CountdownTimer.gameObject.SetActive(false);
                    break;
            }
        }
        #endregion
    }
}