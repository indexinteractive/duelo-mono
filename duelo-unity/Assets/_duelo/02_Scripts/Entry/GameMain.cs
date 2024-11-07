namespace Ind3x.Duelo
{
    using System.Collections;
    using System.Collections.Generic;
    using Firebase;
    using Firebase.Extensions;
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public class GameMain : MonoBehaviour
    {
        #region Fields
        [HideInInspector]
        private bool _firebaseInitialized;
        #endregion

        #region Public Properties
        [Tooltip("Firebase match id")]
        public string MatchId;
        #endregion

        #region Unity Lifecycle
        public IEnumerator Start()
        {
            InitializeFirebase();

            while (!_firebaseInitialized)
            {
                yield return null;
            }

            CommonData.StartupOptions = System.Environment.GetEnvironmentVariable("UNITY_HEADLESS") != null
                ? GetCommandLineOptions()
                : GetEditorOptions();

            Debug.Log(CommonData.StartupOptions);

            // TODO: Implement expiration timer for server


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

            return new StartupOptions(StartupType.Server, "match", expireMin);
        }
        #endregion

        #region Client Initialization
        private StartupOptions GetEditorOptions()
        {
            return new StartupOptions(StartupType.Client, MatchId, 0);
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
        #endregion
    }
}