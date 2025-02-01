namespace Duelo.Server.State
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using Ind3x.State;
    using UnityEngine;

    public class StateMatchLobby : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateMatchLobby] OnEnter");
            Match.SetState(MatchState.Lobby)
                .ContinueWith(() => Match.PublishSyncState())
                .ContinueWith(() =>
                {
                    string playerIds = string.Join(", ", Match.Players.Select(p => p.Value.UnityPlayerId));
                    Debug.Log($"[StateMatchLobby] Waiting for players to join lobby: {playerIds}");
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
                Match.OnPlayersConnectionChanged -= OnConnectionStatusChanged;
            }
        }

        public override StateExitValue OnExit()
        {
            Match.OnPlayersConnectionChanged -= OnConnectionStatusChanged;
#if !DUELO_LOCAL
            GlobalState.AppQuitTimer.Cancel();
#endif
            return base.OnExit();
        }
    }
}
