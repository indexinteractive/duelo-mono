namespace Duelo.Client.Screen
{
    using Duelo.Common.Core;
    using Ind3x.State;
    using UnityEngine;

    public class GameScreen : GameState
    {
        #region Inherited Properties
        protected GameObject _gui;
        #endregion

        #region Constants
        const float UI_Z_DEPTH = 6.0f;
        #endregion

        protected virtual void FitToScreen(Camera camera, GameObject gui)
        {
            RectTransform rt = gui.GetComponent<RectTransform>();
            Vector2 lowerLeft = camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMin + rt.offsetMin));
            Vector2 upperRight = camera.WorldToScreenPoint(rt.TransformPoint(rt.anchorMax + rt.offsetMax));

            float totalWidth = Mathf.Abs(upperRight.x - lowerLeft.x);
            float totalHeight = Mathf.Abs(upperRight.y - lowerLeft.y);
            float guiScale = 1.0f;
            if (totalWidth > Screen.width)
            {
                guiScale = Screen.width / totalWidth;
            }
            if (totalHeight > Screen.height)
            {
                guiScale = Mathf.Min(Screen.height / totalHeight, guiScale);
            }

            gui.transform.localScale *= guiScale;
        }

        protected T SpawnUI<T>(string prefabLookup)
        {
            _gui = GameObject.Instantiate(ClientData.Prefabs.MenuLookup[prefabLookup]);

            _gui.transform.position = new Vector3(0, 0, UI_Z_DEPTH);
            _gui.transform.SetParent(ClientData.Camera.transform, false);

            return _gui.GetComponent<T>();
        }

        protected void ShowUI()
        {
            if (_gui != null)
            {
                _gui.SetActive(true);
            }
        }

        protected void HideUI()
        {
            if (_gui != null)
            {
                _gui.SetActive(false);
            }
        }

        protected void DestroyUI()
        {
            if (_gui != null)
            {
                GameObject.Destroy(_gui);
                _gui = null;
            }
        }

        #region Input
        public virtual void HandleUIEvent(GameObject source, object eventData) { }
        #endregion
    }
}