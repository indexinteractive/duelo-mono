namespace Duelo.Common.Component
{
    using UnityEngine;

    public class HoverComponent : ActionComponent
    {
        #region Private Fields
        private bool _targetReached = false;
        private Vector3 _destination;
        private float _speed = 5f;
        #endregion

        #region Public Properties
        [Header("Hover Properties")]
        [SerializeField]
        private float stopThreshold = 0.05f;
        #endregion

        #region ActionComponent Implementation
        public override bool IsFinished => _targetReached;

        public override void Initialize(params object[] args)
        {
            if (args.Length > 0 && args[0] is Vector3 destination)
            {
                _destination = destination;
                _targetReached = false;
            }
            else
            {
                Debug.LogError("[HoverComponent] Invalid or missing arguments for Initialize.");
            }

            if (args.Length > 1 && args[1] is float speed)
            {
                _speed = speed;
            }
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            if (_targetReached)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _destination) <= stopThreshold)
            {
                _targetReached = true;
                transform.position = _destination;
                Debug.Log("[HoverComponent] Destination reached.");
            }
        }
        #endregion
    }
}
