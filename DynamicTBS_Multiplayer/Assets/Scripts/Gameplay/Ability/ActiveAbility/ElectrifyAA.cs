using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifyAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int electrifyRadius; // 1

    public static int radius;

    public int Cooldown { get { return aaCooldown; } }

    Character character;
    private ElectrifyAAAction electrifyAAAction;

    private void Awake()
    {
        radius = electrifyRadius;

        this.character = gameObject.GetComponent<Character>();
        electrifyAAAction = GameObject.Find("ActionRegistry").GetComponent<ElectrifyAAAction>();
    }

    public void Execute()
    {
        electrifyAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(electrifyAAAction);
    }

    public int CountActionDestinations()
    {
        return electrifyAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        electrifyAAAction.ShowActionPattern(character);
    }
}
