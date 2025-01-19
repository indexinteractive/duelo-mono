namespace Duelo.Common.Service
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using System;
    using Newtonsoft.Json;
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

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

            if (matchmakerData.MatchProperties.Players.Count != 2)
            {
                Debug.LogError("[MatchService] Matchmaker data should have 2 players");
                Application.Quit(Duelo.Common.Util.ExitCode.InvalidMatch);
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
                Players = GetMatchPlayersDto(matchmakerData),
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

        #region Private Helpers
        private MatchPlayersDto GetMatchPlayersDto(Unity.Services.Matchmaker.Models.MatchmakingResults matchmakerData)
        {
            var matchPlayers = new MatchPlayersDto();

            var p1 = matchmakerData.MatchProperties.Players[0];
            var p2 = matchmakerData.MatchProperties.Players[1];

            var p1Team = matchmakerData.MatchProperties.Teams.Where(t => t.PlayerIds.Contains(p1.Id)).FirstOrDefault();
            var p2Team = matchmakerData.MatchProperties.Teams.Where(t => t.PlayerIds.Contains(p2.Id)).FirstOrDefault();

            if (p1Team.TeamName == "challenger")
            {
                matchPlayers.Challenger = new MatchPlayerDto
                {
                    PlayerId = p1.Id,
                    Profile = p1.CustomData.GetAs<PlayerProfileDto>()
                };

                matchPlayers.Defender = new MatchPlayerDto
                {
                    PlayerId = p2.Id,
                    Profile = p2.CustomData.GetAs<PlayerProfileDto>()
                };
            }
            else if (p2Team.TeamName == "challenger")
            {
                matchPlayers.Challenger = new MatchPlayerDto
                {
                    PlayerId = p2.Id,
                    Profile = p2.CustomData.GetAs<PlayerProfileDto>()
                };

                matchPlayers.Defender = new MatchPlayerDto
                {
                    PlayerId = p1.Id,
                    Profile = p1.CustomData.GetAs<PlayerProfileDto>()
                };
            }

            return matchPlayers;
        }
        #endregion
    }
}
