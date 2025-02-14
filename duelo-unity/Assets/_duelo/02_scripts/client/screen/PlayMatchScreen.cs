namespace Duelo.Client.Screen
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.UI;
    using Duelo.Common.Match;
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
        public MatchHudUi Hud;
        public readonly MatchDto _initialMatchDto;
        #endregion

        #region Initialization
        public PlayMatchScreen(MatchDto match)
        {
            _initialMatchDto = match;
        }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[PlayMatchScreen] OnEnter");

            /// Will be unloaded when the match has been joined in <see cref="OnMatchStateChange"/>
            StateMachine.PushState(new LoadingPopup());

            _match.JoinMatch().ContinueWith(() => _match.OnStateChange += OnMatchStateChange);
        }

        public override StateExitValue OnExit()
        {
            _match.OnStateChange -= OnMatchStateChange;

            DestroyUI();
            return null;
        }
        #endregion

        #region Match Events
        private void OnMatchStateChange(MatchDto newState, MatchDto previousState)
        {
            if (!(StateMachine.CurrentState is PlayMatchScreen))
            {
                StateMachine.PopState();
            }

            if (newState.SyncState.Server == MatchState.Initialize || MatchDto.IsMatchLoopState(newState.SyncState.Server))
            {
                if (Hud == null)
                {
                    Hud = SpawnUI<MatchHudUi>(UIViewPrefab.MatchHud);
                    UpdateHudUi(_match.CurrentDto);

                    SetPlayerHealthbarInfo(_match.Players[PlayerRole.Challenger], Hud.ChallengerHealthBar);
                    SetPlayerHealthbarInfo(_match.Players[PlayerRole.Defender], Hud.DefenderHealthBar);
                }
            }

            if (newState.SyncState.Server == MatchState.ChooseMovement)
            {
                StateMachine.PushState(new ChooseMovementPhase());
            }

            if (newState.SyncState.Server == MatchState.ChooseAction)
            {
                StateMachine.PushState(new ChooseActionPhase());
            }

            if (newState.SyncState.Server == MatchState.ExecuteRound)
            {
                StateMachine.PushState(new ExecuteRoundPhase());
            }

            if (MatchDto.IsMatchLoopState(newState.SyncState.Server))
            {
                UpdateHudUi(newState);
                UpdatePlayerHealthBars();
            }
        }

        #endregion

        #region Ui
        public void UpdateHudUi(MatchDto match)
        {
            Hud.TxtMatchState.text = match.SyncState.Server.ToString();
            Hud.TxtRoundNumber.text = match.Rounds.Count().ToString();
        }

        public void SetPlayerHealthbarInfo(MatchPlayer player, PlayerStatusBar healthBar)
        {
            healthBar.SetPlayerInfo(player.ProfileDto.Gamertag, player.Traits);
        }

        public void UpdatePlayerHealthBars()
        {
            // TODO: use health from round schema
            Hud.ChallengerHealthBar.UpdateHealth(3);
            Hud.DefenderHealthBar.UpdateHealth(3);
        }
        #endregion
    }
}