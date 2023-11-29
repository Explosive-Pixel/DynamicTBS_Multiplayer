using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaDistance; // 1
    [SerializeField] private PatternType aaPattern; // PatternType.Cross
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int blockDurationInRounds; // 2

    [SerializeField] private GameObject blockGameObject;

    public static PatternType pattern;
    public static int distance;
    public static int blockingRounds;

    public ActiveAbilityType AbilityType { get { return ActiveAbilityType.BLOCK; } }
    public int Cooldown { get { return aaCooldown; } } // 3

    private BlockAAAction blockAAAction;

    private Character character;

    private bool firstExecution = true;

    private bool isBlocking;

    private void Awake()
    {
        distance = aaDistance;
        pattern = aaPattern;
        blockingRounds = blockDurationInRounds;

        this.character = gameObject.GetComponent<Character>();
        blockAAAction = GameObject.Find("ActionRegistry").GetComponent<BlockAAAction>();

        var defaultIsAttackableBy = character.isAttackableBy;
        character.isAttackableBy = (attacker) =>
        {
            if (isBlocking) return false;
            return defaultIsAttackableBy(character);
        };

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (isBlocking) return true;
            return defaultIsDisabled();
        };

        var defaultIsDamageable = character.isDamageable;
        character.isDamageable = (damage) =>
        {
            if (isBlocking) return false;
            return defaultIsDamageable(damage);
        };
    }

    public void Execute()
    {
        blockAAAction.CreateActionDestinations(character);
        ActionRegistry.Register(blockAAAction);
    }

    public int CountActionDestinations()
    {
        return blockAAAction.CountActionDestinations(character);
    }

    public void ActivateBlock()
    {
        if (firstExecution)
        {
            ChangeIsAttackableByOfOtherCharacters();
            firstExecution = false;
        }

        ToggleBlock(true);
        RoundBasedCounter.Create(gameObject, blockingRounds, () => ToggleBlock(false));
    }

    private void ToggleBlock(bool active)
    {
        blockGameObject.SetActive(active);
        isBlocking = active;
    }

    public void ShowActionPattern()
    {
        blockAAAction.ShowActionPattern(character);
    }

    private void ChangeIsAttackableByOfOtherCharacters()
    {
        foreach (Character c in CharacterManager.GetAllLivingCharacters())
        {
            var cDefaultIsAttackableBy = c.isAttackableBy;
            c.isAttackableBy = (attacker) =>
                {
                    if (isBlocking && attacker != character)
                    {
                        Tile attackerTile = Board.GetTileByCharacter(attacker);
                        Tile cTile = Board.GetTileByCharacter(c);
                        Tile characterTile = Board.GetTileByCharacter(character);
                        if (attackerTile.Row == cTile.Row && cTile.Row == characterTile.Row)
                        {
                            if (attackerTile.Column < characterTile.Column && characterTile.Column < cTile.Column)
                                return false;
                            else if (attackerTile.Column > characterTile.Column && characterTile.Column > cTile.Column)
                                return false;
                        }
                        else if (attackerTile.Column == cTile.Column && cTile.Column == characterTile.Column)
                        {
                            if (attackerTile.Row < characterTile.Row && characterTile.Row < cTile.Row)
                                return false;
                            else if (attackerTile.Row > characterTile.Row && characterTile.Row > cTile.Row)
                                return false;
                        }

                    }
                    return cDefaultIsAttackableBy(attacker);
                };
        }
    }
}