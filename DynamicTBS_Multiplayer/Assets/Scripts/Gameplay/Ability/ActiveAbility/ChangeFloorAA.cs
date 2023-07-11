using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown;
    [SerializeField] private int changeFloorRadius; // 2
    [SerializeField] private int changeFloorRadiusWithInhabitants; // 1

    public static int radius;
    public static int radiusWithInhabitants;

    public int Cooldown { get { return aaCooldown; } }
    private ChangeFloorAAAction changeFloorAAAction;

    CharacterMB character;

    private void Awake()
    {
        radius = changeFloorRadius;
        radiusWithInhabitants = changeFloorRadiusWithInhabitants;

        this.character = gameObject.GetComponent<CharacterMB>();
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
