namespace Duelo.Client.Screen
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Client.UI;
    using Duelo.Common.Core;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Common.Util;
    using Duelo.Gameboard;
    using Ind3x.State;
    using Unity.Services.Matchmaker;
    using Unity.Services.Matchmaker.Models;
    using UnityEngine;

    /// <summary>
    /// Match Making screen. Uses <see cref="UI.MatchmakingUi"/> as the main UI.
    /// </summary>
    public class MatchmakingScreen : GameScreen
    {
        #region Public Properties
        public MatchmakingUi UiElements;

        // TODO: This should probably come from a remote config
        public readonly string UGS_QUEUE_NAME = "test-1v1-queue";
        #endregion

        #region Screen Implementation
        public override void OnEnter()
        {
            Debug.Log("[MatchmakingScreen] OnEnter");
            UiElements = SpawnUI<MatchmakingUi>(UIViewPrefab.Matchmaking);

            StartMatchmaking()
                .ContinueWith(createResponse => WaitForMatch(createResponse?.Id))
                .ContinueWith(assignment => FetchMatch(assignment));
        }

        public override StateExitValue OnExit()
        {
            DestroyUI();
            return null;
        }

        /// <summary>
        /// When a matchmaking assignment is given, a loading popup will appear to load the <see cref="MatchDto"/>.
        /// Once the data loads, the <see cref="LoadingPopup"/> state is removed and we come back to this state via resume
        /// </summary>
        /// <param name="results"></param>
        public override void Resume(StateExitValue results)
        {
            Debug.Log("[MatchmakingScreen] Resume");
            var data = results.data as LoadingPopup<MatchDto>.LoadResult;

            if (data.Result?.MatchId != null)
            {
                Debug.Log("[MatchmakingScreen] Match found: " + data.Result.MatchId);

                GlobalState.Kernel = new MatchKernel();
                GlobalState.ClientMatch = new ClientMatchFirebase(data.Result);
                MapService.Instance.GetMap(data.Result.MapId)
                    .ContinueWith(LoadAssets)
                    .ContinueWith(() =>
                    {
                        var camera = GameObject.FindAnyObjectByType<DueloCamera>();
                        camera.SetMapCenter(GlobalState.Map.MapCenter);
                        camera.FollowPlayers(GlobalState.ClientMatch.Players);
                        GlobalState.Camera = camera;

                        StateMachine.SwapState(new PlayMatchScreen(data.Result));
                    });
            }
            else
            {
                // TODO: Swap state to some error screen
                Debug.LogError("[MatchmakingScreen] Match not found");
            }
        }
        #endregion

        #region Matchmaking Logic
        private async UniTask<CreateTicketResponse> StartMatchmaking()
        {
            Debug.Log("[MatchmakingScreen] Starting matchmaking...");

            var players = new List<Unity.Services.Matchmaker.Models.Player>
            {
                new Unity.Services.Matchmaker.Models.Player(GlobalState.PlayerData.UnityPlayerId, GlobalState.PlayerData.ActiveProfile)
            };

            var options = new CreateTicketOptions(UGS_QUEUE_NAME);

            try
            {
                var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
                Debug.Log($"[MatchmakingScreen] Ticket created with ID: {ticketResponse.Id}");

                return ticketResponse;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[MatchmakingScreen] Failed to create ticket: {ex.Message}");
            }

            return null;
        }

        private async UniTask<MultiplayAssignment> WaitForMatch(string ticketId)
        {
            Debug.Log($"[MatchmakingScreen] Polling ticket status for ID: {ticketId}");
            MultiplayAssignment assignment = null;

            while (assignment == null)
            {
                try
                {
                    var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticketId);
                    if (ticketStatus?.Type == typeof(MultiplayAssignment))
                    {
                        var ticket = ticketStatus.Value as MultiplayAssignment;
                        Debug.Log($"[MatchmakingScreen] Ticket status: {ticketStatus.Type} - {ticket.Status}");

                        if (ticket.Status == MultiplayAssignment.StatusOptions.Found)
                        {
                            assignment = ticket;
                        }
                        // TODO: handle MultiplayAssignment.StatusOptions.Failed and MultiplayAssignment.StatusOptions.Timeout
                        else if (ticket.Status == MultiplayAssignment.StatusOptions.Failed || ticket.Status == MultiplayAssignment.StatusOptions.Timeout)
                        {
                            Debug.LogError($"[MatchmakingScreen] Ticket not assigned: {ticket.Status}");
                            break;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[MatchmakingScreen] Error polling ticket status: {e.Message}");
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1f));
            }

            return assignment;
        }

        private void FetchMatch(MultiplayAssignment assignment)
        {
            if (assignment?.MatchId != null)
            {
                Debug.Log("[MatchmakingScreen] Match found: " + assignment.MatchId);
                var loadState = new LoadingPopup<MatchDto>(Common.Service.DueloCollection.Match, assignment.MatchId);
                StateMachine.PushState(loadState);
            }
            else
            {
                // TODO: Swap state to some error screen
                Debug.LogError($"[MatchmakingScreen] Match not found \"{assignment?.MatchId}\"");
                StateMachine.SwapState(new MainMenuScreen());
            }
        }
        #endregion

        #region Loading
        public void LoadAssets(DueloMapDto dto)
        {
            if (dto == null)
            {
                Debug.LogError("[MatchmakingScreen] Map not found, crashing");
                Application.Quit(ExitCode.MapNotFound);
            }

            GlobalState.Map.Load(dto);
            GlobalState.ClientMatch.LoadAssets();
            GlobalState.Kernel.RegisterEntities(GlobalState.ClientMatch.Players.Values.ToArray());
        }
        #endregion
    }
}
