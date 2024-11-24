namespace Duelo.Common.Service
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using System;
    using Newtonsoft.Json;
    using UnityEngine;
    using System.Collections.Generic;

    public class MatchService : FirebaseService<MatchService>
    {
        public async UniTask<MatchDto> GetMatch(string matchId)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);

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

        public async UniTask<bool> SetData(string matchId, string jsonData)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);
                await dbRef.SetRawJsonValueAsync(jsonData).AsUniTask();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchService] An error occurred while updating the match: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> SetData(string matchId, Dictionary<string, object> update)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);
                await dbRef.UpdateChildrenAsync(update).AsUniTask();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchService] An error occurred while updating the match: {ex.Message}");
                return false;
            }
        }
    }
}
