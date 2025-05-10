using UnityEngine;

public class RepairAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int repairRange;
    [SerializeField] private PatternType repairPattern;

    public static int range;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.REPAIR; } }

    private RepairAAAction repairAAAction;

    Character character;

    private void Awake()
    {
        range = repairRange;
        pattern = repairPattern;

        character = gameObject.GetComponent<Character>();
        repairAAAction = GameObject.Find("ActionRegistry").GetComponent<RepairAAAction>();
    }

    public void Execute()
    {
        repairAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(repairAAAction);
    }

    public int CountActionDestinations()
    {
        return repairAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        repairAAAction.ShowActionPattern(character);
    }
}
