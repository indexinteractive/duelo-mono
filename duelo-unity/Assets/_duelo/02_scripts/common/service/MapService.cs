namespace Duelo.Common.Service
{
    using Cysharp.Threading.Tasks;
    using System;
    using Newtonsoft.Json;
    using Duelo.Gameboard;
    using UnityEngine;

    public class MapService : FirebaseService<MapService>
    {
        public async UniTask<DueloMapDto> GetMap(string mapId)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Map, mapId);

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
                Debug.Log($"[MapService] An error occurred while retrieving the map: {ex.Message}");

                return null;
            }
        }

        public async UniTask SaveMap(string mapId, DueloMapDto map)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Map, mapId);

                string mapJson = JsonConvert.SerializeObject(map);
                Debug.Log("Saving map: " + mapJson);

                await dbRef.SetRawJsonValueAsync(mapJson).AsUniTask();
            }
            catch (Exception ex)
            {
                Debug.Log($"[MapService] An error occurred while saving the map: {ex.Message}");
            }
        }
    }
}