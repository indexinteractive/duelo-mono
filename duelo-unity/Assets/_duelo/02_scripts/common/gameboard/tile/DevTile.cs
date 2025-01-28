namespace Duelo.Gameboard
{
    using System;
    using UnityEngine;

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
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _colorOverlay = transform.Find("ColorOverlay").gameObject;
            _overlays.Add(Overlay.MoveTarget, transform.Find("DebugMoveOverlay").gameObject);
            _overlays.Add(Overlay.AttackTarget, transform.Find("DebugAttackOverlay").gameObject);

            // ! IMPORTANT NOTE: Each of these overlays are child objects of the tile prefab this script is attached to.
            // When raycasting to find the tile beneath a given object, these layers are ignored because
            // they are contained in the ignoreRaycasting layer.
            _overlays.Add(Overlay.Selected, transform.Find("TileSelectionOverlay").gameObject);
            _overlays.Add(Overlay.End, transform.Find("ArrowHeadOverlay").gameObject);
            _overlays.Add(Overlay.Bend, transform.Find("ArrowBendOverlay").gameObject);
            _overlays.Add(Overlay.Straight, transform.Find("ArrowLineOverlay").gameObject);
            _overlays.Add(Overlay.Origin, transform.Find("ArrowOriginOverlay").gameObject);
        }
        #endregion

        #region Tile Helpers
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