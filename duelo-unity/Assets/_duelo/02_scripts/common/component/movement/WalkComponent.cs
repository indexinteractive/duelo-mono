namespace Duelo.Common.Component
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class WalkComponent : ActionComponent
    {
        #region Private Fields
        private bool _targetReached;
        private Vector3 _destination;
        #endregion

        #region ActionComponent Implementation
        public override bool IsFinished => _targetReached;

        public override void Initialize(params object[] args)
        {
            _destination = (Vector3)args[0];
        }

        public override void OnActionRemoved()
        {
        }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            Debug.Log("Walking to " + _destination);
            UniTask.Delay(1000).ContinueWith(() =>
            {
                _targetReached = true;
                Debug.Log("Finished Walking");
            });
        }
        #endregion
    }
}