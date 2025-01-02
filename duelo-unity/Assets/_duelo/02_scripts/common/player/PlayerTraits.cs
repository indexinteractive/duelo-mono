namespace Duelo.Common.Player
{
    using UnityEngine;

    /// <summary>
    /// Belongs to a <see cref="Common.Match.MatchPlayer"/> and is used by <see cref="Common.Component.HurtboxComponent"/>
    /// to calculate the damage dealt by the player.
    /// </summary>
    public class PlayerTraits : MonoBehaviour
    {
        [Tooltip("Base health of the player")]
        public int BaseHealth = 6;
        [Tooltip("Base strength of the player")]
        public int BaseStrength = 1;
        [Tooltip("Base movement range of the player")]
        public int MovementRange = 3;
    }
}