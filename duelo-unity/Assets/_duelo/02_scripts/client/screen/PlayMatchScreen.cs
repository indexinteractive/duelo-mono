namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Ind3x.State;
    using R3;
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
        private readonly CompositeDisposable _dataSubs = new();
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            Debug.Log("[PlayMatchScreen] OnEnter");

            /// Will be unloaded when the match has been joined in <see cref="OnMatchStateChange"/>
            StateMachine.PushState(new LoadingPopup());

            Client.JoinMatch().ContinueWith(() =>
            {
                Match.SyncState.Server
                    .Subscribe(OnMatchStateChange)
                    .AddTo(_dataSubs);
                Match.CurrentRound
                    .WhereNotNull()
                    .Subscribe(UpdateHudUi)
                    .AddTo(_dataSubs);
                Match.CurrentRound
                    .WhereNotNull()
                    .Subscribe(UpdatePlayerHealthBars)
                    .AddTo(_dataSubs);
            });
        }

        public override StateExitValue OnExit()
        {
            _dataSubs.Dispose();

            DestroyUI();
            return null;
        }
        #endregion

        #region Match Events
        private void OnMatchStateChange(MatchState newState)
        {
            Debug.Log($"[PlayMatchScreen] MatchState.{newState}");

            if (newState == MatchState.Initialize || MatchDto.IsMatchLoopState(newState))
            {
                if (!(StateMachine.CurrentState is PlayMatchScreen))
                {
                    StateMachine.PopState();
                }

                if (Hud == null)
                {
                    Hud = SpawnUI<MatchHudUi>(UIViewPrefab.MatchHud);

                    SetPlayerHealthbarInfo(Match.Players[PlayerRole.Challenger], Hud.ChallengerHealthBar);
                    SetPlayerHealthbarInfo(Match.Players[PlayerRole.Defender], Hud.DefenderHealthBar);
                }
            }

            if (newState == MatchState.ChooseMovement)
            {
                StateMachine.PushState(new ChooseMovementPhase());
            }

            if (newState == MatchState.ChooseAction)
            {
                StateMachine.PushState(new ChooseActionPhase());
            }

            if (newState == MatchState.ExecuteRound)
            {
                StateMachine.PushState(new ExecuteRoundPhase());
            }

            if (Hud != null)
            {
                Hud.TxtMatchState.text = newState.ToString();
            }
        }
        #endregion

        #region Ui
        public void UpdateHudUi(MatchRound round)
        {
            if (Hud != null)
            {
                Hud.TxtRoundNumber.text = round.RoundNumber.ToString();
            }
        }

        public void SetPlayerHealthbarInfo(MatchPlayer player, PlayerStatusBar healthBar)
        {
            Debug.Log($"[PlayMatchScreen] Setting health bar info for {player.ProfileDto.Gamertag}");
            healthBar.SetPlayerInfo(player.ProfileDto.Gamertag, player.Traits);
        }

        public void UpdatePlayerHealthBars(MatchRound round)
        {
            if (Hud != null)
            {
                Hud.ChallengerHealthBar.UpdateHealth(round.PlayerState[PlayerRole.Challenger].Health.CurrentValue);
                Hud.DefenderHealthBar.UpdateHealth(round.PlayerState[PlayerRole.Challenger].Health.CurrentValue);
            }
        }
        #endregion
    }
}