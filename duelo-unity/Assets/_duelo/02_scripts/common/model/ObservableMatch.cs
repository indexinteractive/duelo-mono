namespace Duelo.Common.Model
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using ObservableCollections;
    using R3;

    public class ObservableSyncState
    {
        public ReactiveProperty<MatchState> Server = new(MatchState.Startup);
        public ReactiveProperty<MatchState?> Defender = new(null);
        public ReactiveProperty<MatchState?> Challenger = new(null);

        public void Update(PlayerRole role, MatchState state)
        {
            if (role == PlayerRole.Defender)
            {
                Defender.Value = state;
            }
            else if (role == PlayerRole.Challenger)
            {
                Challenger.Value = state;
            }
        }

        public void Update(SyncStateDto dbSyncState)
        {
            Defender.Value = dbSyncState.Defender;
            Challenger.Value = dbSyncState.Challenger;
        }
    }

    public abstract class ObservableMatch
    {
        public readonly MatchDto InitialDto;

        public readonly string MatchId;
        public readonly string MapId;
        public readonly DateTime? CreatedTime;
        public readonly MatchClockConfigurationDto ClockConfig;

        public readonly ObservableList<MatchRound> Rounds = new();
        public readonly ReadOnlyReactiveProperty<MatchRound> CurrentRound;

        public readonly ObservableDictionary<PlayerRole, MatchPlayer> Players = new();
        public MatchPlayersDto PlayersDto => InitialDto.Players;

        public ObservableSyncState SyncState = new();

        public ObservableMatch(Unity.Services.Matchmaker.Models.MatchmakingResults matchmakerData)
        {
            InitialDto = new MatchDto();

            MatchId = matchmakerData.MatchId;
            // TODO: Should have map selection logic based on matchmaker data
            MapId = "devmap";

            CreatedTime = DateTime.UtcNow;

            // TODO: This configuration should come from a remote config and can be based on matchmaker arguments
            ClockConfig = new MatchClockConfigurationDto()
            {
                ExpectedRounds = 5,
                FreeRounds = 1,
                InitialTimeAllowedMs = 10000,
                MinTimeAllowedMs = 3000,
            };

            InitialDto.Players = MatchPlayersDto.FromMatchmakerData(matchmakerData);

            CurrentRound = Rounds.ObserveChanged()
                .Select(_ => Rounds?.LastOrDefault())
                .ToReadOnlyReactiveProperty();
        }

        public ObservableMatch(MatchDto dto)
        {
            InitialDto = dto;

            MatchId = dto.MatchId;
            MapId = dto.MapId;
            ClockConfig = dto.ClockConfig;
            CreatedTime = dto.CreatedTime;

            CurrentRound = Rounds.ObserveChanged()
                .Select(_ => Rounds?.LastOrDefault())
                .ToReadOnlyReactiveProperty();

            UpdateFromDto(dto);
        }

        public void UpdateFromDto(MatchDto dto)
        {
            if (dto.Rounds?.Length > 0)
            {
                for (int i = 0; i < dto.Rounds.Length; i++)
                {
                    if (i < Rounds.Count)
                    {
                        Rounds[i].UpdateFromDto(dto.Rounds[i]);
                    }
                    else
                    {
                        Rounds.Add(new MatchRound(dto.Rounds[i]));
                    }
                }
            }

            SyncState.Server.Value = dto.SyncState.Server;
        }

        public abstract void Dispose();
    }
}