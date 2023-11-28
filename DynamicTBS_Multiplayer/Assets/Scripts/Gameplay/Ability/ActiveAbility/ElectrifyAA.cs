using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifyAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int electrifyRadius; // 1
    [SerializeField] private int electrifyDuration; // 2
    [SerializeField] private int electrifyStunDuration; // 1
    [SerializeField] private GameObject electrifyPrefab;

    public static int radius;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.ELECTRIFY; } }
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

    public void ElectrifyTile(Tile tile)
    {
        ElectrifiedState.Create(tile.gameObject, electrifyDuration, electrifyStunDuration, electrifyPrefab);
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
