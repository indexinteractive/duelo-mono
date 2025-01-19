namespace Duelo
{
    using Duelo.Client.Camera;
    using Duelo.Client.Screen;
    using Duelo.Common.Core;
    using Duelo.Gameboard;
    using Duelo.Server.State;
    using Ind3x.State;
    using Ind3x.Util;
    using System.Collections;
    using UnityEngine;

    public class GameMain : MonoBehaviour
    {
        #region Public Properties
        [Header("Startup Options")]
        [Tooltip("Tells the game to start as a server or client")]
        [SerializeField]
        private StartupMode _startupMode;

        [Tooltip("Editor string that can be used to simulate command line arguments. Any value used here will have priority over other values.")]
        [SerializeField]
        private string _editorCommandLineArgs = "";

        public readonly StateMachine StateMachine = new();
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            Debug.Log($":::: DUELO game - {Application.version} ::::");

            var startupOptions = new StartupOptions(_startupMode, _editorCommandLineArgs.Split(' '));
            GameData.StartupOptions = startupOptions;
            Debug.Log(startupOptions);

            if (startupOptions.StartupType == StartupMode.Server)
            {
                yield return FirebaseInstance.Instance.Initialize("FIR_SERVER", false);

                GameData.AppQuitTimer = AppQuitTimer.RunInstance(startupOptions.ServerExpirationSeconds);
                GameData.Prefabs = FindAnyObjectByType<PrefabList>();
                GameData.Map = FindAnyObjectByType<DueloMap>();

                StateMachine.PushState(new StateRunServerMatch());
            }
            else if (startupOptions.StartupType == StartupMode.Client)
            {
                yield return FirebaseInstance.Instance.Initialize(startupOptions.PlayerIdOverride, false);

                GameData.StateMachine = StateMachine;

                GameData.Prefabs = FindAnyObjectByType<PrefabList>();
                GameData.Map = FindAnyObjectByType<DueloMap>();
                GameData.Camera = FindAnyObjectByType<DueloCamera>();

                StateMachine.PushState(new LoadingScreen());
            }
        }

        public void Update()
        {
            StateMachine.Update();
        }

        public void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void OnDestroy()
        {
            GameData.AppQuitTimer?.Cancel();
            GameData.ClientMatch?.Dispose();
        }
        #endregion
    }
}