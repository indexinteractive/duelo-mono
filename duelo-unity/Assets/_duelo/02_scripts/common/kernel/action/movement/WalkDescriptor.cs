namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Component;
    using Duelo.Common.Player;
    using UnityEngine;

    /// <summary>
    /// Descriptor for the <see cref="Common.Component.WalkGameAction"/>
    /// </summary>
    [CreateAssetMenu(fileName = "WalkMovement", menuName = "Duelo/Actions/Movement/WalkMovement")]
    public class WalkDescriptor : ActionDescriptor
    {
        #region WalkDescriptor Properties
        public readonly Vector3 Destination;
        #endregion

        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(WalkGameAction);

        public override IEnumerable<Vector3> GetMovablePositions(PlayerTraits traits, Vector3 origin)
        {
            var range = traits.BaseMovementRange;
            var tiles = new List<Vector3>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    var tile = new Vector3(origin.x + x, origin.y, origin.z + z);
                    if (Vector3.Distance(origin, tile) <= range)
                    {
                        tiles.Add(tile);
                    }
                }
            }

            return tiles;
        }
        #endregion
    }
}
