namespace Duelo.Common.Core
{
    using Duelo.Client.Camera;
    using Duelo.Common.Kernel;
    using Duelo.Common.Model;
    using Duelo.Common.Service;
    using Duelo.Gameboard;
    using Ind3x.State;
    using Ind3x.Util;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Data that needs to be accessed across different classes during game execution
    /// </summary>
    public class GlobalState
    {
        #region Common Data
        public static ObservableMatch Match;

        public static IDueloService Services;

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
        /// Logged in player data on device
        /// </summary>
        public static DueloPlayerDto PlayerData;

        public static DueloCamera Camera;

        public static DueloInput Input;
        #endregion

        #region Server Only Data
        /// <summary>
        /// Timer to quit the application if no players join within the expiration time
        /// </summary>
        public static AppQuitTimer AppQuitTimer;
        #endregion
    }
}