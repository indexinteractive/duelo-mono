namespace Duelo.Common.Core
{
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

        public string MatchId;

        public StartupMode StartupType;

        public StartupOptions(StartupMode mode, string matchId, int selfDestructSeconds)
        {
            StartupType = mode;
            MatchId = matchId;
            ServerExpirationSeconds = selfDestructSeconds;
        }

        public override string ToString()
        {
            return $"\n[StartupOptions] {StartupType} configuration -- MatchId: {MatchId}, ServerExpirationSeconds: {ServerExpirationSeconds}";
        }
    }
}