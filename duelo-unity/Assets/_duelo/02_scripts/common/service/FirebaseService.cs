namespace Duelo.Common.Service
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Database;
    using Duelo.Gameboard;
    using Firebase.Database;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    public class FirebaseService : IDueloService
    {
        public DatabaseReference GetRef(string collection, params string[] path)
        {
            string collectionName = collection.ToString().ToLower();
            string pathString = string.Join("/", path);
            return FirebaseInstance.Instance.Db.GetReference(collectionName).Child(pathString);
        }

        public async UniTask<DueloPlayerDto> GetDevicePlayer()
        {
            try
            {
#if DUELO_LOCAL
                string unityPlayerId = GlobalState.StartupOptions?.PlayerIdOverride;
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
            var playerRef = GetRef(SchemaCollection.Player, userId);
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
            uid = GlobalState.StartupOptions.PlayerIdOverride;
#endif

            var data = new DueloPlayerDto()
            {
                UnityPlayerId = uid,
                Profiles = new System.Collections.Generic.Dictionary<string, PlayerProfileDto>()
            };

            string json = JsonConvert.SerializeObject(data);

            var playerRef = GetRef(SchemaCollection.Player, uid);
            await playerRef.SetRawJsonValueAsync(json);

            Debug.Log($"[DeviceService] Anonymous user created with playerId: {uid}");

            return data;
        }

        public async UniTask SetActiveProfile(string playerId, string profileId)
        {
            try
            {
                var dbRef = GetRef(SchemaCollection.Player, playerId, "activeProfileid");
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
                var profileRef = GetRef(SchemaCollection.Player, playerId, "profiles").Push();
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

        public async UniTask<DueloMapDto> GetMap(string mapId)
        {
            try
            {
                var dbRef = GetRef(SchemaCollection.Map, mapId);

                var dataSnapshot = await dbRef.GetValueAsync().AsUniTask();

                if (!dataSnapshot.Exists)
                {
                    Debug.Log($"[MapService] Map with Id {mapId} does not exist.");
                    return null;
                }

                string mapJson = dataSnapshot.GetRawJsonValue();
                return JsonConvert.DeserializeObject<DueloMapDto>(mapJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MapService] An error occurred while retrieving the map: {ex.Message}");

                return null;
            }
        }

        public async UniTask SaveMap(string mapId, DueloMapDto map)
        {
            try
            {
                var dbRef = GetRef(SchemaCollection.Map, mapId);

                string mapJson = JsonConvert.SerializeObject(map);
                Debug.Log("Saving map: " + mapJson);

                await dbRef.SetRawJsonValueAsync(mapJson).AsUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log($"[MapService] An error occurred while saving the map: {ex.Message}");
            }
        }

        public async UniTask<MatchDto> GetMatch(string matchId)
        {
            try
            {
                var dbRef = GetRef(SchemaCollection.Match, matchId);

                var dataSnapshot = await dbRef.GetValueAsync().AsUniTask();

                if (!dataSnapshot.Exists)
                {
                    Console.WriteLine($"[MatchService] Match with ID {matchId} does not exist.");
                    return null;
                }

                return JsonConvert.DeserializeObject<MatchDto>(dataSnapshot.GetRawJsonValue());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MatchService] An error occurred while retrieving the match: {ex.Message}");

                return null;
            }
        }

        public async UniTask<DueloPlayerDto> GetPlayerById(string playerId)
        {
            try
            {
                var dbRef = GetRef(SchemaCollection.Player, playerId);

                var dataSnapshot = await dbRef.GetValueAsync().AsUniTask();

                if (!dataSnapshot.Exists)
                {
                    Debug.Log($"[PlayerService] Player with Id {playerId} does not exist.");
                    return null;
                }

                string json = dataSnapshot.GetRawJsonValue();
                return JsonConvert.DeserializeObject<DueloPlayerDto>(json);
            }
            catch (Exception ex)
            {
                Debug.Log($"[PlayerService] An error occurred while retrieving the map: {ex.Message}");

                return null;
            }
        }
    }
}