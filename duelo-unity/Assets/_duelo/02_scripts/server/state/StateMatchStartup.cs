namespace Duelo.Server.State
{
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
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
            Debug.Log("StateMatchStartup");

            Match.SetState(MatchState.Pending).Save()
                .ContinueWith(_ => MapService.Instance.GetMap(Match.MapId))
                .ContinueWith(LoadMap)
                .ContinueWith(() =>
                {
                    StateMachine.SwapState(new StateMatchLobby());
                });
        }

        public void LoadMap(DueloMapDto dto)
        {
            ServerData.Map.Load(dto);
            foreach (var player in Match.Players)
            {
                Map.SpawnPlayer(player);
            }
        }
    }
}