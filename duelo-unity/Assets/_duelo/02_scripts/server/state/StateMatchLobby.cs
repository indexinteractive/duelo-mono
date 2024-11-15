namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Duelo.Server.Match;
    using Ind3x.State;
    using UnityEngine;

    public class StateMatchLobby : ServerMatchState
    {
        public override void OnEnter()
        {
            Match.UpdateState(MatchState.Lobby).ContinueWith(success =>
            {
                Debug.Log("[StateMatchLobby] Waiting for players to join lobby");
                Match.OnPlayersConnectionChanged += OnConnectionStatusChanged;
            });
        }

        private void OnConnectionStatusChanged(ConnectionChangedEventArgs e)
        {
            if (e.ChallengerStatus == ConnectionStatus.Online && e.DefenderStatus == ConnectionStatus.Online)
            {
                Debug.Log("[StateMatchLobby] Both players are now online. Transitioning to game initialization.");

                UniTask.Create(async () =>
                {
                    await UniTask.DelayFrame(1);
                    StateMachine.SwapState(new StateInitializeGame());
                })
                .Forget();
            }
        }

        public override StateExitValue OnExit()
        {
            Match.OnPlayersConnectionChanged -= OnConnectionStatusChanged;
            return base.OnExit();
        }
    }
}
