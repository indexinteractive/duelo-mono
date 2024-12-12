namespace Ind3x.Util
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class AnimationHelper
    {
        /// <summary>
        /// This method waits for an animation to finish by checking the current animation and flipping two flags:
        ///
        /// 1. if the current animation tag matches expectation, animation has started
        /// 2. if the animation has started and we see the tag change again, the animation has ended
        ///
        /// ---- Caveats! ----
        /// - This will not work on animations that loop
        /// - This will not work on animations that do NOT transition to another state, like Player:Death
        ///
        /// </summary>
        public static async UniTask WaitForAnimationChange(string animationTag, Animator animator)
        {
            bool animationStarted = false;
            bool animationEnded = false;

            while (!(animationStarted && animationEnded))
            {
                if (animator == null)
                {
                    return;
                }

                var state = animator.GetCurrentAnimatorStateInfo(0);
                if (state.IsTag(animationTag))
                {
                    animationStarted = true;
                }

                if (animationStarted && !state.IsTag(animationTag))
                {
                    animationEnded = true;
                }

                await UniTask.Yield();
            }
        }

        /// <summary>
        /// Waits for an animation to finish by checking the tag and play time.
        ///
        /// This method will ONLY work on:
        /// - animations that transition but are intended to loop more than once
        /// - animations that do not have a transition to another state
        ///
        /// The reason it won't work on animations that transition to other states
        /// is that the normalized time reaches >= 1.0 at the time of this check.
        /// By the time the animation has finished, the tag will have changed
        /// </summary>
        public static async UniTask WaitForAnimationComplete(string animationTag, Animator animator)
        {
            bool animationFinished = false;

            while (!animationFinished)
            {
                if (animator == null)
                {
                    break;
                }

                var state = animator.GetCurrentAnimatorStateInfo(0);

                animationFinished = state.IsTag(animationTag) && (state.normalizedTime >= 1.0f);

                await UniTask.Yield();
            }
        }
    }
}