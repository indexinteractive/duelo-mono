namespace Duelo.Common.Kernel
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Solely responsible for running a round of the game.
    /// - Activates all IExecuteEntity instances
    /// - Waits for all IExecuteEntity instances to finish their actions
    /// - Deactivates all IExecuteEntity instances
    ///
    /// Server usage: <see cref="Server.State.StateExecuteRound"/>
    /// </summary>
    public class MatchKernel
    {
        #region Public Properties
        public readonly List<IExecuteEntity> Entities = new();
        #endregion

        #region Entity Management
        public void RegisterEntities(params IExecuteEntity[] entities)
        {
            Entities.AddRange(entities);
        }
        #endregion

        #region Round Execution Logic
        public void QueueMovementPhase(MovementPhaseDto movementDto)
        {
            if (movementDto == null)
            {
                Debug.LogWarning("[MatchKernel] No movement data to add to the queue");
                return;
            }

            var challengerMovement = movementDto.Challenger;
            if (challengerMovement != null)
            {
                Debug.Log("[MatchKernel] Queueing challenger movement");
                var args = new object[] { challengerMovement.TargetPosition };
                QueuePlayerAction(PlayerRole.Challenger, challengerMovement.ActionId, args);
            }

            var defenderMovement = movementDto.Defender;
            if (defenderMovement != null)
            {
                Debug.Log("[MatchKernel] Queueing defender movement");
                var args = new object[] { defenderMovement.TargetPosition };
                QueuePlayerAction(PlayerRole.Defender, defenderMovement.ActionId, args);
            }
        }

        public void QueuePlayerAction(PlayerRole role, int actionId, params object[] args)
        {
            foreach (var entity in Entities)
            {
                if (entity is MatchPlayer player && player.Role == role)
                {
                    var actionDescriptor = ActionFactory.Instance.GetDescriptor(actionId, args);
                    if (actionDescriptor != null)
                    {
                        Debug.Log($"[MatchKernel] Adding action {actionId} to Player {player.UnityPlayerId}({player.Role})");
                        player.Enqueue(actionDescriptor);
                    }
                }
            }
        }

        /// <summary>
        /// Runs all actions queued in each entity.
        ///
        /// Server: <see cref="Server.State.StateExecuteRound"/>
        /// Server testing: <see cref="ServerPhaseTesting.StateExecuteRound"/>
        ///
        /// Client: <see cref="Client.Screen.PlayMatchScreen"/>
        /// </summary>
        public async UniTask RunRound()
        {
            foreach (var entity in Entities)
            {
                entity.Begin();
            }

            Debug.Log("[MatchKernel] Waiting for actors to finish their actions");
            while (Entities.Any(x => x.IsRunning))
            {
                await UniTask.Yield();
            }
            Debug.Log("[MatchKernel] Actors finished their actions");

            foreach (var entity in Entities)
            {
                entity.End();
            }
        }
        #endregion
    }
}