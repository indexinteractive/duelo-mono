namespace Duelo.Common.Model
{
    public class ActionId
    {
        public const int None = -1;
    }

    public class MovementActionId : ActionId
    {
        public const int Walk = 10;
        public const int Run = 20;
        public const int Hover = 30;
    }

    public class AttackActionId : ActionId
    {
        public const int CloseRange = 110;
        public const int CannonFire = 120;
    }

    public class DefenseActionId : ActionId
    {
        public const int Shield = 210;
    }
}