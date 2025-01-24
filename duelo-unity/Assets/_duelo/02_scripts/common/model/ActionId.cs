namespace Duelo.Common.Model
{
    /// <summary>
    /// Used in <see cref="CharacterTesting"/> to select actions in the editor.
    /// Would be nice to have a dropdown editor component for this so that
    /// it could automatically populate the dropdown with the available actions.
    /// </summary>
    public enum ActionDropdownItem
    {
        None = -1,
        Walk = 10,
        Run = 20,
        Hover = 30,
        CloseRange = 110,
        CannonFire = 120,
        Shield = 210,
    }

    public class ActionId
    {
        public const int None = -1;

        public static bool IsMovementAction(int actionId)
        {
            return actionId >= 10 && actionId < 100;
        }

        public static bool IsAttackAction(int actionId)
        {
            return actionId >= 100 && actionId < 200;
        }

        public static bool IsDefenseAction(int actionId)
        {
            return actionId >= 200 && actionId < 300;
        }
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