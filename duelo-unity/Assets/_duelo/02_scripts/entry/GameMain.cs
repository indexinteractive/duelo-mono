namespace Duelo
{
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Client.Screen;
    using Duelo.Common.Core;
    using Duelo.Gameboard;
    using Duelo.Server.State;
    using Firebase;
    using Firebase.Extensions;
    using Ind3x.State;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameMain : MonoBehaviour
    {
        #region Fields
        [HideInInspector]
        private bool _firebaseInitialized;
        #endregion

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
            InitializeFirebase();

            while (!_firebaseInitialized)
            {
                yield return null;
            }

            var startupOptions = new StartupOptions(_startupMode, _editorCommandLineArgs.Split(' '));
            Debug.Log(startupOptions);

            if (startupOptions.StartupType == StartupMode.Server)
            {
                // TODO: Implement expiration timer for server
                GameData.StartupOptions = startupOptions;
                GameData.Prefabs = FindAnyObjectByType<PrefabList>();
                GameData.Map = FindAnyObjectByType<DueloMap>();

                StateMachine.PushState(new StateRunServerMatch());
            }
            else if (startupOptions.StartupType == StartupMode.Client)
            {
                GameData.StartupOptions = startupOptions;
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
        #endregion

        #region Firebase Methods
        private void InitializeFirebase()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebaseComponents();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
                    Application.Quit();
                }
            });
        }

        private void InitializeFirebaseComponents()
        {
            UniTask.WhenAll(
                InitializeRemoteConfig()
            ).ContinueWith(() =>
            {
                _firebaseInitialized = true;
            });
        }

        // Sets the default values for remote config.  These are the values that will
        // be used if we haven't fetched yet.
        UniTask InitializeRemoteConfig()
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>()
            { };

            // var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            // return remoteConfig.SetDefaultsAsync(defaults)
            //   .ContinueWith(result => remoteConfig.FetchAndActivateAsync())
            //   .Unwrap();

            return UniTask.FromResult(0);
        }
        #endregion
    }
}