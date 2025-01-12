namespace Ind3x.Util
{
    using System.Collections;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Firebase;
    using Firebase.Auth;
    using Firebase.Database;
    using Firebase.Extensions;
    using UnityEngine;

    public class FirebaseInstance : Singleton<FirebaseInstance>
    {
        #region Fields
        private bool _firebaseInitialized;
        private FirebaseApp _app;

        public FirebaseApp App => _app;
        public FirebaseDatabase Db => FirebaseDatabase.GetInstance(App);
        public FirebaseAuth Auth => FirebaseAuth.GetAuth(App);
        #endregion

        #region Initialization
        public IEnumerator Initialize(string appName, bool usePersistence = true)
        {
            string filePath = System.IO.Path.Combine(Application.streamingAssetsPath, "google-services-desktop.json");
            Debug.Log("Loading Firebase config from: " + filePath);

            string jsonContents = System.IO.File.ReadAllText(filePath);
            var options = AppOptions.LoadFromJsonConfig(jsonContents);

            _app = FirebaseApp.Create(options, appName);

            Db.SetPersistenceEnabled(usePersistence);

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

            while (!_firebaseInitialized)
            {
                yield return null;
            }
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
            Dictionary<string, object> defaults = new Dictionary<string, object>() { };

            // var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            // return remoteConfig.SetDefaultsAsync(defaults)
            //   .ContinueWith(result => remoteConfig.FetchAndActivateAsync())
            //   .Unwrap();

            return UniTask.FromResult(0);
        }
        #endregion
    }
}