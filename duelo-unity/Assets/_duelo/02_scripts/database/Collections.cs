namespace Duelo.Database
{
    public class SchemaCollection
    {
        public static string Match = "match";
        public static string Map = "map";
        public static string Player = "player";
    }

    public class SchemaMatchField
    {
        public static string Players = "players";
        public static string Sync = "sync";
        public static string Rounds = "rounds";
    }

    public class SchemaMatchRoundField
    {
        public static string Movement = "movement";
        public static string Action = "action";
    }
}