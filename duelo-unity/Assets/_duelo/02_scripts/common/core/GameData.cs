namespace Duelo.Common.Core
{
    using Duelo.Client.Camera;
    using Duelo.Client.Match;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using Duelo.Server.Match;
    using Ind3x.State;

    /// <summary>
    /// Data that needs to be accessed across different classes during game execution
    /// </summary>
    public class GameData
    {
        #region Common Data
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
        /// Created if a match is found in <see cref="Client.Screen.MainMenuScreen"/>
        /// </summary>
        public static ClientMatch ClientMatch;

        /// <summary>
        /// Logged in player data on device
        /// </summary>
        public static DueloPlayerDto PlayerData;

        /// <summary>
        /// Current profile in use by the player
        /// </summary>
        public static PlayerProfileDto ActiveProfile;

        public static DueloCamera Camera;
        #endregion

        #region Server Only Data
        /// <summary>
        /// Truth object for the current match. All decisions should be made based on this object.
        /// Created on load in <see cref="Server.State.StateRunServerMatch"/>
        /// </summary>
        public static ServerMatch ServerMatch;
        #endregion
    }
}