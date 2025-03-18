namespace Duelo.Client.Screen
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Util;
    using Duelo.Database;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Debugging Match screen, intended for local development only. This screen is a testing replacement
    /// to <see cref="MatchmakingScreen"/> that we arrived at from <see cref="MainMenuScreen.HandleUIEvent"/>.
    ///
    /// If we arrived at this screen, expect DUELO_LOCAL to be defined.
    ///
    /// Uses <see cref="UI.DebugMatchViewUi"/> as the main UI.
    /// </summary>
    public class DebugMatchScreen : GameScreen
    {
        #region Public Properties
        public DebugMatchViewUi UiElements;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[DebugMatchScreen] OnEnter");
            UiElements = SpawnUI<DebugMatchViewUi>(UIViewPrefab.DebugMatch);
            UiElements.InputGameId.text = GlobalState.StartupOptions.MatchId;
        }

        public override void Resume(StateExitValue results)
        {
            Debug.Log("[DebugMatchScreen] Resume");
            var data = results.data as LoadingPopup<MatchDto>.LoadResult;

            if (data.Result?.MatchId != null)
            {
                Debug.Log("[DebugMatchScreen] Match found: " + data.Result.MatchId);

                GlobalState.Kernel = new MatchKernel();

                var db = new FirebaseMatchDatabase(data.Result.MatchId);
                GlobalState.Match = new ClientMatch(data.Result, db);

                GlobalState.Services.GetMap(data.Result.MapId)
                    .ContinueWith(LoadAssets)
                    .ContinueWith(() =>
                    {
                        var camera = GameObject.FindAnyObjectByType<DueloCamera>();
                        camera.SetMapCenter(GlobalState.Map.MapCenter);
                        camera.FollowPlayers(Match.Players.ToDictionary(x => x.Key, kvp => kvp.Value));
                        GlobalState.Camera = camera;

                        StateMachine.SwapState(new PlayMatchScreen());
                    });
            }
            else
            {
                // TODO: Swap state to some error screen
                Debug.LogError("[DebugMatchScreen] Match not found");
                StateMachine.SwapState(new MainMenuScreen());
            }
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return null;
        }
        #endregion

        #region Loading
        public void LoadAssets(DueloMapDto dto)
        {
            if (dto == null)
            {
                Debug.LogError("[DebugMatchScreen] Map not found, crashing");
                Application.Quit(ExitCode.MapNotFound);
            }

            GlobalState.Map.Load(dto);
            Client.LoadAssets();
            GlobalState.Kernel.RegisterEntities(Match.Players[PlayerRole.Challenger], Match.Players[PlayerRole.Defender]);
        }
        #endregion

        #region Input
        public override void HandleUIEvent(GameObject source, object eventData)
        {
            string input = UiElements.InputGameId?.text;
            if (!string.IsNullOrWhiteSpace(input))
            {
                var loadState = new LoadingPopup<MatchDto>(Database.SchemaCollection.Match, input);
                StateMachine.PushState(loadState);
            }
        }
        #endregion
    }
}