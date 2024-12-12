namespace Duelo.Common.Component
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Util;
    using Ind3x.Util;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class CloseRangeAttackAction : GameAction
    {
        #region Private Fields
        private bool _punched = false;
        private Animator _animator;
        #endregion

        #region ActionComponent Implementation
        public override bool IsFinished => _punched;

        public override void OnActionMounted()
        {
            _animator.SetTrigger(PlayerAnimation.AttackCloseRange);

            AnimationHelper
                .WaitForAnimationComplete(PlayerAnimation.AttackCloseRange, _animator)
                .ContinueWith(OnAnimationFinished);
        }

        public override void OnActionRemoved() { }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            Assert.IsNotNull(_animator, "[GameRoundPlayer] Animator component was not found");
        }
        #endregion

        #region Events
        public void OnAnimationFinished()
        {
            _punched = true;
        }
        #endregion
    }
}