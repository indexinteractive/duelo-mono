namespace Duelo.Client.Camera
{
    using UnityEngine;
    using Unity.Cinemachine;
    using System.Collections.Generic;
    using Duelo.Common.Model;
    using Duelo.Common.Match;

    public enum CameraMode
    {
        PlayerFocus,
        Midpoint,
        Cutscene
    }

    public class DueloCamera : MonoBehaviour
    {
        private CameraMode currentMode;

        [Header("Cameras")]
        [HideInInspector]
        public Camera RootCamera;
        public CinemachineCamera PlayerCamera;
        public CinemachineCamera MidpointCamera;
        public CinemachineCamera CutsceneCamera;

        [Header("Tracking")]
        public CinemachineTargetGroup PlayerTrackingGroup;
        public Transform MapCenterTarget;

        [Header("Player Follow Mode")]
        public float MinHeight = 1f;

        [Header("Cutscene Mode")]
        public Transform[] cutscenePath;
        public float cutsceneSpeed = 3f;

        private int cutsceneIndex;
        private bool isCutscenePlaying;

        #region Private Fields
        private Dictionary<PlayerRole, MatchPlayer> _players = new();
        private PlayerRole _targetPlayer;
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            _targetPlayer = PlayerRole.Challenger;
            RootCamera = GetComponentInChildren<Camera>();
            SetCameraMode(CameraMode.PlayerFocus);
        }

        private void Update()
        {
            HandleInput();
            if (currentMode == CameraMode.Cutscene && isCutscenePlaying)
            {
                MoveAlongCutscenePath();
            }
            else if (currentMode == CameraMode.Midpoint)
            {
                UpdateMidpoint();
            }
        }
        #endregion

        #region Input Handling
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetCameraMode(CameraMode.PlayerFocus);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetCameraMode(CameraMode.Midpoint);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SetCameraMode(CameraMode.Cutscene);
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchOrbitFocus();
            }
        }
        #endregion

        #region Camera Modes
        private void SetCameraMode(CameraMode mode)
        {
            currentMode = mode;

            PlayerCamera.gameObject.SetActive(false);
            MidpointCamera.gameObject.SetActive(false);
            CutsceneCamera.gameObject.SetActive(false);

            switch (mode)
            {
                case CameraMode.PlayerFocus:
                    PlayerCamera.gameObject.SetActive(true);
                    SetOrbitCameraTarget(_targetPlayer);
                    break;
                case CameraMode.Midpoint:
                    MidpointCamera.gameObject.SetActive(true);
                    break;
                case CameraMode.Cutscene:
                    CutsceneCamera.gameObject.SetActive(true);
                    StartCutscene();
                    break;
            }
        }

        public void SetMapCenter(Vector3 mapCenter)
        {
            MapCenterTarget.transform.position = mapCenter;
        }
        #endregion

        #region Orbiting
        private void SwitchOrbitFocus()
        {
            _targetPlayer = (_targetPlayer == PlayerRole.Challenger)
                ? PlayerRole.Defender
                : PlayerRole.Challenger;

            if (currentMode == CameraMode.PlayerFocus)
            {
                SetOrbitCameraTarget(_targetPlayer);
            }
        }

        private void SetOrbitCameraTarget(PlayerRole role)
        {
            if (!_players.ContainsKey(role))
            {
                return;
            }

            var target = _players[role].transform;
            PlayerCamera.LookAt = target;
            PlayerCamera.Follow = target;
        }
        #endregion

        #region Player Tracking
        public void FollowPlayers(Dictionary<PlayerRole, MatchPlayer> players)
        {
            _players = players;
            SetCameraMode(CameraMode.Midpoint);

            PlayerTrackingGroup.AddMember(players[PlayerRole.Challenger].transform, 1, 1);
            PlayerTrackingGroup.AddMember(players[PlayerRole.Defender].transform, 1, 1);
        }

        private void UpdateMidpoint()
        {
            if (_players.ContainsKey(PlayerRole.Defender) && _players.ContainsKey(PlayerRole.Challenger))
            {
                if (_players[PlayerRole.Defender] == null || _players[PlayerRole.Challenger] == null)
                {
                    return;
                }

                var pointA = _players[PlayerRole.Defender].transform;
                var pointB = _players[PlayerRole.Challenger].transform;

                var midpoint = new Vector3(
                    (pointA.position.x + pointB.position.x) / 2,
                    Mathf.Max((pointA.position.y + pointB.position.y) / 2, MinHeight),
                    (pointA.position.z + pointB.position.z) / 2
                );

                Debug.DrawLine(pointA.position, midpoint, Color.red);
                Debug.DrawLine(pointB.position, midpoint, Color.blue);
            }
        }
        #endregion

        #region Cutscene Mode
        private void StartCutscene()
        {
            cutsceneIndex = 0;
            isCutscenePlaying = true;
            if (cutscenePath.Length > 0)
            {
                CutsceneCamera.transform.position = cutscenePath[0].position;
                CutsceneCamera.transform.rotation = cutscenePath[0].rotation;
            }
        }

        private void MoveAlongCutscenePath()
        {
            if (cutsceneIndex >= cutscenePath.Length) return;

            Transform targetPoint = cutscenePath[cutsceneIndex];
            CutsceneCamera.transform.position = Vector3.MoveTowards(
                CutsceneCamera.transform.position,
                targetPoint.position,
                cutsceneSpeed * Time.deltaTime
            );

            CutsceneCamera.transform.rotation = Quaternion.RotateTowards(
                CutsceneCamera.transform.rotation,
                targetPoint.rotation,
                cutsceneSpeed * 10f * Time.deltaTime
            );

            if (Vector3.Distance(CutsceneCamera.transform.position, targetPoint.position) < 0.1f)
            {
                cutsceneIndex++;
            }

            if (cutsceneIndex >= cutscenePath.Length)
            {
                isCutscenePlaying = false;
            }
        }
        #endregion
    }
}