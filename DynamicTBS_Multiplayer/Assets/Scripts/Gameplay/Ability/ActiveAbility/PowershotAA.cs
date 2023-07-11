using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowershotAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int powershotDamage; // 1
    [SerializeField] private int powershotSelfDamage; // 1

    public static int damage;
    public static int selfDamage;
    public int Cooldown { get { return aaCooldown; } }

    private PowershotAAAction powershotAAAction;

    CharacterMB character;

    private void Awake()
    {
        damage = powershotDamage;
        selfDamage = powershotSelfDamage;

        this.character = gameObject.GetComponent<CharacterMB>();
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