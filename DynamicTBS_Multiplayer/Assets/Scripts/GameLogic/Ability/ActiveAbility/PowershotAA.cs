using UnityEngine;

public class PowershotAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int powershotDamage; // 1
    [SerializeField] private int powershotSelfDamage; // 1

    public static int damage;
    public static int selfDamage;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.POWERSHOT; } }
    private PowershotAAAction powershotAAAction;

    Character character;

    private void Awake()
    {
        damage = powershotDamage;
        selfDamage = powershotSelfDamage;

        this.character = gameObject.GetComponent<Character>();
        powershotAAAction = GameObject.Find("ActionRegistry").GetComponent<PowershotAAAction>();
    }

    public void Execute()
    {
        powershotAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(powershotAAAction);
    }

    public int CountActionDestinations()
    {
        return powershotAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        powershotAAAction.ShowActionPattern(character);
    }
}