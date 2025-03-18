namespace Duelo.Common.Core
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using ObservableCollections;
    using R3;
    using UnityEngine;

    public class ObservablePlayerState
    {
        public ReactiveProperty<float> Health;
        public ReactiveProperty<Vector3> Position;

        public ObservablePlayerState(PlayerRoundStateDto dto = null)
        {
            Health = new ReactiveProperty<float>(dto?.Health ?? default);
            Position = new ReactiveProperty<Vector3>(dto?.Position ?? default);
            UpdateFromDto(dto);
        }

        public void UpdateFromDto(PlayerRoundStateDto dto)
        {
            Health.Value = dto?.Health ?? default;
            Position.Value = dto?.Position ?? default;
        }
    }

    public class MatchRound
    {
        #region Public Properties
        public readonly int RoundNumber;
        public readonly uint TimeAllowed;
        public readonly DateTime StartTime;

        public ObservableDictionary<PlayerRole, ObservablePlayerState> PlayerState = new();
        #endregion

        #region Events
        private readonly CompositeDisposable _subscriptions = new();
        #endregion

        #region Movement Phase
        /// <summary>
        /// Movement data received from the players
        /// </summary>
        public ObservableDictionary<PlayerRole, PlayerRoundMovementDto> PlayerMovement = new();
        public uint MovementTimer;
        #endregion

        #region Action Phase
        /// <summary>
        /// Action data received from the players
        /// </summary>
        public ObservableDictionary<PlayerRole, PlayerRoundActionDto> PlayerAction = new();
        public uint ActionTimer;
        #endregion

        #region Initialization
        public MatchRound(int roundNumber, uint timeAllowed, Dictionary<PlayerRole, PlayerRoundStateDto> initialStates)
        {
            RoundNumber = roundNumber;
            TimeAllowed = timeAllowed;
            StartTime = DateTime.UtcNow;

            foreach (var (role, state) in initialStates)
            {
                PlayerState[role] = new ObservablePlayerState(state);
            }
        }

        public MatchRound(MatchRoundDto dto)
        {
            RoundNumber = dto.RoundNumber;
            UpdateFromDto(dto);
        }

        public void UpdateFromDto(MatchRoundDto dto)
        {
            PlayerState.Clear();
            PlayerMovement.Clear();
            PlayerAction.Clear();

            if (dto.Movement != null)
            {
                MovementTimer = dto.Movement.Timer;
                PlayerMovement[PlayerRole.Challenger] = dto.Movement.Challenger;
                PlayerMovement[PlayerRole.Defender] = dto.Movement.Defender;
            }

            if (dto.Action != null)
            {
                ActionTimer = dto.Action.Timer;
                PlayerAction[PlayerRole.Challenger] = dto.Action.Challenger;
                PlayerAction[PlayerRole.Defender] = dto.Action.Defender;
            }

            PlayerState[PlayerRole.Challenger] = new(dto.PlayerState.Challenger);
            PlayerState[PlayerRole.Defender] = new(dto.PlayerState.Defender);
        }
        #endregion

        #region Movement Methods
        public MovementPhaseDto KickoffMovement()
        {
            MovementTimer = TimeAllowed;

            return new MovementPhaseDto()
            {
                Timer = MovementTimer
            };
        }

        public void UpdateMovement(MovementPhaseDto dto)
        {
            PlayerMovement.Clear();
            if (dto.Challenger != null)
            {
                PlayerMovement.Add(PlayerRole.Challenger, dto.Challenger);
            }

            if (dto.Defender != null)
            {
                PlayerMovement.Add(PlayerRole.Defender, dto.Defender);
            }
        }
        #endregion

        #region Action Methods
        /// <summary>
        /// Kicks off the action phase by setting the timer and adds a
        /// callback to the action node listener.
        /// </summary>
        public ActionPhaseDto KickoffActions()
        {
            ActionTimer = TimeAllowed;

            return new ActionPhaseDto()
            {
                Timer = ActionTimer
            };
        }

        public void UpdateActions(ActionPhaseDto dto)
        {
            PlayerAction.Clear();
            if (dto.Challenger != null)
            {
                PlayerAction.Add(PlayerRole.Challenger, dto.Challenger);
            }

            if (dto.Defender != null)
            {
                PlayerAction.Add(PlayerRole.Defender, dto.Defender);
            }
        }

        /// <summary>
        /// Called when the round ends to clean up the listeners in <see cref="ServerMatch.NewRound"/>.
        /// Returns a dictionary of updates to ensure the database is updated
        /// </summary>
        public Dictionary<string, object> End()
        {
            _subscriptions.Dispose();

            Dictionary<string, object> updates = new();

            return updates;
        }

        /// <summary>
        /// This is called once only when creating a round in <see cref="ServerMatch.NewRound"/>.
        /// Should only publish the initial round state, the rest is updated atomically as the round plays out.
        /// </summary>
        public MatchRoundDto Publish()
        {
            var data = new MatchRoundDto()
            {
                RoundNumber = RoundNumber,
                PlayerState = GetRoundStatesDto(),
                Action = null,
                Movement = null,
            };

            return data;
        }
        #endregion

        #region Dto Methods
        public RoundStatesDto GetRoundStatesDto()
        {
            return new RoundStatesDto()
            {
                Challenger = new PlayerRoundStateDto()
                {
                    Health = PlayerState.GetValueOrDefault(PlayerRole.Challenger).Health.Value,
                    Position = PlayerState.GetValueOrDefault(PlayerRole.Challenger).Position.Value
                },
                Defender = new PlayerRoundStateDto()
                {
                    Health = PlayerState.GetValueOrDefault(PlayerRole.Defender).Health.Value,
                    Position = PlayerState.GetValueOrDefault(PlayerRole.Defender).Position.Value
                }
            };
        }
        #endregion
    }
}