namespace Duelo.Client.UI
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.UI;

    public class PlayerStatusBar : MonoBehaviour
    {
        #region References
        [Tooltip("Parent grid transform for the hearts")]
        public Transform HeartsGrid;

        [Tooltip("Prefab used to spawn a heart")]
        public HeartContainer HeartPrefab;

        [Tooltip("Text that displays the player's gamertag")]
        public Text Gamertag;
        #endregion

        #region Private Fields
        private int _maxHearts = 3;
        private List<HeartContainer> hearts = new List<HeartContainer>();
        #endregion

        #region Initialization
        public void SetPlayerInfo(string gamertag, int maxHits)
        {
            Gamertag.text = gamertag;
            _maxHearts = maxHits / 2;
            InitializeHearts();
        }

        private void InitializeHearts()
        {
            foreach (var heart in hearts)
            {
                Destroy(heart.gameObject);
            }

            hearts.Clear();

            for (int i = 0; i < _maxHearts; i++)
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