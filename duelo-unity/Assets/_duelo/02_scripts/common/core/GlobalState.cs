namespace Duelo.Common.Core
{
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Gameboard;
    using Duelo.Server.Match;
    using Ind3x.State;
    using Ind3x.Util;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Data that needs to be accessed across different classes during game execution
    /// </summary>
    public class GlobalState
    {
        #region Common Data
        public static Firebase.Database.DatabaseReference MatchRef
        {
            get
            {
                if (StartupOptions == null)
                {
                    return null;
                }

                string matchId = StartupOptions.StartupType == StartupMode.Server ? ServerMatch.MatchId : ClientMatch.MatchId;
                return FirebaseInstance.Instance.Db.GetReference(DueloCollection.Match.ToString().ToLower()).Child(matchId);
            }
        }

        /// <summary>
        /// Options passed to the server on startup, either from the editor or the command line
        /// </summary>
        public static StartupOptions StartupOptions;

        /// <summary>
        /// Runs execution phase logic for matches
        /// </summary>
        public static MatchKernel Kernel;

        /// <summary>
        /// Prefab list loaded when the server starts
        /// </summary>
        public static PrefabList Prefabs;

        public static StateMachine StateMachine;

        /// <summary>
        /// The game world that players inhabit
        /// </summary>
        public static DueloMap Map;
        #endregion

        #region Client Only Data
        /// <summary>
        /// Truth object for the current match. All decisions should be made based on this object.
        /// Created if a match is found in <see cref="Client.Screen.MatchmakingScreen.Resume"/>
        /// </summary>
        public static IClientMatch ClientMatch;

        /// <summary>
        /// Logged in player data on device
        /// </summary>
        public static DueloPlayerDto PlayerData;

        public static DueloCamera Camera;

        public static DueloInput Input;
        #endregion

        #region Server Only Data
        /// <summary>
        /// Truth object for the current match. All decisions should be made based on this object.
        /// Created on load in <see cref="Server.State.StateRunServerMatch"/>
        /// </summary>
        public static IServerMatch ServerMatch;

        /// <summary>
        /// Timer to quit the application if no players join within the expiration time
        /// </summary>
        public static AppQuitTimer AppQuitTimer;
        #endregion
    }
}