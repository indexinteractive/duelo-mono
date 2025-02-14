namespace Duelo.Client.UI
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;
    using Duelo.Common.Player;

    public class PlayerStatusBar : MonoBehaviour
    {
        #region References
        [Tooltip("Parent grid transform for the hearts")]
        public Transform HeartsGrid;

        [Tooltip("Prefab used to spawn a heart")]
        public HeartContainer HeartPrefab;

        public Text Gamertag;
        public Image PlayerAvatar;
        #endregion

        #region Private Fields
        private List<HeartContainer> hearts = new List<HeartContainer>();
        #endregion

        #region Initialization
        public void SetPlayerInfo(string gamertag, PlayerTraits traits)
        {
            Gamertag.text = gamertag;
            PlayerAvatar.sprite = traits.Avatar;

            int hitsPerHeart = 2;
            InitializeHearts(traits.BaseHealth / hitsPerHeart);
        }

        private void InitializeHearts(int maxHearts)
        {
            foreach (var heart in hearts)
            {
                Destroy(heart.gameObject);
            }

            hearts.Clear();

            for (int i = 0; i < maxHearts; i++)
            {
                HeartContainer newHeart = Instantiate(HeartPrefab, HeartsGrid);
                newHeart.UpdateHeartDisplay(HeartContainer.HeartState.Full);
                hearts.Add(newHeart);
            }
        }
        #endregion

        #region Helpers
        public void UpdateHealth(int halfHearts)
        {
            for (int i = 0; i < hearts.Count; i++)
            {
                if (halfHearts >= (i + 1) * 2)
                {
                    hearts[i].UpdateHeartDisplay(HeartContainer.HeartState.Full);
                }
                else if (halfHearts == (i * 2) + 1)
                {
                    hearts[i].UpdateHeartDisplay(HeartContainer.HeartState.Half);
                }
                else
                {
                    hearts[i].UpdateHeartDisplay(HeartContainer.HeartState.Empty);
                }
            }
        }
        #endregion
    }
}