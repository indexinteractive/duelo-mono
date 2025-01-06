namespace Duelo.Common.Core
{
    using Microsoft.Extensions.Configuration;
    using UnityEngine;

    public enum StartupMode
    {
        Server,
        Client
    }

    /// <summary>
    /// A class that defines startup options for the server
    /// </summary>
    public class StartupOptions
    {
        /// <summary>
        /// If set to a positive non-zero value, the server will exit after the time has elapsed
        /// </summary>
        public int ServerExpirationSeconds;

        /// <summary>
        /// Allows the player id to be overridden for testing purposes
        /// </summary>
        public string PlayerIdOverride;

        public string MatchId;
        public StartupMode StartupType;
        public int MatchSyncTimeoutMs;

        #region Initialization
        public StartupOptions(StartupMode mode, string[] overrides)
        {
            StartupType = mode;

            IConfiguration args = new ConfigurationBuilder()
                .AddCommandLine(System.Environment.GetCommandLineArgs())
                .Build();

            Debug.Log($"[StartupOptions] Command line arguments: {string.Join(',', args.AsEnumerable())}");

            IConfiguration editorArgs = new ConfigurationBuilder()
                .AddCommandLine(overrides)
                .Build();

            Debug.Log($"[StartupOptions] Editor arguments: {string.Join(',', editorArgs.AsEnumerable())}");

            MatchSyncTimeoutMs = 10 * 1000;

            ServerExpirationSeconds = GetArgOrDefault(editorArgs["expire"], args["expire"], 0);
            MatchId = GetArgOrDefault(editorArgs["matchId"], args["matchId"], string.Empty);
            PlayerIdOverride = GetArgOrDefault<string>(editorArgs["playerId"], args["playerId"], null);

            if (StartupType == StartupMode.Server && string.IsNullOrWhiteSpace(MatchId))
            {
                throw new System.Exception($"Invalid matchId: {MatchId}");
            }
        }
        #endregion

        #region Helpers
        private T GetArgOrDefault<T>(string editorValue, string cmdValue, T defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(editorValue))
            {
                return (T)System.Convert.ChangeType(editorValue, typeof(T));
            }

            if (!string.IsNullOrWhiteSpace(cmdValue))
            {
                return (T)System.Convert.ChangeType(cmdValue, typeof(T));
            }

            return (T)System.Convert.ChangeType(defaultValue, typeof(T));
        }
        #endregion

        #region Printing
        public override string ToString()
        {
            if (StartupType == StartupMode.Server)
            {
                return $"\n[StartupOptions] {StartupType} startup -- MatchId: {MatchId}, ServerExpirationSeconds: {ServerExpirationSeconds}";
            }

            return $"\n[StartupOptions] {StartupType} startup -- PlayerId: {PlayerIdOverride}";
        }
        #endregion
    }
}