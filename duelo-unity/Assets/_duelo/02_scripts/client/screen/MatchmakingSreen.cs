namespace Duelo.Client.Screen
{
    using System.Linq;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Util;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// Match Making screen. Uses <see cref="UI.MatchmakingUi"/> as the main UI.
    /// </summary>
    public class MatchmakingScreen : GameScreen
    {
        #region Public Properties
        public GameObject StartButton;
        public MatchmakingUi UiElements;
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MatchmakingScreen] OnEnter");
            UiElements = SpawnUI<MatchmakingUi>(UIViewPrefab.Matchmaking);
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
                Debug.LogError("[MatchmakingScreen] Map not found, crashing");
                Application.Quit(ExitCode.MapNotFound);
            }

            GameData.Map.Load(dto);
            GameData.ClientMatch.LoadAssets();
            GameData.Kernel.RegisterEntities(GameData.ClientMatch.Players.Values.ToArray());
        }
        #endregion
    }
}