namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Component;
    using Duelo.Common.Match;
    using Duelo.Common.Player;
    using UnityEngine;

    /// <summary>
    /// Descriptor for the <see cref="Common.Component.VelocityMovementGameAction"/>
    /// </summary>
    [CreateAssetMenu(fileName = "VelocityMovement", menuName = "Duelo/Actions/Movement/VelocityMovement")]
    public class VelocityMovementDescriptor : ActionDescriptor
    {
        #region Editor Properties
        [Header("Action Properties")]
        [Tooltip("Value that will be multiplied by the player's base speed")]
        public float SpeedMultiplier = 1;

        [Tooltip("Will be added to player.Traits.BaseMovementRange")]
        public int RangeModifier = 0;
        #endregion

        #region ActionDescriptor Implementation
        public override Type BehaviorType => typeof(VelocityMovementGameAction);

        public override GameAction AddComponentTo(MatchPlayer target)
        {
            var action = base.AddComponentTo(target) as VelocityMovementGameAction;
            action.SpeedMultiplier = SpeedMultiplier;
            return action;
        }

        public override IEnumerable<Vector3> GetMovablePositions(PlayerTraits traits, Vector3 origin)
        {
            var range = traits.BaseMovementRange + RangeModifier;
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
