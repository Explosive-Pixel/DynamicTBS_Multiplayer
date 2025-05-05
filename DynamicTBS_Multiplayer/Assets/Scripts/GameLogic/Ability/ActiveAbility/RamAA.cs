using UnityEngine;

public class RamAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int ramDamage;
    [SerializeField] private int ramSelfDamage;
    [SerializeField] private int ramRange;
    [SerializeField] private PatternType ramPattern;

    public static int damage;
    public static int selfDamage;
    public static int range;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.RAM; } }

    private RamAAAction ramAAAction;

    Character character;

    private void Awake()
    {
        damage = ramDamage;
        selfDamage = ramSelfDamage;
        range = ramRange;
        pattern = ramPattern;

        character = gameObject.GetComponent<Character>();
        ramAAAction = GameObject.Find("ActionRegistry").GetComponent<RamAAAction>();
    }

    public void Execute()
    {
        ramAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(ramAAAction);
    }

    public int CountActionDestinations()
    {
        return ramAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        ramAAAction.ShowActionPattern(character);
    }
}
