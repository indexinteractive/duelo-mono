namespace Duelo.Client.Screen
{
    using Duelo.Client.Match;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Main Menu Screen state. Uses <see cref="UI.MainMenu"/> as the main UI.
    /// </summary>
    public class MainMenuScreen : GameScreen
    {
        #region Public Properties
        public GameObject StartButton;
        public MainMenu UiElements;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MainMenuScreen] OnEnter");
            UiElements = SpawnUI<MainMenu>(UIViewPrefab.MainMenu);
        }

        public override void Resume(StateExitValue results)
        {
            Debug.Log("[MainMenuScreen] Resume");
            var data = results.data as LoadingPopup<MatchDto>.LoadResult;

            if (data.Result.MatchId != null)
            {
                Debug.Log("[MainMenuScreen] Match found: " + data.Result.MatchId);
                // TODO: not sure if this class should be kept
                GameData.ClientMatch = new ClientMatch(data.Result);

                StateMachine.SwapState(new PlayMatchScreen(data.Result));
            }
            else
            {
                // TODO: Swap state to some error screen
            }
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return null;
        }
        #endregion

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            string input = UiElements.InputGameId?.text;
            string matchId = string.IsNullOrWhiteSpace(input) ? GameData.StartupOptions.MatchId : input;

            var loadState = new LoadingPopup<MatchDto>(DueloCollection.Match, matchId);
            StateMachine.PushState(loadState);
        }
        #endregion
    }
}