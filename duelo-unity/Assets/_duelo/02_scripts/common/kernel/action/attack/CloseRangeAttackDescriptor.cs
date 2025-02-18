namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Component;
    using Duelo.Common.Player;
    using UnityEngine;

    [CreateAssetMenu(fileName = "CloseRangeAttack", menuName = "Duelo/Actions/Attack/CloseRangeAttack")]
    public class CloseRangeAttackDescriptor : ActionDescriptor
    {
        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(CloseRangeAttackAction);

        public override IEnumerable<Vector3> GetAttackRangeTiles(PlayerTraits traits, Vector3 origin)
        {
            var range = Mathf.Max(traits.BaseAttackRange, 1);
            HashSet<Vector3> attackPositions = new() { origin };

            for (int i = 1; i <= range; i++)
            {
                attackPositions.Add(new Vector3(origin.x + i, origin.y, origin.z));
                attackPositions.Add(new Vector3(origin.x - i, origin.y, origin.z));
                attackPositions.Add(new Vector3(origin.x, origin.y, origin.z + i));
                attackPositions.Add(new Vector3(origin.x, origin.y, origin.z - i));
            }

            return attackPositions;
        }
        #endregion
    }
}