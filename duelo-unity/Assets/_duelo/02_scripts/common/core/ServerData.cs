using Duelo.Common.Model;

namespace Duelo.Common.Core
{
    /// <summary>
    /// Data that needs to be shared across different classes on the server
    /// </summary>
    public class ServerData
    {
        public static StartupOptions StartupOptions;
        public static MatchDto MatchDto;
    }
}