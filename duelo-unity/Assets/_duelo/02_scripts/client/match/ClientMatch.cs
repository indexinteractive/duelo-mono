namespace Duelo.Client.Match
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Duelo.Common.Core;
    using Duelo.Common.Match;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Firebase.Database;
    using Newtonsoft.Json;
    using UnityEditor;
    using UnityEngine;

    public class ClientMatch
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
            _ref = FirebaseDatabase.DefaultInstance.GetReference(DueloCollection.Match.ToString().ToLower()).Child(match.MatchId);

            _ref.ValueChanged += OnMatchUpdate;
        }
        #endregion

        #region Data Update
        private void OnMatchUpdate(object sender, ValueChangedEventArgs eventArgs)
        {
            if (eventArgs.DatabaseError != null)
            {
                Debug.Log(eventArgs.DatabaseError.Message);
                return;
            }

            try
            {
                string jsonValue = eventArgs.Snapshot.GetRawJsonValue();

                MatchDto previousDto = CurrentDto;
                CurrentDto = JsonConvert.DeserializeObject<MatchDto>(jsonValue);

                if (previousDto.State != CurrentDto.State)
                {
                    OnStateChange?.Invoke(CurrentDto, previousDto);

                    foreach (var player in Players)
                    {
                        player.Value.OnMatchStateChanged(CurrentDto.State);
                    }
                }
            }
            catch (System.Exception error)
            {
                Debug.Log("Error: " + error.Message);
            }
        }
        #endregion

        #region Asset Loading
        public void LoadAssets()
        {
            SpawnPlayer(PlayerRole.Challenger, CurrentDto.Players.Challenger);
            SpawnPlayer(PlayerRole.Defender, CurrentDto.Players.Defender);
        }

        public void SpawnPlayer(PlayerRole role, MatchPlayerDto playerDto)
        {
            string prefabPath = $"Assets/_duelo/03_character/{playerDto.Profile.CharacterUnitId}.prefab";

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

            if (prefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                Application.Quit();
            }

            var spawnPoint = GameData.Map.SpawnPoints[role];
            var gameObject = GameObject.Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);

            Debug.Log($"Character spawned for {role} at {gameObject.transform.position}");

            var matchPlayer = gameObject.GetComponent<MatchPlayer>();

            matchPlayer.Initialize(CurrentDto.MatchId, role, playerDto);

            Players.Add(role, matchPlayer);
        }
    }
    #endregion
}