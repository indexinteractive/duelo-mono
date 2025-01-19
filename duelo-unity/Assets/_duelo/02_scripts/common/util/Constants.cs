namespace Duelo.Common.Util
{
    public class ExitCode
    {
        public const int InvalidMatch = 400;
        public const int MatchNotFound = 404;
        public const int MapNotFound = 406;
        public const int DuplicatePrefab = 429;
        public const int ServerJsonNotFound = 501;
        public const int UnityServicesFailed = 503;
    }

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
        public const string ChooseActionPartial = "UiChooseActionPartial";
        public const string ChooseGamertag = "UiChooseGamertag";
        public const string ChooseMovementPartial = "UiChooseMovementPartial";
        public const string DebugMatch = "UiDebugMatch";
        public const string MainMenu = "UiMainMenu";
        public const string MatchHud = "UiMatchHud";
        public const string Matchmaking = "UiMatchmaking";
        public const string PopupMessage = "UiPopupMessage";
        public const string ProfileCreate = "UiProfileCreate";
        public const string Profiles = "UiProfiles";
        public const string ProfileSelect = "UiProfileSelect";
    }
}