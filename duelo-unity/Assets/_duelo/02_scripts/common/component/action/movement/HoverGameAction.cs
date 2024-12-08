namespace Duelo.Common.Component
{
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// Created by <see cref="Kernel.HoverDescriptor"/>
    /// </summary>
    public class HoverGameAction : GameAction
    {
        #region Private Fields
        private bool _targetReached = false;
        private Vector3 _destination;
        private VelocityComponent _velocityComponent;
        #endregion

        #region Public Properties
        [Header("Hover Properties")]
        [SerializeField]
        private float stopThreshold = 0.05f;

        public float Speed => _velocityComponent.SpeedPerStep;
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
        }

        public override void OnActionMounted() { }
        public override void OnActionRemoved() { }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _velocityComponent = GetComponent<VelocityComponent>();
            Assert.IsNotNull(_velocityComponent, "[HoverComponent] Missing VelocityComponent.");
        }

        private void Update()
        {
            if (_targetReached)
            {
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _destination, Speed * Time.deltaTime);

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
