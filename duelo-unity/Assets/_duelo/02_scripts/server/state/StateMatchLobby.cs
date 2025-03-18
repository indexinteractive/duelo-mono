namespace Duelo.Server.State
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Ind3x.State;
    using R3;
    using UnityEngine;

    public class StateMatchLobby : ServerMatchState
    {
        private readonly CompositeDisposable _connectionSubs = new();

        public override void OnEnter()
        {
            Debug.Log("[StateMatchLobby] OnEnter");
            Server.PublishSyncState(MatchState.Lobby)
                .ContinueWith(() =>
                {
                    string playerIds = string.Join(", ", Match.Players.Select(p => p.Value.UnityPlayerId));
                    Debug.Log($"[StateMatchLobby] Waiting for players to join lobby: {playerIds}");

                    Match.Players[PlayerRole.Challenger].Status
                        .Subscribe(status => OnConnectionStatusChanged(PlayerRole.Challenger, status))
                        .AddTo(_connectionSubs);

                    Match.Players[PlayerRole.Defender].Status
                        .Subscribe(status => OnConnectionStatusChanged(PlayerRole.Defender, status))
                        .AddTo(_connectionSubs);
                });
        }

        private void OnConnectionStatusChanged(PlayerRole role, ConnectionStatus status)
        {
            Debug.Log($"[StateMatchLobby] Player {role} status changed to {status}");

            if (Match.Players.All(p => p.Value.Status.Value == ConnectionStatus.Online))
            {
                Debug.Log("[StateMatchLobby] Both players are now online. Transitioning to game initialization.");
                StateMachine.SwapState(new StateInitializeGame());
            }
        }

        public override StateExitValue OnExit()
        {
            _connectionSubs?.Dispose();
#if !DUELO_LOCAL
            GlobalState.AppQuitTimer.Cancel();
#endif
            return base.OnExit();
        }
    }
}
