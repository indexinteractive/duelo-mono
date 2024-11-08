namespace Duelo.Common.Service
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using System;
    using Newtonsoft.Json;
    using Duelo.Server.State;

    public class MatchService : FirebaseService<MatchService>
    {
        public async UniTask<MatchDto> GetMatch(string matchId)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);

                var dataSnapshot = await dbRef.GetValueAsync();

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

        public async UniTask<bool> UpdateMatchState(string matchId, ServerMatchState state)
        {
            try
            {
                var dbRef = GetRef(DueloCollection.Match, matchId);
                await dbRef.Child("state").SetValueAsync(state.ToString());

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while updating the match state: {ex.Message}");
                return false;
            }
        }
    }
}
