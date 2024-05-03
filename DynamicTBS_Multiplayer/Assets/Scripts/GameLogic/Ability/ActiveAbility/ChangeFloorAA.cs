using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int changeFloorRange; // 2
    [SerializeField] private int changeFloorRangeWithInhabitants; // 1
    [SerializeField] private PatternType aaPattern;

    public static int range;
    public static int rangeWithInhabitants;
    public static PatternType pattern;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.CHANGE_FLOOR; } }
    public int Cooldown { get { return aaCooldown; } }
    private ChangeFloorAAAction changeFloorAAAction;

    Character character;

    private void Awake()
    {
        range = changeFloorRange;
        rangeWithInhabitants = changeFloorRangeWithInhabitants;
        pattern = aaPattern;

        this.character = gameObject.GetComponent<Character>();
        changeFloorAAAction = GameObject.Find("ActionRegistry").GetComponent<ChangeFloorAAAction>();
    }

    public void Execute()
    {
        changeFloorAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(changeFloorAAAction);
    }

    public int CountActionDestinations()
    {
        return changeFloorAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        changeFloorAAAction.ShowActionPattern(character);
    }
}
