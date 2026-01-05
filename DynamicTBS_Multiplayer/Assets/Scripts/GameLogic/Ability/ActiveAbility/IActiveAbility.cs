using System.ComponentModel;

public enum ActiveAbilityType
{
    [Description("Jump")]
    JUMP = 0,
    [Description("Longshot")]
    LONGSHOT = 1,
    [Description("Ram")]
    RAM = 2,
    [Description("Repair")]
    REPAIR = 3,
    [Description("Hypnotize")]
    HYPNOTIZE = 4,
    [Description("Transfusion")]
    TRANSFUSION = 5
}

static class ActiveAbilityTypeMethods
{
    public static string LocalizedDescription(this ActiveAbilityType activeAbilityType)
    {
        return AbilityLocalization.GetActiveAbilityName(activeAbilityType);
    }
}


public interface IActiveAbility
{
    ActiveAbilityType AbilityType { get; }
    IAction AssociatedAction { get; }
    int Cooldown { get { return 1; } }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}