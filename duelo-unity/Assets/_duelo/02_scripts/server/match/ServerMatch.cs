namespace Duelo.Server.Match
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class ConnectionChangedEventArgs
    {
        public ConnectionStatus ChallengerStatus { get; set; }
        public ConnectionStatus DefenderStatus { get; set; }
    }

    /// <summary>
    /// Object that represents a match in the server. Communicates directly
    /// with firebase to perform updates on the match state.
    /// </summary>
    public class ServerMatch
    {
        #region Match Properties
        public string MatchId { get; private set; }

        public MatchState State { get; private set; }
        public readonly MatchClock Clock;

        public readonly MatchPlayer Defender;
        public readonly MatchPlayer Challenger;

        public readonly List<MatchRound> Rounds;
        public MatchRound CurrentRound => Rounds.LastOrDefault();

        public MatchPlayer[] Players => new[] { Challenger, Defender };
        #endregion

        #region Player Properties
        public Action<ConnectionChangedEventArgs> OnPlayersConnectionChanged;
        #endregion

        #region Initialization
        public ServerMatch(MatchDto dbData)
        {
            MatchId = dbData.MatchId;
            State = MatchState.Startup;

            Defender = new MatchPlayer(MatchId, PlayerRole.Defender, dbData.Players.Defender);
            Challenger = new MatchPlayer(MatchId, PlayerRole.Challenger, dbData.Players.Challenger);

            Rounds = new List<MatchRound>();
            Clock = new MatchClock(dbData.ClockConfig);

            Defender.OnStatusChanged += UpdatePlayersConnection;
            Challenger.OnStatusChanged += UpdatePlayersConnection;
        }
        #endregion

        #region Player Connections
        private void UpdatePlayersConnection(PlayerStatusChangedEvent e)
        {
            try
            {
                Debug.Log($"[FirebaseMatch] Player status changed: challenger={Challenger.Status}, defender={Defender.Status}");

                var eventArgs = new ConnectionChangedEventArgs
                {
                    ChallengerStatus = Challenger.Status,
                    DefenderStatus = Defender.Status
                };

                OnPlayersConnectionChanged?.Invoke(eventArgs);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[FirebaseMatch] Error checking player online status: {ex.Message}");
            }
        }
        #endregion

        #region Match States
        public ServerMatch SetState(MatchState state)
        {
            State = state;
            return this;
        }

        public ServerMatch NewRound()
        {
            Clock.NewRound();

            var round = new MatchRound(Clock.CurrentRound, Clock.CurrentTimeAllowedMs);
            Rounds.Add(round);

            return this;
        }
        #endregion

        #region Firebase
        public async UniTask<object> Save()
        {
            string json = JsonConvert.SerializeObject(ToDto());
            return await MatchService.Instance.SetData(MatchId, json);
        }

        private object ToDto()
        {
            return new MatchDto
            {
                MatchId = MatchId,
                State = State,
                ClockConfig = Clock.ToDto(),
                Players = new MatchPlayersDto
                {
                    Challenger = Challenger.ToDto(),
                    Defender = Defender.ToDto()
                },
                Rounds = Rounds.Select(x => x.ToDto())
            };
        }
        #endregion
    }
}