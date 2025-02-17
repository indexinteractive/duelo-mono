namespace Duelo.Common.Component
{
    using UnityEngine;

    public class DefenseRingAction : GameAction
    {
        #region Public Properties
        public GameObject RingPrefab;
        public Vector3 ScaleModifier;
        #endregion

        #region Private Fields
        private bool _isFinished;
        #endregion

        #region Game Action Implementation
        public override bool IsFinished => _isFinished;

        public override void OnActionMounted()
        {
            var shield = InstantiateAtCenter(RingPrefab, transform);

            // TODO: this works for the capsule character since the collider is in the first
            // child transform. But this is far from ideal.
            var collider = GetComponentInChildren<Collider>();
            ResizeToColliderBounds(shield, collider);
            AdjustPosititionOffset(shield, collider);

            _isFinished = true;
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Helpers
        private GameObject InstantiateAtCenter(GameObject prefab, Transform parent)
        {
            var shield = Instantiate(prefab, parent);
            shield.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            return shield;
        }

        private void ResizeToColliderBounds(GameObject shield, Collider collider)
        {
            var bounds = collider.bounds;
            shield.transform.localScale = new Vector3(
                bounds.size.x * ScaleModifier.x,
                bounds.size.y * ScaleModifier.y,
                bounds.size.z * ScaleModifier.z
            );
        }

        /// <summary>
        /// This is necessary since the origin of the player is actually the center of each map tile.
        /// The 3d model and collider are not centered, but are offset to be above the tile itself
        /// </summary>
        private void AdjustPosititionOffset(GameObject shield, Collider collider)
        {
            Vector3 offset = transform.position - collider.bounds.center;
            shield.transform.position = transform.position - offset;
        }
        #endregion
    }
}