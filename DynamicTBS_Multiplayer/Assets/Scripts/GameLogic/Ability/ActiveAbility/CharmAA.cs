using UnityEngine;

public class CharmAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int charmRange;
    [SerializeField] private int charmDuration;
    [SerializeField] private PatternType aaPattern;

    public static int range;
    public static int duration;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.CHARM; } }

    private CharmAAAction charmAAAction;

    Character character;

    private void Awake()
    {
        range = charmRange;
        duration = charmDuration;
        pattern = aaPattern;

        character = gameObject.GetComponent<Character>();
        charmAAAction = GameObject.Find("ActionRegistry").GetComponent<CharmAAAction>();
    }

    public void Execute()
    {
        charmAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(charmAAAction);
    }

    public int CountActionDestinations()
    {
        return charmAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        charmAAAction.ShowActionPattern(character);
    }
}
