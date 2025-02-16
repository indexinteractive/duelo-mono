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
    using Newtonsoft.Json;
    using UnityEngine;

    public class ClientMatchFirebase : IClientMatch
    {
        #region Match Properties
        public string MatchId => CurrentDto.MatchId;
        public string MapId => CurrentDto.MapId;
        public Dictionary<PlayerRole, MatchPlayer> Players { get; private set; }

        public MatchDto CurrentDto { get; private set; }
        public MatchRoundDto CurrentRound => CurrentDto.Rounds.Last();

        public MatchPlayer DevicePlayer => Players.Values.FirstOrDefault(p => p.IsDevicePlayer);
        #endregion

        #region Firebase References
        private DatabaseReference MatchRef => MatchService.Instance.GetRef(DueloCollection.Match, MatchId);
        private DatabaseReference RoundRef => MatchRef.Child("rounds").Child(CurrentRound.RoundNumber.ToString());
        #endregion

        #region Actions
        /// <summary>
        /// Event that is triggered when the match state changes.
        /// The first parameter is the new state, the second is the previous state.
        /// </summary>
        public Action<MatchDto, MatchDto> OnStateChange { get; set; }
        #endregion

        #region Initialization
        public ClientMatchFirebase(MatchDto match)
        {
            CurrentDto = match;
            Players = new Dictionary<PlayerRole, MatchPlayer>();
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
            Debug.Log($"[ClientMatch] {player.ProfileDto.Gamertag} Joining match {MatchId}");
            if (player != null)
            {
                await MatchRef.Child("players").Child(player.Role.ToString().ToLower())
                    .Child("connection")
                    .SetValueAsync(ConnectionStatus.Online.ToString());
            }

            MatchRef.ValueChanged += OnMatchUpdate;
            Debug.Log($"[ClientMatch] {player.ProfileDto.Gamertag} Listening for match updates");
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
            await MatchRef.Child("sync").Child(DevicePlayer.Role.ToString().ToLower()).SetValueAsync(state);
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

            string jsonValue = eventArgs.Snapshot?.GetRawJsonValue() ?? string.Empty;

            if (string.IsNullOrEmpty(jsonValue))
            {
                Debug.LogWarning("[ClientMatch] No match data to deserialize");
                return;
            }

            try
            {
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

        #region Round Updates
        public void DispatchMovement(int actionId, Vector3 position)
        {
            var data = new PlayerRoundMovementDto
            {
                ActionId = actionId,
                TargetPosition = position
            };

            var playerRole = DevicePlayer.Role;
            string json = JsonConvert.SerializeObject(data);

            Debug.Log($"[ClientMatch] Dispatching movement for {playerRole}: {json}");
            RoundRef.Child("movement").Child(playerRole.ToString().ToLower()).SetRawJsonValueAsync(json);
        }

        public void DispatchAttack(int actionId)
        {
            var data = new PlayerRoundActionDto
            {
                ActionId = actionId
            };

            var playerRole = DevicePlayer.Role;
            string json = JsonConvert.SerializeObject(data);

            Debug.Log($"[ClientMatch] Dispatching attack for {playerRole}: {json}");
            RoundRef.Child("action").Child(playerRole.ToString().ToLower()).SetRawJsonValueAsync(json);
        }
        #endregion

        #region Asset Loading
        /// <summary>
        /// Responsible for loading and spawning character assets before the match starts.
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

            var playerRef = MatchRef.Child("players").Child(role.ToString().ToLower());
            matchPlayer.Initialize(playerRef, role, playerDto);
            Debug.Log($"[ClientMatch] Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            MatchRef.ValueChanged -= OnMatchUpdate;
        }
        #endregion
    }
}