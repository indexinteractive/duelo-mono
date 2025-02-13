namespace Duelo.Server.State
{
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
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

            Match.PublishSyncState(MatchState.Pending)
                .ContinueWith(() => MapService.Instance.GetMap(Match.MapId))
                .ContinueWith(LoadAssets)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateMatchLobby());
                });
        }

        public void LoadAssets(DueloMapDto dto)
        {
            GlobalState.Map.Load(dto);

            Match.LoadAssets();

            Kernel.RegisterEntities(Match.Players.Values.ToArray());
        }
    }
}