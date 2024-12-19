namespace Duelo.Common.Util
{
    public class Strings
    {
        public const string DefaultMapName = "Unnamed Map";
    }

    public class SpecialTiles
    {
        public const string DefenderSpawn = "spawn_defender";
        public const string ChallengerSpawn = "spawn_challenger";
    }

    /// <summary>
    /// Animation tags should always be in the format "action:ActionName"
    /// where ActionName is the class name of the action descriptor
    /// </summary>
    public class PlayerAnimation
    {
        public const string AttackCloseRange = "attack:CloseRange";
    }

    public class UIMenuPrefab
    {
        public const string MainMenu = "UiMainMenu";
    }
}