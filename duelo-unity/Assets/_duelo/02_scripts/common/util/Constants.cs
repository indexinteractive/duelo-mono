namespace Duelo.Common.Util
{
    public class Strings
    {
        public const string DefaultMapName = "Unnamed Map";
        public const string LabelLoading = "Loading...";
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

    public class UIViewPrefab
    {
        public const string MainMenu = "UiMainMenu";
        public const string MatchMaking = "UiMatchMaking";
        public const string UiPopupMessage = "UiPopupMessage";
        public const string MatchHud = "UiMatchHud";
        public const string ChooseMovementPartial = "UiChooseMovementPartial";
        public const string ChooseActionPartial = "UiChooseActionPartial";
        public const string Profiles = "UiProfiles";
        public const string ProfileCreate = "UiProfileCreate";
        public const string ChooseGamertag = "UiChooseGamertag";
        public const string ProfileSelect = "UiProfileSelect";
    }
}