using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaDistance; // 1
    [SerializeField] private PatternType aaPattern; // PatternType.Cross
    [SerializeField] private int aaCooldown; // 3
    [SerializeField] private int blockDurationInRounds; // 2

    public static PatternType pattern;
    public static int distance;
    public static int blockingRounds;

    public int Cooldown { get { return aaCooldown; } } // 3

    private BlockAAAction blockAAAction;

    private Character character;
    private int currentBlockCount = 0;

    private bool firstExecution = true;

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
            if (IsBlocking()) return false;
            return defaultIsAttackableBy(character);
        };

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (IsBlocking()) return true;
            return defaultIsDisabled();
        };

        var defaultIsDamageable = character.isDamageable;
        character.isDamageable = (damage) =>
        {
            if (IsBlocking()) return false;
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

    public bool IsBlocking()
    {
        return currentBlockCount > 0;
    }

    public void ActivateBlock()
    {
        if (firstExecution)
        {
            ChangeIsAttackableByOfOtherCharacters();
            firstExecution = false;
        }

        currentBlockCount = blockingRounds + 1;
        ToggleBlockPrefab(true);
        SubscribeEvents();
    }

    public void ReduceBlockCount()
    {
        if (currentBlockCount > 0)
        {
            currentBlockCount -= 1;

            if (currentBlockCount == 0)
            {
                currentBlockCount = 0;
                ToggleBlockPrefab(false);
                UnsubscribeEvents();
            }
        }
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
                    if (currentBlockCount > 0 && attacker != character)
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

    private void ToggleBlockPrefab(bool active)
    {
        // TODO
        UIUtils.FindChildGameObject(character.gameObject, "Block").SetActive(active);
    }

    private void ReduceBlockCounter(PlayerType player)
    {
        if (character.Side == player)
        {
            ReduceBlockCount();
        }
    }

    private void SubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded += ReduceBlockCounter;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnPlayerTurnEnded -= ReduceBlockCounter;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}