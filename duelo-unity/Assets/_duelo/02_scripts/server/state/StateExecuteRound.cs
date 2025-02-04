namespace Duelo.Server.State
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    public class StateExecuteRound : ServerMatchState
    {
        #region Private Fields
        private readonly int _maxSyncWaitMs = GlobalState.StartupOptions.MatchSyncTimeoutMs;
        #endregion

        public override void OnEnter()
        {
            Debug.Log("[StateExecuteRound]");
            Match.WaitForSyncState(MatchState.ExecuteRound)
                .ContinueWith(() => QueuePlayerActions())
                .ContinueWith(Kernel.RunRound)
                .ContinueWith(WaitForClientSync);
        }

        private void QueuePlayerActions()
        {
            var challengerMovement = Match.CurrentRound.PlayerMovement?.Challenger;
            if (challengerMovement != null)
            {
                var args = new object[] { challengerMovement.TargetPosition };
                Kernel.QueuePlayerAction(PlayerRole.Challenger, challengerMovement.ActionId, args);
            }

            var defenderMovement = Match.CurrentRound.PlayerMovement?.Defender;
            if (defenderMovement != null)
            {
                var args = new object[] { defenderMovement.TargetPosition };
                Kernel.QueuePlayerAction(PlayerRole.Defender, defenderMovement.ActionId, args);
            }
        }

        private async UniTask WaitForClientSync()
        {
            var end = DateTime.Now.AddMilliseconds(_maxSyncWaitMs);

            // while (_players.Count < Match.Players.Count)
            {
                // Debug.Log($"[ExecuteRound] Waiting for {_players.Count}/{Match.Players.Count} players to execute round");
                await UniTask.Delay(100);

                if (DateTime.Now >= end)
                {
                    Debug.Log("[ExecuteRound] Timeout waiting for players to execute round!");
                    // TODO: Check for disconnect, show lag indicator, etc
                    // break;
                }
            }

            StateMachine.SwapState(new StateEndRound());
        }
    }
}