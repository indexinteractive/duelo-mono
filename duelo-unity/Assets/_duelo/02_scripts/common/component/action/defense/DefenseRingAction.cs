namespace Duelo.Common.Component
{
    using Duelo.Common.Player;
    using UnityEngine;

    // TODOOOO

    // split out shield logic as a separate component
    // this should just spawn it and thats its
    // rotation / round finite object logic should be in the component



    public class DefenseRingAction : GameAction
    {
        #region Public Properties
        public GameObject ShieldPrefab;
        #endregion

        #region Private Fields
        private bool _isFinished;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            var playerTraits = GetComponent<PlayerTraits>();
            ShieldPrefab = playerTraits.DefenseRingPrefab;
        }
        #endregion

        #region Game Action Implementation
        public override bool IsFinished => _isFinished;

        public override void OnActionMounted()
        {
            var shield = InstantiateAtCenter(ShieldPrefab, transform);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localRotation = Quaternion.identity;

            var collider = GetComponent<Collider>();
            ResizeToColliderBounds(shield, collider);

            _isFinished = true;
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Helpers
        private GameObject InstantiateAtCenter(GameObject prefab, Transform parent)
        {
            var shield = Instantiate(prefab, parent);
            shield.transform.localPosition = Vector3.zero;
            return shield;
        }

        private void ResizeToColliderBounds(GameObject shield, Collider collider)
        {
            var bounds = collider.bounds;
            shield.transform.localScale = new Vector3(bounds.size.x, bounds.size.y, bounds.size.z);
        }
        #endregion
    }
}