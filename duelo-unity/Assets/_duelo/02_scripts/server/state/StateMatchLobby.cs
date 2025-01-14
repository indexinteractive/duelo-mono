namespace Duelo.Server.State
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using Ind3x.State;
    using UnityEngine;
    using Duelo.Common.Core;

    public class StateMatchLobby : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateMatchLobby] OnEnter");
            Match.SetState(MatchState.Lobby).Save().ContinueWith(() =>
            {
                Debug.Log("[StateMatchLobby] Waiting for players to join lobby");
                if (Match.Players.All(p => p.Value.Status == ConnectionStatus.Online))
                {
                    Debug.Log("[StateMatchLobby] Both players are already online. Transitioning to game initialization.");
                    StateMachine.SwapState(new StateInitializeGame());
                }
                else
                {
                    Match.OnPlayersConnectionChanged += OnConnectionStatusChanged;
                }
            });
        }

        private void OnConnectionStatusChanged(ConnectionChangedEventArgs e)
        {
            if (e.ChallengerStatus == ConnectionStatus.Online && e.DefenderStatus == ConnectionStatus.Online)
            {
                Debug.Log("[StateMatchLobby] Both players are now online. Transitioning to game initialization.");
                StateMachine.SwapState(new StateInitializeGame());
            }
        }

        public override StateExitValue OnExit()
        {
            Match.OnPlayersConnectionChanged -= OnConnectionStatusChanged;
            GameData.AppQuitTimer.Cancel();
            return base.OnExit();
        }
    }
}
