namespace Duelo.Common.Service
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    public class DeviceService : FirebaseService<DeviceService>
    {
        public async UniTask<DueloPlayerDto> GetDevicePlayer()
        {
            try
            {
#if DUELO_LOCAL
                string unityPlayerId = GameData.StartupOptions?.PlayerIdOverride;
#else
                string unityPlayerId = Unity.Services.Authentication.AuthenticationService.Instance.PlayerId;
#endif
                DueloPlayerDto dto = null;

                if (unityPlayerId != null)
                {
                    Debug.Log($"[DeviceService] User is signed in: {unityPlayerId}");
                    dto = await FetchPlayerDto(unityPlayerId);
                }

                if (dto == null)
                {
                    Debug.Log("[DeviceService] No player found for this device");
                    dto = await CreatePlayer(unityPlayerId);
                }

                return dto;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DeviceService] Error in GetDevicePlayer: {ex.Message}");
                throw;
            }
        }

        private async UniTask<DueloPlayerDto> FetchPlayerDto(string userId)
        {
            var playerRef = GetRef(DueloCollection.Player, userId);
            var dataSnapshot = await playerRef.GetValueAsync().AsUniTask();

            if (!dataSnapshot.Exists)
            {
                Debug.Log($"[DeviceService] Player not found for userId: {userId}");
                return null;
            }

            string json = dataSnapshot.GetRawJsonValue();
            return JsonConvert.DeserializeObject<DueloPlayerDto>(json);
        }

        private async UniTask<DueloPlayerDto> CreatePlayer(string uid)
        {
#if DUELO_LOCAL
            /// <summary>
            /// The override below is intended to match the values created by the server during
            /// local testing: <see cref="Server.State.StateRunServerMatch.CreateTestMatchmakingResults"/>
            /// </summary>
            uid = GameData.StartupOptions.PlayerIdOverride;
#endif

            var data = new DueloPlayerDto()
            {
                UnityPlayerId = uid,
                Profiles = new System.Collections.Generic.Dictionary<string, PlayerProfileDto>()
            };

            string json = JsonConvert.SerializeObject(data);

            var playerRef = GetRef(DueloCollection.Player, uid);
            await playerRef.SetRawJsonValueAsync(json);

            Debug.Log($"[DeviceService] Anonymous user created with playerId: {uid}");

            return data;
        }

        public async UniTask SetActiveProfile(string playerId, string profileId)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Player, playerId, "activeProfileid");
                await dbRef.SetValueAsync(profileId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DeviceService] Error in SetActiveProfile: {ex.Message}");
                throw;
            }
        }

        public async UniTask<PlayerProfileDto> CreateProfile(string playerId, string gamertag, string characterId)
        {
            try
            {
                var profileRef = GetRef(DueloCollection.Player, playerId, "profiles").Push();
                var id = profileRef.Key;

                var profile = new PlayerProfileDto
                {
                    Id = id,
                    Gamertag = gamertag,
                    CharacterUnitId = characterId
                };

                string json = JsonConvert.SerializeObject(profile);
                await profileRef.SetRawJsonValueAsync(json);

                return profile;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[DeviceService] Error in CreateProfile: {ex.Message}");
                throw;
            }
        }
    }
}
