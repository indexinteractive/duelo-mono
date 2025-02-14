namespace Duelo.Common.Player
{
    using System;
    using UnityEngine;

    [Serializable]
    public class UnitPerkDto
    {
        public string Name;
        public string Description;
    }

    /// <summary>
    /// Belongs to a <see cref="Common.Match.MatchPlayer"/> and is used by <see cref="Common.Component.HurtboxComponent"/>
    /// to calculate the damage dealt by the player.
    /// </summary>
    public class PlayerTraits : MonoBehaviour
    {
        [Header("Character Details")]
        [Tooltip("Unique identifier of this character")]
        public string CharacterId;
        [Tooltip("Name of the character")]
        public string CharacterName;
        [Tooltip("Player avatar")]
        public Sprite Avatar;

        [Header("Power")]
        [Tooltip("Base health of the player")]
        public int BaseHealth = 6;
        [Tooltip("Base strength of the player")]
        public int BaseStrength = 1;

        [Header("Movement")]
        [Tooltip("Model that will be shown as a preview when choosing movement")]
        public GameObject GhostPrefab;
        [Tooltip("Base movement range of the player")]
        public int BaseMovementRange = 3;
        [Tooltip("Base speed of the player")]
        public int BaseSpeed;

        [Header("Attack")]
        [Tooltip("Base attack range of the player")]
        public int BaseAttackRange;

        public PlayerActionItemDto[] Attacks;

        [Header("Defense")]
        [Tooltip("Prefab used for the defense ring")]
        public GameObject DefenseRingPrefab;

        public PlayerActionItemDto[] Defenses;

        // TODO: Perks should probably be scriptable objects or similar
        [Tooltip("Base defense of the player")]
        public UnitPerkDto Perk;
    }
}