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
        public readonly MatchDto _initialMatchDto;
        #endregion

        #region Initialization
        public PlayMatchScreen(MatchDto match)
        {
            _initialMatchDto = match;
            GameData.ClientMatch.OnStateChange += OnMatchStateChange;
        }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[PlayMatchScreen] OnEnter");
            StateMachine.PushState(new LoadingPopup());
            OnMatchStateChange(_initialMatchDto, null);
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
                    UpdateHudUi(GameData.ClientMatch.CurrentDto);
                }
            }

            if (newState.State == MatchState.ChooseMovement)
            {
                StateMachine.PushState(new ChooseMovementView());
            }

            if (newState.State == MatchState.ChooseAction)
            {
                StateMachine.PushState(new ChooseActionView());
            }

            if (MatchDto.IsMatchLoopState(newState.State))
            {
                UpdateHudUi(newState);
            }
        }
        #endregion

        #region Ui
        private void UpdateHudUi(MatchDto match)
        {
            MatchRoundDto currentRound = match.Rounds.Last();

            Hud.TxtMatchState.text = match.State.ToString();
            Hud.TxtRoundNumber.text = currentRound.RoundNumber.ToString();
        }
        #endregion
    }
}