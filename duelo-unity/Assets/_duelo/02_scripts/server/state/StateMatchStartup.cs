namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using Ind3x.State;
    using UnityEngine;

    /// <summary>
    /// One time startup state for matches
    /// </summary>
    public class StateMatchStartup : ServerMatchState
    {
        public override void OnEnter()
        {
            Debug.Log("[StateMatchStartup] OnEnter");

            GlobalState.Kernel = new MatchKernel();

            Server.PublishSyncState(MatchState.Pending)
                .ContinueWith(() => GlobalState.Services.GetMap(Match.MapId))
                .ContinueWith(LoadAssets)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateMatchLobby());
                });
        }

        public void LoadAssets(DueloMapDto dto)
        {
            GlobalState.Map.Load(dto);

            Server.LoadAssets();

            Kernel.RegisterEntities(Match.Players[PlayerRole.Defender], Match.Players[PlayerRole.Challenger]);
        }
    }
}