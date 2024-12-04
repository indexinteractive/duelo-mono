namespace Duelo.Common.Component
{
    using UnityEngine;

    public enum VelocityType
    {
        /// <summary>
        /// PerStep mode will move the entity at a constant speed per step,
        /// regardless of the distance to the target.
        /// </summary>
        PerStep,

        /// <summary>
        /// TotalTravelTime mode will use a total time value that it should take the entity
        /// to reach the target. It will then divide that total time by the distance
        /// to the target to calculate the speed.
        /// </summary>
        TotalTravelTime
    }

    public partial class VelocityComponent : MonoBehaviour
    {
        #region Public Properties
        /// <summary>
        /// TotalTravelTime: Time to reach the target.
        /// PerStep: Speed of the entity at each grid step
        /// </summary>
        public VelocityType Mode = VelocityType.PerStep;

        [Range(1f, 10f)]
        public float SpeedPerStep = 5f;

        [Range(2, 10)]
        public float TotalTimeSeconds = 2.0f;
        #endregion

        public float CalculateStepDuration(int pathLength)
        {
            if (Mode == VelocityType.PerStep)
            {
                return SpeedPerStep;
            }
            else
            {
                return TotalTimeSeconds / pathLength;
            }
        }
    }
}