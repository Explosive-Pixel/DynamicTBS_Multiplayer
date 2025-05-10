using UnityEngine;

public class TransfusionAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int transfusionRange;
    [SerializeField] private PatternType transfusionPattern;

    public static int range;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.TRANSFUSION; } }

    private TransfusionAAAction transfusionAAAction;

    Character character;

    private void Awake()
    {
        range = transfusionRange;
        pattern = transfusionPattern;

        character = gameObject.GetComponent<Character>();
        transfusionAAAction = GameObject.Find("ActionRegistry").GetComponent<TransfusionAAAction>();
    }

    public void Execute()
    {
        transfusionAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(transfusionAAAction);
    }

    public int CountActionDestinations()
    {
        return transfusionAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        transfusionAAAction.ShowActionPattern(character);
    }
}
