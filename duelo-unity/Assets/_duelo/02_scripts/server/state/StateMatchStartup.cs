namespace Duelo.Server.State
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// One time startup state for matches
    /// </summary>
    public class StateMatchStartup : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("StateMatchStartup");
            var update = new Dictionary<string, object>
            {
                { "state", MatchState.Pending.ToString() },
                { "startTime", DateTime.UtcNow.ToString("o") }
            };

            Match.PartialUpdate(update).ContinueWith(success =>
            {
                Debug.Log("[StateMatchStartup] update? " + success);
                StateMachine.SwapState(new StateMatchLobby());
            });
        }
    }
}