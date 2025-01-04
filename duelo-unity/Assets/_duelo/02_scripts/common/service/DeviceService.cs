namespace Duelo.Common.Service
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Firebase.Auth;
    using Newtonsoft.Json;
    using UnityEngine;

    public class DeviceService : FirebaseService<DeviceService>
    {
        private readonly FirebaseAuth _firebaseAuth;

        public DeviceService()
        {
            _firebaseAuth = FirebaseAuth.DefaultInstance;
        }

        public async UniTask<DueloPlayerDto> GetDevicePlayer()
        {
            try
            {
                var user = _firebaseAuth.CurrentUser;

                if (user != null)
                {
                    Debug.Log($"[DeviceService] User is signed in: {user.UserId}");
                    return await FetchPlayerDto(user.UserId);
                }
                else
                {
                    Debug.Log("[DeviceService] No player found for this device");
                    var credential = await _firebaseAuth.SignInAnonymouslyAsync().AsUniTask();

                    return await CreatePlayer(credential.User.UserId);
                }
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
            var data = new DueloPlayerDto()
            {
                PlayerId = uid,
                Profiles = new System.Collections.Generic.Dictionary<string, PlayerProfileDto>()
            };

            string json = JsonConvert.SerializeObject(data);

            var playerRef = GetRef(DueloCollection.Player, uid);
            await playerRef.SetRawJsonValueAsync(json);

            Debug.Log($"[DeviceService] Anonymous user created with playerId: {uid}");

            return data;
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
