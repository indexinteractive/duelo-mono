namespace Duelo.Common.Kernel
{
    using System;
    using System.Collections.Generic;
    using Duelo.Common.Component;
    using Duelo.Common.Model;
    using Duelo.Common.Player;
    using UnityEngine;

    /// <summary>
    /// Descriptor for the <see cref="Common.Component.WalkGameAction"/>
    /// </summary>
    public class WalkDescriptor : ActionDescriptor
    {
        #region Initialization
        public WalkDescriptor() : base() { }
        public WalkDescriptor(Vector3 destination) : base()
        {
            Destination = destination;
        }
        #endregion

        #region WalkDescriptor Properties
        public readonly Vector3 Destination;
        #endregion

        #region ActionDescriptor Implementation
        public override int ActionId => MovementActionId.Walk;
        public override Type BehaviorType => typeof(WalkGameAction);

        public override object[] InitializationParams()
        {
            return new object[] { Destination };
        }

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
