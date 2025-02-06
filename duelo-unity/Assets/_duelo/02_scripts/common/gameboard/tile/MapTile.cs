namespace Duelo.Gameboard
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum Overlay
    {
        MoveTarget,
        AttackTarget,
        Selected,
        Origin,
        Straight,
        Bend,
        End
    }

    public class MapTile : MonoBehaviour, ITraversable<MapTile>
    {
        #region Overlays
        protected Dictionary<Overlay, GameObject> _overlays = new();
        protected GameObject _colorOverlay;
        #endregion

        #region Public Properties
        /// <summary>
        /// Set by <see cref="DueloMap.SetMovableTiles"/>  indicate that this tile is movable.
        /// </summary>
        public bool IsMovable;
        #endregion

        #region Pathfinding
        public List<MapTile> GetNeighbors(bool includeUnreachable, bool includeDiagonals)
        {
            var neighbors = new List<MapTile>();
            var list = includeDiagonals ? Util.AllDirections : Util.CardinalDirections;

            foreach (var direction in list)
            {
                var node = GetNeighboringNode(direction, includeUnreachable);
                if (node != null)
                {
                    neighbors.Add(node);
                }
            }

            return neighbors;
        }

        /// <param name="includeBlockedTiles">Will include tiles that have objects above</param>
        /// <returns></returns>
        public MapTile GetNeighboringNode(Vector3 direction, bool includeBlockedTiles)
        {
            Vector3 halfExtents = new Vector3(0.25f, (1) / 2.0f, 0.25f);
            Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

            foreach (Collider item in colliders)
            {
                MapTile tile = item.GetComponent<MapTile>();
                if (tile != null)
                {
                    // Checks if there is something above this tile and does NOT return it
                    // if there is a hit, as it would be unwalkable.
                    if (includeBlockedTiles || !Physics.Raycast(tile.transform.position, Vector3.up, out var hit, 1))
                    {
                        return tile;
                    }
                }
            }

            return null;
        }

        public float DistanceTo(MapTile other)
        {
            return Vector3.Distance(this.transform.position, other.transform.position);
        }
        #endregion

        #region Public Methods
        public void MakeMoveTarget()
        {
            SetOverlay(Overlay.MoveTarget);
        }

        public void MakeAttackTarget()
        {
            SetOverlay(Overlay.AttackTarget);
        }
        #endregion

        #region Overlays
        public virtual void ClearOverlays(bool clearColorOverlay = true, bool clearDebugOverlays = true, bool clearArrowOverlays = true)
        {
            if (clearColorOverlay && _colorOverlay != null)
            {
                _colorOverlay.SetActive(false);
            }

            if (clearDebugOverlays)
            {
                foreach (Overlay item in Enum.GetValues(typeof(Overlay)))
                {
                    SetOverlay(item, false);
                }
            }

            if (clearArrowOverlays)
            {
                foreach (Overlay item in Enum.GetValues(typeof(Overlay)))
                {
                    SetOverlay(item, false);
                }
            }
        }

        public virtual bool IsOverlayActive(Overlay overlay)
        {
            return _overlays[overlay].activeSelf;
        }

        /// <summary>
        /// Sets the overlay to look at a specific target while keeping the overlay
        /// plane parallel to the ground.
        /// </summary>
        public virtual void OverlayLookAt(Overlay overlay, Vector3 actualTarget)
        {
            if (_overlays.ContainsKey(overlay))
            {
                actualTarget.y = transform.position.y;
                var direction = Vector3.Normalize(actualTarget - this.transform.position);
                OverlayRotation(overlay, direction);
            }
        }

        /// <summary>
        /// Rotates the overlay in the direction of the given vector
        /// </summary>
        public virtual void OverlayRotation(Overlay overlay, Vector3 actualTarget)
        {
            var overlayObject = _overlays[overlay];
            overlayObject.transform.rotation = Quaternion.LookRotation(actualTarget, Vector3.up);
        }

        /// <summary>
        /// Sets the active overlay using a specified indicator
        /// </summary>
        public virtual void SetOverlay(Overlay overlay, bool isActive = true)
        {
            if (_overlays.ContainsKey(overlay))
            {
                _overlays[overlay].SetActive(isActive);
            }
        }

        /// <summary>
        /// Sets the overlay based on a specific color
        /// </summary>
        public virtual void SetOverlay(Color color, bool isActive = true)
        {
            if (_colorOverlay != null)
            {
                _colorOverlay.SetActive(isActive);
                var material = _colorOverlay.GetComponent<Renderer>().material;
                material.color = color;
                material.SetColor("_EmissionColor", color);
            }
        }
        #endregion
    }
}