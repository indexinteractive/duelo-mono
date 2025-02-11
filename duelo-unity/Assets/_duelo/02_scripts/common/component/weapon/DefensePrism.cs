namespace Duelo.Common.Component
{
    using UnityEngine;

    public class DefensePrism : RoundFiniteObject
    {
        #region Public Properties
        public float RotationSpeed = 150f;
        public bool RotateClockwise = true;
        #endregion

        #region Unity Lifecycle
        private void Update()
        {
            Rotate();
        }
        #endregion

        #region Helpers
        private void Rotate()
        {
            var speed = RotateClockwise ? RotationSpeed : -RotationSpeed;
            transform.Rotate(Vector3.up, speed * Time.deltaTime);
        }
        #endregion

        #region RoundFiniteObject Implementation
        protected override bool OnDecomission()
        {
            return true;
        }
        #endregion
    }
}