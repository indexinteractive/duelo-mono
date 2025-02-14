namespace Duelo.Client.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Assertions;

    public class HeartContainer : MonoBehaviour
    {
        public enum HeartState
        {
            Full,
            Half,
            Empty
        }

        #region References
        public Image FullHeart;
        public Image HalfHeart;
        public Image EmptyHeart;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            Assert.IsNotNull(FullHeart, $"[HeartContainer] {name}: FullHeart is not assigned");
            Assert.IsNotNull(HalfHeart, $"[HeartContainer] {name}: HalfHeart is not assigned");
            Assert.IsNotNull(EmptyHeart, $"[HeartContainer] {name}: EmptyHeart is not assigned");
        }
        #endregion

        public void UpdateHeartDisplay(HeartState state)
        {
            FullHeart.gameObject.SetActive(state == HeartState.Full);
            HalfHeart.gameObject.SetActive(state == HeartState.Half);
            EmptyHeart.gameObject.SetActive(state == HeartState.Empty);
        }
    }
}