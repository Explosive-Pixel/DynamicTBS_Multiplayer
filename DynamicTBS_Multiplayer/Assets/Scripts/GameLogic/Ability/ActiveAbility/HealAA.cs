using UnityEngine;

public class HealAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int healRange; // 2
    [SerializeField] private int healPoints; // 1
    [SerializeField] private int moveSpeedBuffer; // 1
    [SerializeField] private PatternType aaPattern;

    [SerializeField] private GameObject speedBuff;

    public static int range;
    public static int healingPoints;
    public static int moveSpeedBuff;
    public static string speedBuffName;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.HEAL; } }

    private HealAAAction healAAAction;

    Character character;

    private void Awake()
    {
        range = healRange;
        healingPoints = healPoints;
        moveSpeedBuff = moveSpeedBuffer;
        speedBuffName = speedBuff.name;
        pattern = aaPattern;

        character = gameObject.GetComponent<Character>();
        healAAAction = GameObject.Find("ActionRegistry").GetComponent<HealAAAction>();
    }

    public void Execute()
    {
        healAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(healAAAction);
    }

    public int CountActionDestinations()
    {
        return healAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        healAAAction.ShowActionPattern(character);
    }
}