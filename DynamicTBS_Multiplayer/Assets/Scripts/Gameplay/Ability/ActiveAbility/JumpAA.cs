using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 2
    [SerializeField] private PatternType jumpPattern; // Star
    [SerializeField] private int jumpDistance; // 3

    public static PatternType movePattern;
    public static int distance;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.JUMP; } }
    public int Cooldown { get { return aaCooldown; } }

    private JumpAAAction jumpAAAction;

    Character character;

    private void Awake()
    {
        movePattern = jumpPattern;
        distance = jumpDistance;

        this.character = gameObject.GetComponent<Character>();
        jumpAAAction = GameObject.Find("ActionRegistry").GetComponent<JumpAAAction>();
    }

    public void Execute()
    {
        jumpAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(jumpAAAction);
    }

    public int CountActionDestinations()
    {
        return jumpAAAction.CountActionDestinations(character);
    }

    public void ShowActionPattern()
    {
        jumpAAAction.ShowActionPattern(character);
    }
}