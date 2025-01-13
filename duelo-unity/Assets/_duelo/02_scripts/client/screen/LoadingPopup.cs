namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Client.UI;
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Firebase.Database;
    using Ind3x.State;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    /// <summary>
    /// Utility popup that can fetch data from firebase.
    /// The type <see cref="T" /> must be a class that can be deserialized from JSON
    /// and will be returned as the result of the fetch.
    /// </summary>
    public class LoadingPopup<T> : GameScreen where T : class
    {
        #region LoadResult class
        public class LoadResult
        {
            public T Result;
            public bool WasSuccessful;

            public LoadResult(T results, bool wasSuccessful)
            {
                Result = results;
                WasSuccessful = wasSuccessful;
            }
        }
        #endregion

        #region Constants
        private const float TimeoutSeconds = 10.0f;
        #endregion

        #region Firebase Fields
        private DatabaseReference _ref;
        private bool _isComplete = false;
        private bool _wasSuccessful = false;
        private float _timeoutTime;
        private PopupMessage _ui;
        #endregion

        #region Data
        private T result = default(T);
        #endregion

        #region Initialization
        public LoadingPopup(DueloCollection collection, string path)
        {
            string collectionName = collection.ToString().ToLower();
            string pathString = string.Join("/", path);

            _ref = FirebaseInstance.Instance.Db.GetReference(collectionName).Child(pathString);
        }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            _ref.GetValueAsync().AsUniTask().ContinueWith(OnDataReturned);

            _ui = SpawnUI<PopupMessage>(UIViewPrefab.PopupMessage);
            _timeoutTime = Time.realtimeSinceStartup + TimeoutSeconds;
        }

        public override void Update()
        {
            if (_isComplete || Time.realtimeSinceStartup > _timeoutTime)
            {
                StateMachine.PopState();
            }
            else
            {
                UpdateLabelText();
            }
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return new StateExitValue(typeof(LoadingPopup<T>), new LoadResult(result, _wasSuccessful));
        }
        #endregion

        #region Inherited Methods
        protected virtual T ParseResult(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

        #region Private Helpers
        private void OnDataReturned(DataSnapshot snapshot)
        {
            _wasSuccessful = true;

            if (snapshot.Exists)
            {
                var json = snapshot.GetRawJsonValue();
                if (!string.IsNullOrEmpty(json))
                {
                    result = ParseResult(json);
                }
            }

            _isComplete = true;
        }

        private void UpdateLabelText()
        {
            _ui.Message.text = Strings.LabelLoading + Ind3x.Util.Formatting.EllipsisGenerator();
        }
        #endregion
    }

    /// <summary>
    /// Utility popup that displays a standard loading message.
    /// Similar to <see cref="LoadingPopup{T}"/>, but does not fetch data from firebase.
    /// Needs to be manually removed by the caller.
    /// </summary>
    public class LoadingPopup : GameScreen
    {
        #region Firebase Fields
        private PopupMessage _ui;
        #endregion

        #region Initialization
        public LoadingPopup() { }
        #endregion

        #region GameScreen Implementation
        public override void OnEnter()
        {
            _ui = SpawnUI<PopupMessage>(UIViewPrefab.PopupMessage);
        }

        public override void Update()
        {
            UpdateLabelText();
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return new StateExitValue(typeof(LoadingPopup));
        }
        #endregion

        #region Private Helpers
        private void UpdateLabelText()
        {
            _ui.Message.text = Strings.LabelLoading + Ind3x.Util.Formatting.EllipsisGenerator();
        }
        #endregion
    }
}
