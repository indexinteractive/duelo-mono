namespace Ind3x.Duelo
{
    public enum StartupType
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

        public StartupType StartupType;

        public StartupOptions(StartupType sType, string matchId, int selfDestructSeconds)
        {
            StartupType = sType;
            MatchId = matchId;
            ServerExpirationSeconds = selfDestructSeconds;
        }

        public override string ToString()
        {
            return $"\n[StartupOptions] {StartupType} configuration -- MatchId: {MatchId}, ServerExpirationSeconds: {ServerExpirationSeconds}";
        }
    }
}