namespace Duelo.Common.Util
{
    class Strings
    {
        public const string DefaultMapName = "Unnamed Map";
    }

    class SpecialTiles
    {
        public const string DefenderSpawn = "spawn_defender";
        public const string ChallengerSpawn = "spawn_challenger";
    }

    /// <summary>
    /// Animation tags should always be in the format "action:ActionName"
    /// where ActionName is the class name of the action descriptor
    /// </summary>
    class PlayerAnimation
    {
        public const string AttackCloseRange = "attack:CloseRange";
    }
}