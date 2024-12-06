namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Pathfinding;
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
        private bool PreviousPathIsIdentical(List<MapTile> oldPath, List<MapTile> newPath)
        {
            if (oldPath.Count == newPath.Count)
            {
                var bothPaths = oldPath.Zip(newPath, (oldTile, newTile) => new { oldTile, newTile });
                return !bothPaths.Any(x => x.oldTile != x.newTile);
            }

            return false;
        }

        public void PaintPathTiles(Path<MapTile> path)
        {
            if (path != null)
            {
                var newPath = path.AsNodeList();
                if (PreviousPathIsIdentical(_previousPath, newPath))
                {
                    return;
                }

                // Reset all tiles in the old path, even if some overlap with this new path
                ClearPath();

                // The path begins at the target and moves backwards to the root
                foreach (var node in path)
                {
                    // At the beginning of the path, root
                    if (node.Previous == null && node.Next != null)
                    {
                        Debug.DrawRay(node.Data.transform.position, Vector3.up * 5f, Color.cyan);
                        (node.Data as DevTile)?.SetOverlay(PathIndicator.Origin);
                        (node.Data as DevTile)?.OverlayLookAt(PathIndicator.Origin, node.Next.Data.transform.position);
                    }
                    // At the end of the path, target
                    else if (node.Next == null)
                    {
                        Debug.DrawRay(node.Data.transform.position, Vector3.up * 5f, Color.red);
                        (node.Data as DevTile)?.SetOverlay(PathIndicator.End);

                        var direction = (node.Data.transform.position - node.Previous.Data.transform.position).normalized;
                        (node.Data as DevTile)?.OverlayRotation(PathIndicator.End, direction);

                        (node.Data as DevTile)?.SetOverlay(PathIndicator.Selected);

                        if (GhostModel != null)
                        {
                            DestroyGhost();
                            CreateGhost(node.Data, direction);
                        }
                    }
                    // All nodes in between
                    else
                    {
                        var thisPosition = node.Data.transform.position;

                        var prev = node.Previous.Data.transform.position;
                        prev.y = thisPosition.y;

                        var next = node.Next.Data.transform.position;
                        next.y = thisPosition.y;

                        var thisToPrev = (prev - thisPosition).normalized;
                        var thisToNext = (next - thisPosition).normalized;

                        var cross = Vector3.Cross(thisToPrev, thisToNext);

                        // A non-zero cross product means that the vectors are not parallel

                        if (cross != Vector3.zero)
                        {
                            (node.Data as DevTile)?.SetOverlay(PathIndicator.Bend);

                            if (cross.y > 0)
                            {
                                (node.Data as DevTile)?.OverlayRotation(PathIndicator.Bend, thisToPrev);
                            }
                            else
                            {
                                (node.Data as DevTile)?.OverlayRotation(PathIndicator.Bend, thisToNext);
                            }
                        }
                        else
                        {
                            (node.Data as DevTile)?.SetOverlay(PathIndicator.Straight);
                            (node.Data as DevTile)?.OverlayLookAt(PathIndicator.Straight, node.Previous.Data.transform.position);
                        }
                    }
                }

                _previousPath = newPath;
            }
        }

        public void ClearPath()
        {
            foreach (var tile in _previousPath)
            {
                (tile as DevTile)?.ClearOverlays(false, false, true);
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