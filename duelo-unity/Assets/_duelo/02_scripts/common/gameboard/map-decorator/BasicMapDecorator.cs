namespace Duelo.Gameboard.MapDecorator
{
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Model;
    using UnityEngine;

    class BasicMapDecorator : IMapDecorator
    {
        #region Decorator Constants
        public static Color MovableTileColor = new Color(0f, 1f, 0f, 0.5f);
        public static Color ActionTileColor = new Color(1f, 0f, 0f, 0.5f);
        #endregion

        #region Private Fields
        private GameObject _ghost = null;
        private readonly Dictionary<PlayerRole, List<MapTile>> _previousPath = new();
        private readonly List<MapTile> _movableTiles = new();
        private readonly List<MapTile> _actionTiles = new();
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

        #region Player Movement Path
        public void PaintPathTiles(PlayerRole role, List<MapTile> path)
        {
            if (path != null)
            {
                if (_previousPath.ContainsKey(role) && PreviousPathIsIdentical(_previousPath[role], path))
                {
                    return;
                }

                // Reset all tiles in the old path, even if some overlap with this new path
                ClearPath(role);

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
                        tile.SetOverlay(Overlay.Origin);
                        tile.OverlayLookAt(Overlay.Origin, path[i - 1].transform.position);
                    }
                    // At the end of the path, target
                    else if (isPathEnd)
                    {
                        Debug.DrawRay(tile.transform.position, Vector3.up * 5f, Color.red);
                        tile.SetOverlay(Overlay.End);

                        if (path.Count > 1)
                        {
                            var direction = (tile.transform.position - path[i + 1].transform.position).normalized;
                            tile.OverlayRotation(Overlay.End, direction);
                            tile.SetOverlay(Overlay.Selected);

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
                            tile.SetOverlay(Overlay.Bend);

                            if (cross.y > 0)
                            {
                                tile.OverlayRotation(Overlay.Bend, thisToPrev);
                            }
                            else
                            {
                                tile.OverlayRotation(Overlay.Bend, thisToNext);
                            }
                        }
                        else
                        {
                            tile.SetOverlay(Overlay.Straight);
                            tile.OverlayLookAt(Overlay.Straight, path[i + 1].transform.position);
                        }
                    }
                }

                _previousPath[role] = new List<MapTile>(path);
            }
        }

        public void ClearPath(PlayerRole role)
        {
            if (_previousPath.ContainsKey(role))
            {
                foreach (var tile in _previousPath[role])
                {
                    tile.ClearOverlays(false, false, true);
                }

                _previousPath.Remove(role);
            }

            DestroyGhost();
        }
        #endregion

        #region Movable Tiles / ChooseMovementPhase
        public void PaintMovableTiles(List<MapTile> tiles)
        {
            foreach (var tile in tiles)
            {
                tile.SetOverlay(MovableTileColor);
                // tile.SetOverlay(Overlay.MoveTarget);
                _movableTiles.Add(tile);
            }
        }

        public void ClearMovableTiles()
        {
            for (int i = _movableTiles.Count - 1; i >= 0; i--)
            {
                _movableTiles[i].ClearOverlays(true, false, false);
                // _movableTiles[i].ClearOverlay(Overlay.MoveTarget);
                _movableTiles.RemoveAt(i);
            }
        }
        #endregion

        #region Player Action Tiles / ChooseActionPhase
        public void PaintActionTiles(List<MapTile> tiles)
        {
            foreach (var tile in tiles)
            {
                tile.SetOverlay(ActionTileColor);
                // tile.MakeAttackTarget();
                _actionTiles.Add(tile);
            }
        }

        public void ClearActionTiles()
        {
            for (int i = _actionTiles.Count - 1; i >= 0; i--)
            {
                _actionTiles[i].ClearOverlays(true, false, false);
                // _actionTiles[i].ClearOverlay(Overlay.AttackTarget);
                _actionTiles.RemoveAt(i);
            }
        }
        #endregion

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