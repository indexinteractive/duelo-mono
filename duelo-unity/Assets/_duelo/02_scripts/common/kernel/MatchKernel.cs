namespace Duelo.Common.Kernel
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
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

        #region Round Data
        // private readonly Dictionary<PlayerRole, Vector3> _movementData = new();
        private readonly Dictionary<string, int> _actionData = new();
        #endregion

        #region Entity Management
        public void RegisterEntities(params IExecuteEntity[] entities)
        {
            Entities.AddRange(entities);
        }
        #endregion

        #region Round Execution Logic
        public void QueuePlayerMovement(PlayerRole role, int actionId, Vector3 destination)
        {
            foreach (var entity in Entities)
            {
                if (entity is MatchPlayer player && player.Role == role)
                {
                    Debug.Log($"[ChooseMovement] Player {player.Id}({player.Role}) chose destination {destination}");

                    var moveAction = ActionFactory.Instance.GetDescriptor(MovementActionId.Walk, new Vector3(1, 0, 1));
                    if (moveAction != null)
                    {
                        player.Enqueue(moveAction);

                        // _movementData[player.Role] = destination;
                    }
                }
            }
        }

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