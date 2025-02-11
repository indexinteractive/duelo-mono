namespace Duelo.Common.Component
{
    using UnityEngine;

    public class RandomRotation : MonoBehaviour
    {
        #region Public Properties
        [Range(0, 100)]
        public float RotationSpeed = 5;
        #endregion

        #region Private Fields
        private Vector3 _randomDirection;
        private float _speed;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _randomDirection = new Vector3(
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f),
                Random.Range(-1.0f, 1.0f)
            );

            var randomSpeedMultiplier = Random.Range(0.5f, 2f);
            _speed = randomSpeedMultiplier * RotationSpeed;
        }

        private void Update()
        {
            transform.Rotate(_randomDirection * Time.deltaTime * _speed);
        }
        #endregion
    }
}