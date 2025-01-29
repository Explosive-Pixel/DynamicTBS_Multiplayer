public enum ActiveAbilityType
{
    BLOCK = 1,
    CHANGE_FLOOR = 2,
    ELECTRIFY = 3,
    HEAL = 4,
    JUMP = 5,
    POWERSHOT = 6,
    TAKE_CONTROL = 7,
    CHARM = 8
}

public interface IActiveAbility
{
    ActiveAbilityType AbilityType { get; }
    int Cooldown { get; }
    void Execute();

    int CountActionDestinations();

    void ShowActionPattern();
}