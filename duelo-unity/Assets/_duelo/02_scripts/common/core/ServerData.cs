namespace Duelo.Common.Core
{
    using Duelo.Common.Model;
    using Duelo.Gameboard;
    using Duelo.Server.GameWorld;
    using Duelo.Server.Match;

    /// <summary>
    /// Data that needs to be shared across different classes on the server
    /// </summary>
    public class ServerData
    {
        /// <summary>
        /// Options passed to the server on startup, either from the editor or the command line
        /// </summary>
        public static StartupOptions StartupOptions;

        /// <summary>
        /// Prefab list loaded when the server starts
        /// </summary>
        public static PrefabList Prefabs;

        /// <summary>
        /// The game world that players inhabit
        /// </summary>
        public static GameWorld World;

        /// <summary>
        /// Match data loaded when the server starts
        /// </summary>
        public static MatchDto MatchDto;

        /// <summary>
        /// Truth object for the current match. All decisions should be made based on this object.
        /// Created on load in <see cref="Server.State.StateRunServerMatch"/>
        /// </summary>
        public static ServerMatch Match;
    }
}