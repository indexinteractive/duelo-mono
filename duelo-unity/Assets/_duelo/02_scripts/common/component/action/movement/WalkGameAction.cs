namespace Duelo.Common.Component
{
    using System.Collections.Generic;
    using Duelo.Common.Core;
    using Duelo.Common.Pathfinding;
    using Duelo.Gameboard;
    using UnityEngine;

    public class WalkGameAction : GameAction
    {
        #region Private Fields
        private Queue<MapTile> _pathQueue;
        private bool _targetReached;
        private MapTile _targetTile;

        private VelocityComponent _velocityComponent;
        #endregion

        #region Public Properties
        public float Speed => _velocityComponent.SpeedPerStep;
        #endregion

        #region ActionComponent Implementation
        public override bool IsFinished => _targetReached;

        public override void Initialize(params object[] args)
        {
            var targetPosition = (Vector3)args[0];
            _targetTile = GameData.Map.GetTile(targetPosition);
        }

        public override void OnActionMounted()
        {
            var currentTile = GameData.Map.GetTile(transform.position);

            var path = AStar.FindPathToTile(currentTile, _targetTile);

            if (path != null)
            {
                var reversedPath = path.AsNodeList();
                reversedPath.Reverse();

                _pathQueue = new Queue<MapTile>(reversedPath);
            }
            else
            {
                Debug.LogWarning("No path found to the target tile.");
                _targetReached = true; // Mark finished if no valid path
            }
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _velocityComponent = GetComponent<VelocityComponent>();

            if (_pathQueue == null || _pathQueue.Count == 0)
            {
                Debug.LogWarning("Path queue is empty or null.");
                _targetReached = true;
            }
        }

        private void Update()
        {
            if (_targetReached || _pathQueue == null || _pathQueue.Count == 0)
            {
                return;
            }

            MapTile nextTile = _pathQueue.Peek();
            Vector3 directionToNextTile = (nextTile.transform.position - transform.position);

            // We are close enough to consider this tile reached
            if (directionToNextTile.sqrMagnitude < 0.001f)
            {
                _pathQueue.Dequeue();
            }
            else
            {
                // Rotate towards the next tile
                directionToNextTile.Normalize();
                Quaternion targetRotation = Quaternion.LookRotation(directionToNextTile);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _velocityComponent.RotationSpeed * Time.deltaTime);

                // Once we are facing the right direction, move towards the next tile
                if (Quaternion.Angle(transform.rotation, targetRotation) < 1.0f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, nextTile.transform.position, Speed * Time.deltaTime);
                }

                if (Vector3.Distance(transform.position, nextTile.transform.position) < 0.1f)
                {
                    _pathQueue.Dequeue();
                }
            }

            if (_pathQueue.Count == 0)
            {
                _targetReached = true;
                transform.position = nextTile.transform.position;
            }
        }
        #endregion
    }
}