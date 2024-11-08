namespace Duelo
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Server.State;
    using Firebase;
    using Firebase.Extensions;
    using Ind3x.State;
    using Microsoft.Extensions.Configuration;
    using Newtonsoft.Json;
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
        [Header("Game Configuration")]
        [Tooltip("Tells the game to start as a server or client")]
        public StartupMode GameType;

        [Tooltip("Firebase match id")]
        public string MatchId;

        public StateMachine StateMachine { get; private set; }
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            StateMachine = new StateMachine();

            InitializeFirebase();

            while (!_firebaseInitialized)
            {
                yield return null;
            }

            var startupOptions = System.Environment.GetEnvironmentVariable("UNITY_HEADLESS") != null
                ? GetCommandLineOptions()
                : GetEditorOptions();

            Debug.Log(startupOptions);

            // TODO: Implement expiration timer for server

            MatchService.Instance.GetMatch(startupOptions.MatchId)
                .ContinueWith(match =>
                {
                    if (!ValidateMatch(match))
                    {
                        Debug.LogError("[GameMain] Invalid match, crashing");
                        Application.Quit(1);
                        return;
                    }

                    Debug.Log("found match: " + match.MatchId);

                    if (startupOptions.StartupType == StartupMode.Server)
                    {
                        ServerData.StartupOptions = startupOptions;
                        ServerData.MatchDto = match;
                        StateMachine.PushState(new StateRunServerMatch());
                    }
                    else if (startupOptions.StartupType == StartupMode.Client)
                    {
                        Debug.Log("TODO: Client startup");
                    }
                });
        }

        public void Update()
        {
            StateMachine.Update();
        }

        public void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        public void OnGUI()
        {
            StateMachine.OnGUI();
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

        #region Server Initialization
        private StartupOptions GetCommandLineOptions()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            IConfiguration config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            int expireMin = GetArgOrDefault(config["expire"], 0);
            string matchId = GetArgOrDefault(config["match"], string.Empty);

            if (string.IsNullOrWhiteSpace(matchId) || matchId.Length != 24)
            {
                throw new System.Exception($"Invalid matchId: {matchId}");
            }

            return new StartupOptions(StartupMode.Server, "match", expireMin);
        }
        #endregion

        #region Client Initialization
        private StartupOptions GetEditorOptions()
        {
            return new StartupOptions(GameType, MatchId, 0);
        }
        #endregion

        #region Helpers
        private T GetArgOrDefault<T>(string value, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        private bool ValidateMatch(MatchDto match)
        {
            if (match == null)
            {
                Debug.LogError("[GameMain] Match is null (not found)");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(match.MatchId))
            {
                Debug.LogError("[GameMain] Match id is null or empty");
                return false;
            }
            else if (match.ClockConfig == null)
            {
                Debug.LogError("[GameMain] Match clock config is null");
                return false;
            }

            return true;
        }
        #endregion
    }
}