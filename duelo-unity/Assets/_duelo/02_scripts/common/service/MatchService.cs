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
                    Console.WriteLine($"Match with ID {matchId} does not exist.");
                    return null;
                }

                var matchData = JsonConvert.DeserializeObject<MatchDto>(dataSnapshot.GetRawJsonValue());

                return matchData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the match: {ex.Message}");

                return null;
            }
        }

        public async UniTask<bool> PartialUpdate(string matchId, Dictionary<string, object> update, string childPath = null)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);

                if (childPath != null)
                {
                    dbRef = dbRef.Child(childPath);
                }

                await dbRef.UpdateChildrenAsync(update).AsUniTask();

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred while updating the match: {ex.Message}");
                return false;
            }
        }

        public async UniTask<bool> UpdateMatchState(string matchId, string state)
        {
            var update = new Dictionary<string, object>
            {
                { "state", state }
            };

            return await PartialUpdate(matchId, update);
        }

        public async UniTask<bool> PushRound(string matchId, RoundDto newRound)
        {
            string childPath = $"rounds/{newRound.RoundNumber}";
            return await PartialUpdate(matchId, newRound.ToDictionary(), childPath);
        }
    }
}
