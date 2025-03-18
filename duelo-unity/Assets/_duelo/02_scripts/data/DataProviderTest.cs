namespace Duelo.Data
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Match;
    using Duelo.Common.Model;
    using Ind3x.Observer;
    using ObservableCollections;
    using R3;
    using UnityEngine;

    public class DataProviderTest : MonoBehaviour
    {
        // private DataObserver<MatchDto> match;

        private void Start()
        {
            Debug.Log("[DataProviderTest] Starting data provider test");

            // match = new DataObserver<MatchDto>();
            var emptyDto = new MatchDto()
            {
                Players = new MatchPlayersDto()
                {
                    Challenger = new MatchPlayerDto() { UnityPlayerId = "challenger_id" },
                    Defender = new MatchPlayerDto() { UnityPlayerId = "defender_id" }
                }
            };
            var match = new MockMatch(emptyDto);

            // Subscribe to a specific round's PlayerState Challenger Health
            // match.Subscribe(m => m.Rounds[1].PlayerState.Challenger.Health, OnRoundHealthChanged);
            IDisposable healthUnsub = null;
            match.Rounds
                .ObserveChanged()
                .Select(_ => match.Rounds.FirstOrDefault(r => r.RoundNumber == 1))
                .DistinctUntilChangedBy(r => r)
                .Where(r => r != null)
                .Subscribe(round =>
                {
                    healthUnsub = round.PlayerState[PlayerRole.Challenger].Health.Subscribe(OnRoundHealthChanged);
                });

            MatchDto newData = new MatchDto
            {
                Rounds = new MatchRoundDto[]
                {
                    new MatchRoundDto { RoundNumber = 0, PlayerState = new RoundStatesDto { Challenger = new PlayerRoundStateDto { Health = 100 } } },
                    new MatchRoundDto { RoundNumber = 1, PlayerState = new RoundStatesDto { Challenger = new PlayerRoundStateDto { Health = 80 } } },
                    new MatchRoundDto { RoundNumber = 2, PlayerState = new RoundStatesDto { Challenger = new PlayerRoundStateDto { Health = 60 } } }
                }
            };

            match.UpdateFromDto(newData);

            // Atomic write to update round 1 health
            // match.Set(m => m.Rounds[1].PlayerState.Challenger.Health, 50);
            match.Rounds[1].PlayerState[PlayerRole.Challenger].Health.Value = 50;

            // Atomic write to set a new connection state for Challenger
            // match.Subscribe(m => m.Players.Challenger.Connection, OnConnectionChanged);
            var connectionUnsub = match.Players[PlayerRole.Challenger].Status.Subscribe(OnConnectionChanged);

            // match.Set(m => m.Players.Challenger.Connection, ConnectionStatus.Offline);
            match.Players[PlayerRole.Challenger].Status.Value = ConnectionStatus.Offline;

            // match.Unsubscribe<float>(OnRoundHealthChanged);
            healthUnsub.Dispose();
            Debug.Log("[DataProviderTest] -------- Unsubscribed from health changes");
            // match.Set(m => m.Rounds[1].PlayerState.Challenger.Health, 123);
            match.Rounds[1].PlayerState[PlayerRole.Challenger].Health.Value = 123;

            // match.Set(m => m.Players.Challenger.Connection, ConnectionStatus.Online);
            match.Players[PlayerRole.Challenger].Status.Value = ConnectionStatus.Online;

            // match.Unsubscribe<ConnectionStatus>(OnConnectionChanged);
            connectionUnsub.Dispose();
            Debug.Log("[DataProviderTest] -------- Unsubscribed from connection changes");
            // match.Set(m => m.Players.Challenger.Connection, ConnectionStatus.Online);
            match.Players[PlayerRole.Challenger].Status.Value = ConnectionStatus.Online;

            // var healthValue = match.Get(m => m.Rounds[1].PlayerState.Challenger.Health);
            var healthValue = match.Rounds[1].PlayerState[PlayerRole.Challenger].Health.Value;
            Debug.Log($"Health value r1: {healthValue}");

            Debug.Log("[DataProviderTest] Data provider test finished");
        }

        private void OnRoundHealthChanged(float newValue)
        {
            Debug.Log($"Health is now {newValue}");
        }

        private void OnConnectionChanged(ConnectionStatus newValue)
        {
            Debug.Log($"Connection changed to {newValue}");
        }
    }
}