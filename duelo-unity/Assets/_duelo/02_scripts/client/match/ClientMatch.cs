namespace Duelo.Client.Match
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    public class ClientMatch : IDisposable
    {
        #region Private Fields
        private DatabaseReference _ref;
        #endregion

        #region Match Properties
        public MatchDto CurrentDto { get; private set; }
        public MatchRoundDto CurrentRound => CurrentDto.Rounds.Last();
        public Dictionary<PlayerRole, MatchPlayer> Players = new();

        public MatchPlayer DevicePlayer => Players.Values.FirstOrDefault(p => p.IsDevicePlayer);
        #endregion

        #region Actions
        /// <summary>
        /// Event that is triggered when the match state changes.
        /// The first parameter is the new state, the second is the previous state.
        /// </summary>
        public Action<MatchDto, MatchDto> OnStateChange;
        #endregion

        #region Initialization
        public ClientMatch(MatchDto match)
        {
            CurrentDto = match;
            _ref = FirebaseInstance.Instance.Db.GetReference(DueloCollection.Match.ToString().ToLower()).Child(match.MatchId);
        }
        #endregion

        #region Players
        /// <summary>
        /// Called by <see cref="Client.Screen.PlayMatchScreen.OnEnter"/> to set the player's
        /// connection status to "online" and thereby "joining" the match.
        /// </summary>
        /// <returns></returns>
        public async UniTask JoinMatch()
        {
            var player = DevicePlayer;
            if (player != null)
            {
                await _ref.Child("players").Child(player.Role.ToString().ToLower())
                    .Child("connection")
                    .SetValueAsync(ConnectionStatus.Online.ToString());
            }

            _ref.ValueChanged += OnMatchUpdate;
        }
        #endregion

        #region Server / Client Sync
        /// <summary>
        /// This method is a response by the client for the data published by the server
        /// in <see cref="Server.Match.ServerMatch.PublishSyncState"/>, which is called on
        /// every state change.
        /// By publishing the sync state, we can maintain consistent sync between the server
        /// and player clients
        /// </summary>
        public async UniTask PublishSyncState(string state)
        {
            Debug.Log($"[ClientMatch] Publishing sync state: {state} for {DevicePlayer.Role}");
            await _ref.Child("sync").Child(DevicePlayer.Role.ToString().ToLower()).SetValueAsync(state);
            Debug.Log("[ClientMatch] Sync state published ✔️");
        }
        #endregion

        #region Data Update
        /// <summary>
        /// In general, async void should be avoided *except for event handlers*
        /// </summary>
        private async void OnMatchUpdate(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.DatabaseError != null)
            {
                Debug.LogError($"[ClientMatch] {eventArgs.DatabaseError.Message}");
                return;
            }

            if (eventArgs.Snapshot == null || !eventArgs.Snapshot.Exists)
            {
                Debug.LogWarning("[ClientMatch] No match data to deserialize");
                return;
            }

            try
            {
                string jsonValue = eventArgs.Snapshot.GetRawJsonValue();

                MatchDto previousDto = CurrentDto;
                CurrentDto = JsonConvert.DeserializeObject<MatchDto>(jsonValue);

                Debug.Log($"[ClientMatch] ({DevicePlayer.Role}) Match update -- {previousDto?.SyncState?.Server} (previous) => {CurrentDto?.SyncState?.Server} (current)");

                if (previousDto?.SyncState?.Server != CurrentDto?.SyncState?.Server)
                {
                    await PublishSyncState(CurrentDto?.SyncState?.Server.ToString());

                    OnStateChange?.Invoke(CurrentDto, previousDto);

                    foreach (var player in Players)
                    {
                        player.Value.OnMatchStateChanged(CurrentDto.SyncState.Server);
                    }
                }
            }
            catch (System.Exception error)
            {
                Debug.LogError("[ClientMatch] Error: " + error.Message);
            }
        }
        #endregion

        #region Asset Loading
        /// <summary>
        /// Called by <see cref="Client.Screen.MatchmakingScreen.LoadAssets"/> when running a real match
        /// and by <see cref="Client.Screen.DebugMatchScreen.LoadAssets"/> when running a local debug match.
        /// </summary>
        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, CurrentDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, CurrentDto.Players.Defender);
        }

        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(CurrentDto.MatchId, role, playerDto);
            Debug.Log($"[ClientMatch] Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            _ref.ValueChanged -= OnMatchUpdate;
        }
        #endregion
    }
}