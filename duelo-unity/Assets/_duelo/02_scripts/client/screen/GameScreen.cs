namespace Duelo.Client.Screen
{
    using Duelo.Client.Match;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Ind3x.State;
    using UnityEngine;

    public class GameScreen : GameState
    {
        #region Inherited Properties
        protected GameObject _gui;
        protected IClientMatch _match => GlobalState.ClientMatch;
        protected MatchPlayer _player => GlobalState.ClientMatch.DevicePlayer;
        #endregion

        #region Constants
        const float UI_Z_DEPTH = 3.0f;
        #endregion

        private void FitToScreen(Camera camera, GameObject gui)
        {
            RectTransform rectTransform = gui.GetComponent<RectTransform>();

            float depth = rectTransform.position.z - camera.transform.position.z;
            float frustumHeight = 2.0f * depth * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
            float frustumWidth = frustumHeight * camera.aspect;

            float scaleX = frustumWidth / rectTransform.rect.width;
            float scaleY = frustumHeight / rectTransform.rect.height;
            float scale = Mathf.Min(scaleX, scaleY);

            rectTransform.localScale = new Vector3(scale, scale, scale);
        }

        protected T SpawnUI<T>(string prefabLookup)
        {
            var camera = GlobalState.Camera.GetComponentInChildren<Camera>();

            _gui = GameObject.Instantiate(GlobalState.Prefabs.MenuLookup[prefabLookup]);

            _gui.transform.position = new Vector3(0, 0, UI_Z_DEPTH);
            _gui.transform.SetParent(camera.transform, false);

            // FitToScreen(camera, _gui);

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