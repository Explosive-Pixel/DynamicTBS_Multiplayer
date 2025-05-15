using System.ComponentModel;

public enum ActiveAbilityType
{
    [Description("Block")]
    BLOCK = 1,
    [Description("Change Floor")]
    CHANGE_FLOOR = 2,
    [Description("Electrify")]
    ELECTRIFY = 3,
    [Description("Heal")]
    HEAL = 4,
    [Description("Jump")]
    JUMP = 5,
    [Description("Powershot")]
    POWERSHOT = 6,
    [Description("Take Control")]
    TAKE_CONTROL = 7,
    [Description("Charm")]
    CHARM = 8,
    [Description("Longshot")]
    LONGSHOT = 9,
    [Description("Ram")]
    RAM = 10,
    [Description("Repair")]
    REPAIR = 11,
    [Description("Hypnotize")]
    HYPNOTIZE = 12,
    [Description("Transfusion")]
    TRANSFUSION = 13
}

public interface IActiveAbility
{
    ActiveAbilityType AbilityType { get; }
    int Cooldown { get { return 1; } }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}