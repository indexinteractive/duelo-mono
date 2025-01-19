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
        public async UniTask<MatchDto> CreateMatch(Unity.Services.Matchmaker.Models.MatchmakingResults matchmakerData)
        {
            if (matchmakerData == null)
            {
                Debug.LogError("[MatchService] Matchmaker data is null");
                Application.Quit(Duelo.Common.Util.ExitCode.MatchNotFound);
                return null;
            }

            // TODO: This configuration should come from a remote config and can be based on matchmaker arguments
            var dto = new MatchDto
            {
                MatchId = matchmakerData.MatchId,
                ClockConfig = new MatchClockConfigurationDto()
                {
                    ExpectedRounds = 5,
                    FreeRounds = 1,
                    InitialTimeAllowedMs = 10000,
                    MinTimeAllowedMs = 3000,
                },
                CreatedTime = DateTime.UtcNow,
                MapId = "devmap",
                SyncState = new SyncStateDto
                {
                    Server = MatchState.Initialize
                },
                Rounds = null,
                Players = new MatchPlayersDto
                // TODO! Need player data from matchmaker
                {
                    Challenger = new MatchPlayerDto { },
                    Defender = new MatchPlayerDto { },
                },
                MatchmakerDto = matchmakerData,
            };

            var json = JsonConvert.SerializeObject(dto);

            var dbRef = GetRef(DueloCollection.Match, dto.MatchId);

            Debug.Log($"[MatchService] Creating match with ID {dto.MatchId}");
            await dbRef.SetRawJsonValueAsync(json).AsUniTask();

            return dto;
        }

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

        /// <summary>
        /// Overwrites the match data with the provided JSON data.
        /// </summary>
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
