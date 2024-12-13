using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Duelo.Common.Component
{
    public class ProjectileAttackAction : GameAction
    {
        #region Private Fields
        private ProjectileWeaponComponent[] _projectileComponents;
        private bool _isFinished;
        #endregion

        #region ActionComponent Implementation
        public override bool IsFinished => _isFinished;

        public override void OnActionMounted()
        {
            UniTask.Delay(250)
                .ContinueWith(() =>
                {
                    foreach (var projectileComponent in _projectileComponents)
                    {
                        projectileComponent.gameObject.SetActive(true);
                    }
                })
                .ContinueWith(() => UniTask.Delay(1000))
                .ContinueWith(() =>
                {
                    foreach (var projectileComponent in _projectileComponents)
                    {
                        projectileComponent.Fire();
                    }
                })
                .ContinueWith(() => UniTask.Delay(1000))
                .ContinueWith(() =>
                {
                    foreach (var projectileComponent in _projectileComponents)
                    {
                        projectileComponent.gameObject.SetActive(false);
                    }
                })
                .ContinueWith(() => UniTask.Delay(1000))
                .ContinueWith(() => _isFinished = true);
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _projectileComponents = GetComponentsInChildren<ProjectileWeaponComponent>(true);
            Debug.Log("[ProjectileAttackAction] ProjectileWeaponComponent count: " + _projectileComponents.Length);
        }
        #endregion
    }
}