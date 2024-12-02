namespace Duelo.Common.Kernel
{
    using Duelo.Server.Match;
    using UnityEngine;

    /// <summary>
    /// Base class for all movement actions that can be added to a player's <see cref="MatchPlayer.ActionQueue"/>.
    /// </summary>
    public abstract partial class MovementDescriptor : ActionDescriptor
    {
        public readonly Vector3 Destination;

        public MovementDescriptor(Vector3 destination)
        {
            Destination = destination;
        }
    }
}