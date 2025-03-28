namespace Duelo.Common.Core
{
    using System;
    using Microsoft.Extensions.Configuration;
    using UnityEngine;

    public enum StartupMode
    {
        Server,
        Client
    }

    /// <summary>
    /// A class that defines startup options for the game
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

            IConfiguration editorArgs = new ConfigurationBuilder()
#if DUELO_LOCAL
                .AddCommandLine(overrides)
#endif
                .Build();
            Debug.Log($"[StartupOptions] Editor arguments: {string.Join(',', editorArgs.AsEnumerable())}");

            IConfiguration args = new ConfigurationBuilder()
                .AddCommandLine(System.Environment.GetCommandLineArgs())
                .Build();

            Debug.Log($"[StartupOptions] Command line arguments: {string.Join(',', args.AsEnumerable())}");

            MatchSyncTimeoutMs = 10 * 1000;

            ServerExpirationSeconds = GetArgOrDefault(args["expire"], editorArgs["expire"], 60);
            MatchId = GetArgOrDefault(args["matchId"], editorArgs["matchId"], Guid.NewGuid().ToString());
            PlayerIdOverride = GetArgOrDefault<string>(args["playerId"], editorArgs["playerId"], null);
        }
        #endregion

        #region Helpers
        private T GetArgOrDefault<T>(string priority, string fallback, T defaultValue)
        {
            if (!string.IsNullOrWhiteSpace(priority))
            {
                return (T)System.Convert.ChangeType(priority, typeof(T));
            }

            if (!string.IsNullOrWhiteSpace(fallback))
            {
                return (T)System.Convert.ChangeType(fallback, typeof(T));
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