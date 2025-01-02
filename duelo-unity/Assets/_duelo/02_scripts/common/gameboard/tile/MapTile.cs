namespace Duelo.Gameboard
{
    using System.Collections.Generic;
    using UnityEngine;

    public class MapTile : MonoBehaviour, ITraversable<MapTile>
    {
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

        #region Overlays
        public virtual void ClearOverlays(bool clearColorOverlay = true, bool clearDebugOverlays = true, bool clearArrowOverlays = true) { }

        /// <summary>
        /// Sets the overlay to look at a specific target while keeping the overlay
        /// plane parallel to the ground.
        /// </summary>
        public virtual void OverlayLookAt(PathIndicator overlay, Vector3 actualTarget) { }

        /// <summary>
        /// Rotates the overlay in the direction of the given vector
        /// </summary>
        public virtual void OverlayRotation(PathIndicator overlay, Vector3 direction) { }

        /// <summary>
        /// Sets the active overlay using a specified indicator
        /// </summary>
        public virtual void SetOverlay(PathIndicator overlay, bool isActive = true) { }

        /// <summary>
        /// Sets the overlay based on a specific color
        /// </summary>
        public virtual void SetOverlay(Color color, bool isActive = true) { }
        #endregion
    }
}