namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    class BasicMapDecorator : IMapDecorator
    {
        #region Private Fields
        private GameObject _ghost = null;
        private List<MapTile> _previousPath = new List<MapTile>();
        #endregion

        #region Public Properties
        public GameObject GhostModel;
        public Material GhostMaterial;
        #endregion

        /// <summary>
        /// If the previous path drawn is the same as the new one that is about
        /// to be drawn, there is no need to clear the tiles and redraw.
        /// This avoids issues with restarting tile animations and flipping
        /// overlays off/on for no reason
        /// </summary>
        private bool PreviousPathIsIdentical(IEnumerable<MapTile> oldPath, IEnumerable<MapTile> newPath)
        {
            if (oldPath.Count() == newPath.Count())
            {
                var bothPaths = oldPath.Zip(newPath, (oldTile, newTile) => new { oldTile, newTile });
                return !bothPaths.Any(x => x.oldTile != x.newTile);
            }

            return false;
        }

        public void PaintPathTiles(List<MapTile> path)
        {
            if (path != null)
            {
                if (PreviousPathIsIdentical(_previousPath, path))
                {
                    return;
                }

                // Reset all tiles in the old path, even if some overlap with this new path
                ClearPath();

                // The path begins at the target and moves backwards to the root
                for (int i = 0; i < path.Count; i++)
                {
                    var tile = path[i];
                    var isPathEnd = i == 0;
                    var isPathOrigin = i == path.Count - 1;

                    // At the beginning of the path, root
                    if (isPathOrigin)
                    {
                        Debug.DrawRay(tile.transform.position, Vector3.up * 5f, Color.cyan);
                        tile.SetOverlay(PathIndicator.Origin);
                        tile.OverlayLookAt(PathIndicator.Origin, path[i - 1].transform.position);
                    }
                    // At the end of the path, target
                    else if (isPathEnd)
                    {
                        Debug.DrawRay(tile.transform.position, Vector3.up * 5f, Color.red);
                        tile.SetOverlay(PathIndicator.End);

                        if (path.Count > 1)
                        {
                            var direction = (tile.transform.position - path[i + 1].transform.position).normalized;
                            tile.OverlayRotation(PathIndicator.End, direction);
                            tile.SetOverlay(PathIndicator.Selected);

                            if (GhostModel != null)
                            {
                                DestroyGhost();
                                CreateGhost(tile, direction);
                            }
                        }
                    }
                    // All nodes in between
                    else
                    {
                        var thisPosition = tile.transform.position;

                        var prev = path[i + 1].transform.position;
                        prev.y = thisPosition.y;

                        var next = path[i - 1].transform.position;
                        next.y = thisPosition.y;

                        var thisToPrev = (prev - thisPosition).normalized;
                        var thisToNext = (next - thisPosition).normalized;

                        var cross = Vector3.Cross(thisToPrev, thisToNext);

                        // A non-zero cross product means that the vectors are not parallel
                        if (cross != Vector3.zero)
                        {
                            tile.SetOverlay(PathIndicator.Bend);

                            if (cross.y > 0)
                            {
                                tile.OverlayRotation(PathIndicator.Bend, thisToPrev);
                            }
                            else
                            {
                                tile.OverlayRotation(PathIndicator.Bend, thisToNext);
                            }
                        }
                        else
                        {
                            tile.SetOverlay(PathIndicator.Straight);
                            tile.OverlayLookAt(PathIndicator.Straight, path[i + 1].transform.position);
                        }
                    }
                }

                _previousPath = new List<MapTile>(path);
            }
        }

        public void ClearPath()
        {
            foreach (var tile in _previousPath)
            {
                tile.ClearOverlays(false, false, true);
            }

            DestroyGhost();
        }

        #region Player Ghost
        public void CreateGhost(MapTile tile, Vector3 direction)
        {
            Vector3 target = tile.transform.position;
            target.y += tile.GetComponent<Collider>().bounds.extents.y;

            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            _ghost = GameObject.Instantiate(GhostModel, target, rotation);
            foreach (var renderer in _ghost.GetComponentsInChildren<Renderer>())
            {
                renderer.material = GhostMaterial;
            }
        }

        private void DestroyGhost()
        {
            if (_ghost != null)
            {
                GameObject.Destroy(_ghost);
            }
        }
        #endregion
    }
}