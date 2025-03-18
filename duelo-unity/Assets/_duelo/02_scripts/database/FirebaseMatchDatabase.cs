namespace Duelo.Database
{
    using System;
    using System.Collections.Generic;
    using Cysharp.Threading.Tasks;
    using Duelo.Common.Model;
    using Firebase.Database;
    using Ind3x.Util;
    using Newtonsoft.Json;
    using UnityEngine;

    public class FirebaseMatchDatabase : IMatchDatabase
    {
        #region Refs
        public readonly DatabaseReference MatchRef;

        public DatabaseReference RoundRef(int round) => MatchRef.Child($"{SchemaMatchField.Rounds}/{round}");
        public DatabaseReference ConnectionRef(PlayerRole role) => MatchRef.Child($"{SchemaMatchField.Players}/{role.ToString().ToLower()}/connection");
        public DatabaseReference SyncRef(PlayerRole role) => MatchRef.Child($"{SchemaMatchField.Sync}/{role.ToString().ToLower()}");
        public DatabaseReference MovementRef(int round, PlayerRole role) => RoundRef(round).Child($"{SchemaMatchRoundField.Movement}/{role.ToString().ToLower()}");
        public DatabaseReference ActionsRef(int round, PlayerRole role) => RoundRef(round).Child($"{SchemaMatchRoundField.Action}/{role.ToString().ToLower()}");
        #endregion

        #region Initialization
        public FirebaseMatchDatabase(string matchId)
        {
            MatchRef = FirebaseInstance.Instance.Db.GetReference(SchemaCollection.Match).Child(matchId);
        }
        #endregion

        public async UniTask PublishMatch(MatchDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            Debug.Log($"[FirebaseMatchDatabase] Pushing match data to firebase -- {json}");

            await MatchRef.SetRawJsonValueAsync(json).AsUniTask();
        }

        public void Dispose() { }

        public async UniTask UpdateConnectionStatus(PlayerRole role, string status)
        {
            await ConnectionRef(role).SetValueAsync(status);
        }

        public async UniTask PublishServerSyncState(SyncStateDto dto)
        {
            string json = JsonConvert.SerializeObject(dto);
            Debug.Log($"[ServerMatch] Publishing sync state to firebase -- {json}");
            await MatchRef.Child(SchemaMatchField.Sync).SetRawJsonValueAsync(json);
        }

        public async UniTask UpdateClientState(PlayerRole role, MatchState state)
        {
            await SyncRef(role).SetValueAsync(state.ToString());
        }

        public Action Subscribe(string path, Action<string> callback)
        {
            void listener(object sender, ValueChangedEventArgs e)
            {
                if (e.DatabaseError != null)
                {
                    Debug.LogError($"[FirebaseMatchDatabase] Error reading data at path \"{path}\"");
                    Debug.LogError(e.DatabaseError);
                    return;
                }

                if (!e.Snapshot.Exists)
                {
                    Debug.LogWarning($"[FirebaseMatchDatabase] No data at path \"{path}\"");
                    return;
                }

                string jsonValue = "";
                try
                {
                    jsonValue = e.Snapshot.GetRawJsonValue();
                    Debug.Log($"[FirebaseMatchDatabase] Value changed: {jsonValue}");

                    callback(jsonValue);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[FirebaseMatchDatabase] Error parsing firebase value: {ex.Message}");
                    Debug.Log(jsonValue);
                }
            }

            MatchRef.Child(path).ValueChanged += listener;
            return () => MatchRef.Child(path).ValueChanged -= listener;
        }

        #region Round updates
        public UniTask PublishRound(int round, MatchRoundDto dto)
        {
            string json = JsonConvert.SerializeObject(dto);

            return RoundRef(round)
                .SetRawJsonValueAsync(json)
                .AsUniTask();
        }

        public UniTask UpdateRound(int round, Dictionary<string, object> updates)
        {
            return RoundRef(round)
                .UpdateChildrenAsync(updates)
                .AsUniTask();
        }
        #endregion

        #region Movement Phase
        public UniTask BeginMovementPhase(int roundNumber, MovementPhaseDto dto)
        {
            string update = JsonConvert.SerializeObject(dto);

            return RoundRef(roundNumber)
                .Child(SchemaMatchRoundField.Movement)
                .SetRawJsonValueAsync(update)
                .AsUniTask();
        }

        public UniTask DispatchMovement(int round, PlayerRole role, PlayerRoundMovementDto dto)
        {
            string json = JsonConvert.SerializeObject(dto);
            Debug.Log($"[ClientMatch] Dispatching movement for {role}: {json}");

            return MovementRef(round, role)
                .SetRawJsonValueAsync(json)
                .AsUniTask();
        }
        #endregion

        #region Action Phase
        public UniTask BeginActionPhase(int round, ActionPhaseDto dto)
        {
            string update = JsonConvert.SerializeObject(dto);

            return RoundRef(round)
                .Child(SchemaMatchRoundField.Action)
                .SetRawJsonValueAsync(update)
                .AsUniTask();
        }

        public UniTask DispatchAction(int round, PlayerRole role, PlayerRoundActionDto dto)
        {
            string json = JsonConvert.SerializeObject(dto);

            return ActionsRef(round, role)
                .SetRawJsonValueAsync(json)
                .AsUniTask();
        }
        #endregion
    }
}