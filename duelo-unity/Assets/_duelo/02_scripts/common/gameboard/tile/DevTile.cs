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

    public enum PathIndicator
    {
        Selected,
        Origin,
        Straight,
        Bend,
        End
    }

    public class DevTile : MapTile
    {
        #region Static Methods
        public static MapTile LookForTileBeneathObject(GameObject target)
        {
            if (Physics.Raycast(target.transform.position, Vector3.down, out var hit, 2.0f))
            {
                return hit.collider.GetComponent<MapTile>();
            }

            return null;
        }

        public static Color MovableTileColor = new Color(0f, 1f, 0f, 0.5f);
        #endregion

        #region Overlays
        private Dictionary<PathIndicator, GameObject> _arrowOverlays = new Dictionary<PathIndicator, GameObject>();
        private Dictionary<DebugOverlays, GameObject> _debugOverlays = new Dictionary<DebugOverlays, GameObject>();
        private GameObject _colorOverlay;
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
            _arrowOverlays.Add(PathIndicator.Selected, transform.Find("TileSelectionOverlay").gameObject);
            _arrowOverlays.Add(PathIndicator.End, transform.Find("ArrowHeadOverlay").gameObject);
            _arrowOverlays.Add(PathIndicator.Bend, transform.Find("ArrowBendOverlay").gameObject);
            _arrowOverlays.Add(PathIndicator.Straight, transform.Find("ArrowLineOverlay").gameObject);
            _arrowOverlays.Add(PathIndicator.Origin, transform.Find("ArrowOriginOverlay").gameObject);
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
        #endregion

        #region Overlays
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

        public override void SetOverlay(PathIndicator overlay, bool isActive = true)
        {
            _arrowOverlays[overlay].gameObject.SetActive(isActive);
        }

        public override void OverlayLookAt(PathIndicator overlay, Vector3 actualTarget)
        {
            actualTarget.y = transform.position.y;
            var direction = Vector3.Normalize(actualTarget - this.transform.position);
            OverlayRotation(overlay, direction);
        }

        public override void OverlayRotation(PathIndicator overlay, Vector3 direction)
        {
            var overlayObject = _arrowOverlays[overlay];
            overlayObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        public override void ClearOverlays(bool clearColorOverlay = true, bool clearDebugOverlays = true, bool clearArrowOverlays = true)
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
                foreach (PathIndicator item in Enum.GetValues(typeof(PathIndicator)))
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