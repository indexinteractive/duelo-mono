#if UNITY_EDITOR
#define DUELO_LOCAL
#define UNITY_SERVER
#else
#undef DUELO_LOCAL
#endif

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
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            Debug.Log($":::: DUELO game - {Application.version} ::::");

#if DUELO_LOCAL
            Debug.Log("[GameMain] Running in local mode");
#endif

            var startupOptions = new StartupOptions(_startupMode, _editorCommandLineArgs.Split(' '));
            GameData.StartupOptions = startupOptions;
            Debug.Log(startupOptions);

            GameData.StateMachine = new StateMachine();

#if UNITY_SERVER
            if (startupOptions.StartupType == StartupMode.Server)
            {
                yield return FirebaseInstance.Instance.Initialize("FIR_SERVER", false);

#if !DUELO_LOCAL
                GameData.AppQuitTimer = AppQuitTimer.RunInstance(startupOptions.ServerExpirationSeconds);
#endif
                GameData.Prefabs = FindAnyObjectByType<PrefabList>();
                GameData.Map = FindAnyObjectByType<DueloMap>();

                GameData.StateMachine.PushState(new StateRunServerMatch());
            }
#endif
            if (startupOptions.StartupType == StartupMode.Client)
            {
                yield return FirebaseInstance.Instance.Initialize(startupOptions.PlayerIdOverride, false);

                GameData.Prefabs = FindAnyObjectByType<PrefabList>();
                GameData.Map = FindAnyObjectByType<DueloMap>();
                GameData.Camera = FindAnyObjectByType<DueloCamera>();

                GameData.StateMachine.PushState(new LoadingScreen());
            }
        }

        public void Update()
        {
            GameData.StateMachine.Update();
        }

        public void FixedUpdate()
        {
            GameData.StateMachine.FixedUpdate();
        }

        private void OnDestroy()
        {
            GameData.AppQuitTimer?.Cancel();
            GameData.ClientMatch?.Dispose();
            GameData.StateMachine?.CurrentState?.OnExit();
        }
        #endregion
    }
}