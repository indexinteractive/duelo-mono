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
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// DebugMatch screen. Uses <see cref="UI.DebugMatchViewUi"/> as the main UI.
    /// </summary>
    public class DebugMatchScreen : GameScreen
    {
        #region Public Properties
        public GameObject StartButton;
        public DebugMatchViewUi UiElements;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[DebugMatchScreen] OnEnter");
            UiElements = SpawnUI<DebugMatchViewUi>(UIViewPrefab.DebugMatch);
        }

        public override void Resume(StateExitValue results)
        {
            Debug.Log("[DebugMatchScreen] Resume");
            var data = results.data as LoadingPopup<MatchDto>.LoadResult;

            if (data.Result?.MatchId != null)
            {
                Debug.Log("[DebugMatchScreen] Match found: " + data.Result.MatchId);

                GameData.Kernel = new MatchKernel();
                GameData.ClientMatch = new ClientMatch(data.Result);
                MapService.Instance.GetMap(data.Result.MapId)
                    .ContinueWith(LoadAssets)
                    .ContinueWith(() =>
                    {
                        var camera = GameObject.FindAnyObjectByType<DueloCamera>();
                        camera.SetMapCenter(GameData.Map.MapCenter);
                        camera.FollowPlayers(GameData.ClientMatch.Players);
                        GameData.Camera = camera;

                        StateMachine.SwapState(new PlayMatchScreen(data.Result));
                    });
            }
            else
            {
                // TODO: Swap state to some error screen
                Debug.LogError("[DebugMatchScreen] Match not found");
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

            GameData.Map.Load(dto);
            GameData.ClientMatch.LoadAssets();
            GameData.Kernel.RegisterEntities(GameData.ClientMatch.Players.Values.ToArray());
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