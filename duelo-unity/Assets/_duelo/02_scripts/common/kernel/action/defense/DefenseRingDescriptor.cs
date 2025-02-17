namespace Duelo.Common.Kernel
{
    using System;
    using Duelo.Common.Component;
    using Duelo.Common.Match;
    using UnityEngine;

    [CreateAssetMenu(fileName = "DefenseRing", menuName = "Duelo/Actions/Defense/DefenseRing")]
    public class DefenseRingDescriptor : ActionDescriptor
    {
        #region Editor Properties
        [Header("Action Properties")]
        [Tooltip("Prefab used for the defense ring")]
        public GameObject DefenseRingPrefab;

        [Tooltip("Will be used to adjust the size of the ring when scaling to the collider bounds")]
        public Vector3 ScaleModifier = new Vector3(1, 1, 1);
        #endregion

        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(DefenseRingAction);

        public override GameAction AddComponentTo(MatchPlayer target)
        {
            var action = base.AddComponentTo(target) as DefenseRingAction;
            action.RingPrefab = DefenseRingPrefab;
            action.ScaleModifier = ScaleModifier;
            return action;
        }
        #endregion
    }
}