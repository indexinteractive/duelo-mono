namespace Duelo.Client.Screen
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Service;
    using Unity.Services.Authentication;
    using Unity.Services.Core;
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
            InitializeUnityServices()
                .ContinueWith(() => UniTask.WhenAll(
                    FetchPlayerData()
                ))
                .ContinueWith(OnDevicePlayerData);
        }
        #endregion

        #region Loading
        public async UniTask InitializeUnityServices()
        {
            try
            {
                Debug.Log("[LoadScreen] Initializing Unity Services");
                await UnityServices.InitializeAsync();
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"[LoadScreen] Signed into unity services anonymously: {AuthenticationService.Instance.PlayerId}");
                }
            }
            catch (System.Exception)
            {
                Debug.Log("[LoadScreen] Unity Services failed to initialize");
                // TODO: Need error screen or offline capability
            }
        }

        public async UniTask FetchPlayerData()
        {
            var player = await DeviceService.Instance.GetDevicePlayer();
            Debug.Log($"[LoadScreen] Player data fetched: \nplayerId: {player.UnityPlayerId}\nunityPlayerId: {player.UnityPlayerId}");
            GameData.PlayerData = player;
        }

        public void OnDevicePlayerData()
        {
            if (GameData.PlayerData != null)
            {
                Debug.Log($"[LoadScreen] Received player data from firebase: {GameData.PlayerData.UnityPlayerId}");
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