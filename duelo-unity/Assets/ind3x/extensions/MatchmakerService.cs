namespace Ind3x.Extensions
{
    using System.Net.Http;
    using Cysharp.Threading.Tasks;
    using Newtonsoft.Json;
    using Unity.Services.Matchmaker;
    using Unity.Services.Matchmaker.Models;
    using UnityEngine;

    public static class MatchmakerServiceExtensions
    {
        private static readonly string BASE_URL = "http://localhost:8086/payload";

        public static async UniTask<MatchmakingResults> GetMatchmakingResults(this IMatchmakerService _, string allocationUuid)
        {
            string url = $"{BASE_URL}/{allocationUuid}";
            Debug.Log("[MatchmakerServiceExtensions] Fetching matchmaking results from: " + url);

            try
            {
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<MatchmakingResults>(responseContent);
                }
                else
                {
                    Debug.LogError($"[MatchmakerServiceExtensions] Error: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("[MatchmakerServiceExtensions] Exception: " + ex.Message);
            }

            return null;
        }
    }
}