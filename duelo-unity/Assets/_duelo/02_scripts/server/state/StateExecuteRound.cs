namespace Duelo.Server.State
{
    using System;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Ind3x.State;
    using UnityEngine;

    public class StateExecuteRound : ServerMatchState
    {
        #region Private Fields
        private readonly int _maxSyncWaitMs = ServerData.StartupOptions.MatchSyncTimeoutMs;
        #endregion

        public override void OnEnter()
        {
            Debug.Log("[StateExecuteRound]");

            // TODO: Send round data to clients
            // RoundExecutePhaseDto serverRoundExecuteData = Match.ExecuteKernel.GetExecuteData();

            // TODO: Add event listener to clients to acknowledge actions have completed

            Kernel.RunRound().ContinueWith(WaitForClientSync);
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