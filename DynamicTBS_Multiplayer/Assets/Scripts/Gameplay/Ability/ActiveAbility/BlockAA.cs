using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAA : IActiveAbility
{
    public static int blockingRounds = 2;
    
    public static PatternType pattern = PatternType.Cross;
    public static int distance = 1;

    public int Cooldown { get { return 3; } }

    private BlockAAAction blockAAAction;

    private Character character;
    private int currentBlockCount = 0;

    private bool firstExecution = true;

    public BlockAA(Character character)
    {
        this.character = character;
        blockAAAction = GameObject.Find("ActionRegistry").GetComponent<BlockAAAction>();

        var defaultIsAttackableBy = character.isAttackableBy;
        character.isAttackableBy = (attacker) => {
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
         character.isDamageable = (damage) => {
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
        foreach(Character c in CharacterHandler.GetAllLivingCharacters())
        {
            var cDefaultIsAttackableBy = c.isAttackableBy;
            c.isAttackableBy = (attacker) =>
                {
                    if (currentBlockCount > 0 && attacker != character)
                    {
                        Tile attackerTile = Board.GetTileByCharacter(attacker);
                        Tile cTile = Board.GetTileByCharacter(c);
                        Tile characterTile = Board.GetTileByCharacter(character);
                        if(attackerTile.GetRow() == cTile.GetRow() && cTile.GetRow() == characterTile.GetRow())
                        {
                            if (attackerTile.GetColumn() < characterTile.GetColumn() && characterTile.GetColumn() < cTile.GetColumn())
                                return false;
                            else if (attackerTile.GetColumn() > characterTile.GetColumn() && characterTile.GetColumn() > cTile.GetColumn())
                                return false;
                        }
                        else if (attackerTile.GetColumn() == cTile.GetColumn() && cTile.GetColumn() == characterTile.GetColumn())
                        {
                            if (attackerTile.GetRow() < characterTile.GetRow() && characterTile.GetRow() < cTile.GetRow())
                                return false;
                            else if (attackerTile.GetRow() > characterTile.GetRow() && characterTile.GetRow() > cTile.GetRow())
                                return false;
                        }

                    }
                    return cDefaultIsAttackableBy(attacker);
                };
        }
    }

    private void ToggleBlockPrefab(bool active)
    {
        UIUtils.FindChildGameObject(character.GetCharacterGameObject(), "Block").SetActive(active);
    }

    private void ReduceBlockCounter(Player player)
    {
        if (character.GetSide() == player)
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

    ~BlockAA()
    {
        UnsubscribeEvents();
    }
}