using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int healRange; // 2
    [SerializeField] private int healPoints; // 1
    [SerializeField] private int moveSpeedBuffer; // 1

    public static int range;
    public static int healingPoints;
    public static int moveSpeedBuff;

    public int Cooldown { get { return aaCooldown; } }

    private HealAAAction healAAAction;

    Character character;

    private void Awake()
    {
        range = healRange;
        healingPoints = healPoints;
        moveSpeedBuff = moveSpeedBuffer;

        this.character = gameObject.GetComponent<Character>();
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