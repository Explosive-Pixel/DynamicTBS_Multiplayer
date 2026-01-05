using UnityEngine;

public class HypnotizeAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int hypnotizeRange;
    [SerializeField] private PatternType hypnotizePattern;

    public static int range;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.HYPNOTIZE; } }
    public IAction AssociatedAction { get { return hypnotizeAAAction; } }

    private HypnotizeAAAction hypnotizeAAAction;

    Character character;

    private void Awake()
    {
        range = hypnotizeRange;
        pattern = hypnotizePattern;

        character = gameObject.GetComponent<Character>();
        hypnotizeAAAction = GameObject.Find("ActionRegistry").GetComponent<HypnotizeAAAction>();
    }

    public void Execute()
    {
        hypnotizeAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(hypnotizeAAAction);
    }

    public int CountActionDestinations()
    {
        return hypnotizeAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        hypnotizeAAAction.ShowActionPattern(character);
    }
}
