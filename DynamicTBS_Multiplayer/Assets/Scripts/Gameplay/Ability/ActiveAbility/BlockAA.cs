using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAA : IActiveAbility
{
    public static int blockingRounds = 2;

    public int Cooldown { get { return 3; } }

    private Character character;
    private int currentBlockCount = 0;
    private GameObject blockGameObject = null;

    private bool firstExecution = true;

    public BlockAA(Character character)
    {
        this.character = character;

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
        if (firstExecution)
        {
            ChangeIsAttackableByOfOtherCharacters();
            firstExecution = false;
        }
        ActivateBlock();

    }

    public Character GetCharacter()
    {
        return character;
    }

    public bool IsBlocking()
    {
        return currentBlockCount > 0;
    }

    public void ActivateBlock()
    {
        currentBlockCount = blockingRounds + 1;
        blockGameObject = CreateBlockGameObject();
        GameplayEvents.OnPlayerTurnEnded += ReduceBlockCounter;

        GameplayEvents.ActionFinished(character, ActionType.ActiveAbility, character.GetCharacterGameObject().transform.position, null);
    }

    public void ReduceBlockCount()
    {
        if (currentBlockCount > 0)
        {
            currentBlockCount -= 1;

            if (currentBlockCount == 0)
            {
                GameObject.Destroy(blockGameObject);
                blockGameObject = null;
            }
        }
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

    private GameObject CreateBlockGameObject()
    {
        GameObject blockGameObject = new GameObject();
        blockGameObject.name = "TmpBlockOverlay";

        Vector3 position = character.GetCharacterGameObject().transform.position;
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = blockGameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = SpriteManager.TANK_BLOCK_FRAME_SPRITE;
        blockGameObject.transform.position = new Vector3(position.x, position.y, position.z - 0.1f);
        blockGameObject.transform.rotation = startRotation;
        blockGameObject.AddComponent<BoxCollider>();

        return blockGameObject;
    }

    private void ReduceBlockCounter(Player player)
    {
        if (character.GetSide() == player)
        {
            ReduceBlockCount();

            if (!IsBlocking())
            {
                GameplayEvents.OnPlayerTurnEnded -= ReduceBlockCounter;
            }
        }
    }

    ~BlockAA()
    {
        GameplayEvents.OnPlayerTurnEnded -= ReduceBlockCounter;
    }
}