namespace Duelo.Common.Service
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Newtonsoft.Json;
    using UnityEngine;

    public class PlayerService : FirebaseService<PlayerService>
    {
        public async UniTask<DueloPlayerDto> GetPlayerById(string playerId)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Player, playerId);

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