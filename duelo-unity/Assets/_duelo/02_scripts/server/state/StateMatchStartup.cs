namespace Duelo.Server.State
{
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

            foreach (var player in Match.Players)
            {
                Map.SpawnPlayer(player);
            }

            Match.SetState(MatchState.Pending).Save().ContinueWith(success =>
            {
                StateMachine.SwapState(new StateMatchLobby());
            });
        }
    }
}