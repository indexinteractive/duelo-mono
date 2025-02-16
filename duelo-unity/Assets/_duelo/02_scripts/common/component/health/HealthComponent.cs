namespace Duelo.Common.Component
{
    using UnityEngine;
    using Duelo.Common.Player;
    using Duelo.Common.Model;
    using Duelo.Common.Match;
    using Duelo.Common.Core;
    using Firebase.Database;
    using Newtonsoft.Json;
    using Cysharp.Threading.Tasks;

    [RequireComponent(typeof(PlayerTraits))]
    public class HealthComponent : MonoBehaviour
    {
        #region Components
        private PlayerTraits _traits;
        private MatchPlayer _player;
        #endregion

        #region Public Properties
        public float Health { get; private set; }
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            _traits = GetComponent<PlayerTraits>();
            _player = GetComponent<MatchPlayer>();

            // Initialize health based on player traits
            Health = _traits.BaseHealth;
            Debug.Log($"[HealthComponent] {name} initialized with {Health} health");

            if (GlobalState.MatchRef != null)
            {
                GlobalState.MatchRef.Child("rounds").ValueChanged += OnRoundsUpdated;
            }
        }

        private void OnDestroy()
        {
            if (GlobalState.MatchRef != null)
            {
                GlobalState.MatchRef.Child("rounds").ValueChanged -= OnRoundsUpdated;
            }
        }
        #endregion

        #region Public Methods
        public void Damage(AttackData attack)
        {
            Health -= attack.AttackDamage;
            Debug.Log($"[HealthComponent] {name} took {attack.AttackDamage} damage and is now at {Health} health");

            _player.PublishHealth(Health)
                .ContinueWith(() => Debug.Log($"[HealthComponent] Published health to Firebase"));

            if (Health <= 0)
            {
                // TODO: Broadcast event to notify other components of death
                // Destroy(gameObject);
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Firebase
        private void OnRoundsUpdated(object sender, ValueChangedEventArgs e)
        {
            if (e.DatabaseError != null)
            {
                Debug.LogError($"[HealthComponent] Error: {e.DatabaseError.Message}");
                return;
            }

            string json = e.Snapshot?.GetRawJsonValue() ?? string.Empty;

            if (string.IsNullOrEmpty(json))
            {
                Debug.LogWarning("[HealthComponent] No round data to deserialize");
                return;
            }

            Debug.Log($"[HealthComponent] Rounds updated: {json}");

            try
            {
                var rounds = JsonConvert.DeserializeObject<MatchRoundDto[]>(json);
                Debug.Log($"[HealthComponent] Rounds count: {rounds.Length}");

                MatchRoundDto lastRound = rounds[rounds.Length - 1];

                Debug.Log($"[HealthComponent] Last round: {lastRound?.RoundNumber}");

                if (_player.Role == PlayerRole.Challenger)
                {
                    if (lastRound.PlayerState?.Challenger.Health != null)
                    {
                        Health = lastRound.PlayerState.Challenger.Health;
                        Debug.Log($"[HealthComponent] Updated health to {Health}");
                    }
                }
                else if (_player.Role == PlayerRole.Defender)
                {
                    if (lastRound.PlayerState?.Defender.Health != null)
                    {
                        Health = lastRound.PlayerState.Defender.Health;
                        Debug.Log($"[HealthComponent] Updated health to {Health}");
                    }
                }
            }
            catch (System.Exception error)
            {
                Debug.LogError($"[HealthComponent] Error: {error.Message}");
            }
        }
        #endregion
    }
}
