namespace Duelo.Client.Screen
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Service;
    using Firebase.Database;
    using UnityEngine;

    /// <summary>
    /// Loading screen: Handles one-time startup tasks such as loading the user profile
    /// and other necessary data.
    /// At the moment has no GUI associated with it.
    /// </summary>
    public class LoadingScreen : GameScreen
    {
        #region Screen Implementation
        public override void OnEnter()
        {
            FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(true);

            UniTask.WhenAll(
                FetchPlayerData()
            ).ContinueWith(OnDevicePlayerData);
        }
        #endregion

        #region Loading
        public async UniTask FetchPlayerData()
        {
            GameData.PlayerData = await DeviceService.Instance.GetDevicePlayer();
            GameData.ActiveProfile = GameData.PlayerData.Profiles?.Count > 0
                ? GameData.PlayerData.Profiles.FirstOrDefault().Value
                : null;
        }

        public void OnDevicePlayerData()
        {
            if (GameData.PlayerData != null)
            {
                Debug.Log($"[LoadScreen] Received player data: {GameData.PlayerData.PlayerId}");
                StateMachine.SwapState(new MainMenuScreen());
            }
            else
            {
                Debug.Log("[LoadScreen] something went wrong, change to an error screen");
                // TODO: Need error screen or offline capability
                // StateMachine.SwapState(new ErrorScreen());
            }
        }
        #endregion
    }
}