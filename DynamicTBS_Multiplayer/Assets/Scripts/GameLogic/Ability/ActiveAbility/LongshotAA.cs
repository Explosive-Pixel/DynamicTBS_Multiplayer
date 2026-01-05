using UnityEngine;

public class LongshotAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int longshotDamage; // 1
    [SerializeField] private int longshotSelfDamage; // 1
    [SerializeField] private PatternType longshotPattern;

    public static int damage;
    public static int selfDamage;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.LONGSHOT; } }
    public IAction AssociatedAction { get { return longshotAAAction; } }
    private LongshotAAAction longshotAAAction;

    Character character;

    private void Awake()
    {
        damage = longshotDamage;
        selfDamage = longshotSelfDamage;
        pattern = longshotPattern;

        this.character = gameObject.GetComponent<Character>();
        longshotAAAction = GameObject.Find("ActionRegistry").GetComponent<LongshotAAAction>();
    }

    public void Execute()
    {
        longshotAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(longshotAAAction);
    }

    public int CountActionDestinations()
    {
        return longshotAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        longshotAAAction.ShowActionPattern(character);
    }
}
