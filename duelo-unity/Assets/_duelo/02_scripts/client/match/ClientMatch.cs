namespace Duelo.Client.Match
{
    using System;
    using System.Linq;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Database;
    using Newtonsoft.Json;
    using UnityEngine;

    public class ClientMatch : ObservableMatch, IClientMatch
    {
        #region Match Properties
        public readonly IMatchDatabase Db;
        public MatchPlayer DevicePlayer => Players.FirstOrDefault(p => p.Value.IsDevicePlayer).Value;

        private Action _dataChangedSub;
        #endregion

        #region Initialization
        public ClientMatch(MatchDto match, IMatchDatabase db) : base(match)
        {
            Db = db;
        }
        #endregion

        #region Players
        /// <summary>
        /// Called by <see cref="Client.Screen.PlayMatchScreen.OnEnter"/> to set the player's
        /// connection status to "online" and thereby "joining" the match.
        /// </summary>
        public async UniTask JoinMatch()
        {
            var player = DevicePlayer;
            Debug.Log($"[ClientMatch] {player.ProfileDto.Gamertag} Joining match {MatchId}");
            if (player != null)
            {
                Players[player.Role].Status.Value = ConnectionStatus.Online;
                await Db.UpdateConnectionStatus(player.Role, ConnectionStatus.Online.ToString());

                _dataChangedSub = Db.Subscribe("", OnFirebaseDataChanged);
                Debug.Log($"[ClientMatch] {player.ProfileDto.Gamertag} Listening for match updates");
            }
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
        public async UniTask PublishSyncState(MatchState state)
        {
            Debug.Log($"[ClientMatch] Publishing sync state: {state} for {DevicePlayer.Role}");

            SyncState.Update(DevicePlayer.Role, state);
            await Db.UpdateClientState(DevicePlayer.Role, state);
        }
        #endregion

        #region Data Update
        /// <summary>
        /// Avoid using async void, UNLESS it's an event handler, then we can forgive
        /// </summary>
        private void OnFirebaseDataChanged(string json)
        {
            MatchDto newData = JsonConvert.DeserializeObject<MatchDto>(json);
            Debug.Log($"[ClientMatch] ({DevicePlayer.Role}) Match update -- {json}");

            Db.UpdateClientState(DevicePlayer.Role, newData.SyncState.Server)
                .ContinueWith(() => UpdateFromDto(newData))
                .Forget();
        }
        #endregion

        #region Round Updates
        public void DispatchMovement(int actionId, Vector3 position)
        {
            var dto = new PlayerRoundMovementDto
            {
                ActionId = actionId,
                TargetPosition = position
            };

            var round = CurrentRound.CurrentValue;
            round.PlayerMovement[DevicePlayer.Role] = dto;

            Db.DispatchMovement(round.RoundNumber, DevicePlayer.Role, dto)
                .ContinueWith(() => Debug.Log($"[ClientMatch] Dispatched movement for {DevicePlayer.Role}"))
                .Forget();
        }

        public void DispatchAttack(int actionId)
        {
            var dto = new PlayerRoundActionDto()
            {
                ActionId = actionId
            };

            var round = CurrentRound.CurrentValue;
            round.PlayerAction[DevicePlayer.Role] = dto;

            Db.DispatchAction(round.RoundNumber, DevicePlayer.Role, dto)
                .ContinueWith(() => Debug.Log($"[ClientMatch] Dispatched attack for {DevicePlayer.Role}"))
                .Forget();
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
            SpawnPlayer(PlayerRole.Challenger, InitialDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, InitialDto.Players.Defender);
        }

        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            GameObject prefab = GlobalState.Prefabs.CharacterLookup[playerDto.Profile.CharacterUnitId];

            var spawnPoint = GlobalState.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            // var playerRef = MatchRef.Child("players").Child(role.ToString().ToLower());
            matchPlayer.Initialize(this, role, playerDto);
            Debug.Log($"[ClientMatch] Character spawned for {role} at {gameObject.transform.position}");

            Players.Add(role, matchPlayer);
        }
        #endregion

        #region IDisposable
        public override void Dispose()
        {
            _dataChangedSub?.Invoke();
        }
        #endregion
    }
}