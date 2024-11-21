namespace Duelo.Gameboard
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public enum DebugOverlays
    {
        MoveTarget,
        AttackTarget
    }

    public enum ArrowOverlays
    {
        TileSelected,
        ArrowOrigin,
        ArrowLine,
        ArrowBend,
        ArrowHead
    }

    public class GridTile : MonoBehaviour, ITraversable<GridTile>
    {
        #region Static Methods
        public static GridTile LookForTileBeneathObject(GameObject target)
        {
            if (Physics.Raycast(target.transform.position, Vector3.down, out var hit, 2.0f))
            {
                return hit.collider.GetComponent<GridTile>();
            }

            return null;
        }

        public static Color MovableTileColor = new Color(0f, 1f, 0f, 0.5f);
        #endregion

        #region Overlays
        private Dictionary<ArrowOverlays, GameObject> _arrowOverlays = new Dictionary<ArrowOverlays, GameObject>();
        private Dictionary<DebugOverlays, GameObject> _debugOverlays = new Dictionary<DebugOverlays, GameObject>();
        private GameObject _colorOverlay;
        #endregion

        #region Public Properties
        public float LerpSpeed = 2.0f;
        public bool IsChallengerSpawn;
        public bool IsDefenderSpawn;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _colorOverlay = transform.Find("ColorOverlay").gameObject;
            _debugOverlays.Add(DebugOverlays.MoveTarget, transform.Find("DebugMoveOverlay").gameObject);
            _debugOverlays.Add(DebugOverlays.AttackTarget, transform.Find("DebugAttackOverlay").gameObject);

            // ! IMPORTANT NOTE: Each of these overlays are child objects of the tile prefab this script is attached to.
            // When raycasting to find the tile beneath a given object, these layers are ignored because
            // they are contained in the ignoreRaycasting layer.
            _arrowOverlays.Add(ArrowOverlays.TileSelected, transform.Find("TileSelectionOverlay").gameObject);
            _arrowOverlays.Add(ArrowOverlays.ArrowHead, transform.Find("ArrowHeadOverlay").gameObject);
            _arrowOverlays.Add(ArrowOverlays.ArrowBend, transform.Find("ArrowBendOverlay").gameObject);
            _arrowOverlays.Add(ArrowOverlays.ArrowLine, transform.Find("ArrowLineOverlay").gameObject);
            _arrowOverlays.Add(ArrowOverlays.ArrowOrigin, transform.Find("ArrowOriginOverlay").gameObject);
        }
        #endregion

        #region Public Methods
        public void MakeMoveTarget()
        {
            SetOverlay(DebugOverlays.MoveTarget, true);
        }

        public void MakeAttackTarget()
        {
            SetOverlay(DebugOverlays.AttackTarget);
        }

        public void SetOverlay(Color color, bool isActive = true)
        {
            _colorOverlay.gameObject.SetActive(isActive);
            var material = _colorOverlay.GetComponent<Renderer>().material;
            material.color = color;
            material.SetColor("_EmissionColor", color);
        }

        public void SetOverlay(DebugOverlays overlay, bool isActive = true)
        {
            _debugOverlays[overlay].gameObject.SetActive(isActive);
        }

        public void SetOverlay(ArrowOverlays overlay, bool isActive = true)
        {
            _arrowOverlays[overlay].gameObject.SetActive(isActive);
        }

        public void OverlayLookAt(ArrowOverlays overlay, Vector3 actualTarget)
        {
            actualTarget.y = transform.position.y;
            var direction = Vector3.Normalize(actualTarget - this.transform.position);
            OverlayRotation(overlay, direction);
        }

        public void OverlayRotation(ArrowOverlays overlay, Vector3 direction)
        {
            var overlayObject = _arrowOverlays[overlay];
            overlayObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        public void ClearOverlays(bool clearColorOverlay = true, bool clearDebugOverlays = true, bool clearArrowOverlays = true)
        {
            if (clearColorOverlay)
            {
                _colorOverlay.SetActive(false);
            }

            if (clearDebugOverlays)
            {
                foreach (DebugOverlays item in Enum.GetValues(typeof(DebugOverlays)))
                {
                    SetOverlay(item, false);
                }
            }

            if (clearArrowOverlays)
            {
                foreach (ArrowOverlays item in Enum.GetValues(typeof(ArrowOverlays)))
                {
                    SetOverlay(item, false);
                }
            }
        }

        public T LookForObjectAboveTile<T>() where T : MonoBehaviour
        {
            if (Physics.Raycast(transform.position, Vector3.up, out var hit, 2.0f))
            {
                return hit.collider.GetComponent<T>();
            }

            return null;
        }

        public GameObject LookForObjectAboveTile(string tagName)
        {
            if (Physics.Raycast(transform.position, Vector3.up, out var hit, 2.0f))
            {
                if (hit.collider.CompareTag(tagName))
                {
                    return hit.collider.gameObject;
                }
            }

            return null;
        }
        #endregion

        #region Pathfinding
        public List<GridTile> GetNeighbors(bool includeUnreachable, bool includeDiagonals)
        {
            var neighbors = new List<GridTile>();
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
        public GridTile GetNeighboringNode(Vector3 direction, bool includeBlockedTiles)
        {
            Vector3 halfExtents = new Vector3(0.25f, (1) / 2.0f, 0.25f);
            Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtents);

            foreach (Collider item in colliders)
            {
                GridTile tile = item.GetComponent<GridTile>();
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

        public float DistanceTo(GridTile other)
        {
            return Vector3.Distance(this.transform.position, other.transform.position);
        }
        #endregion

        #region C# Overrides
#if UNITY_EDITOR
        /// <summary>
        /// Allows us to show a nice coordinate in the debugging UI
        /// </summary>
        public override string ToString()
        {
            return $"[{transform?.position.x}, {transform?.position.y}, {transform?.position.z}]";
        }
#endif
        #endregion
    }
}