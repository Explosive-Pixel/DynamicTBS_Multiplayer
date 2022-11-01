using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAA : IActiveAbility
{
    public static int blockingRounds = 2;

    public int Cooldown { get { return 3; } }

    private BlockAAHandler blockAAHandler;

    private Character character;
    private int currentBlockCount = 0;
    private GameObject blockGameObject = null;

    private CharacterHandler characterHandler;
    private Board board;
    private bool firstExecution = true;

    public BlockAA(Character character)
    {
        this.character = character;
        blockAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<BlockAAHandler>();

        var defaultIsAttackableBy = character.isAttackableBy;
        character.isAttackableBy = (attacker) => {
            if (currentBlockCount > 0) return false;
            return defaultIsAttackableBy(character);
        };

        var defaultIsDisabled = character.isDisabled;
        character.isDisabled = () =>
        {
            if (IsBlocking()) return true;
            return defaultIsDisabled();
        };

        /* var defaultIsDamageable = character.isDamageable;
         character.isDamageable = (damage) => {
             if (currentBlockCount > 0) return false;
             return defaultIsDamageable(damage);
         }; */
    }
    public void Execute() 
    {
        if (firstExecution)
        {
            characterHandler = GameObject.Find("GameplayCanvas").GetComponent<CharacterHandler>();
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();
            ChangeIsAttackableByOfOtherCharacters();

            firstExecution = false;
        }
        blockAAHandler.ExecuteBlockAA(this);
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
        foreach(Character c in characterHandler.GetAllLivingCharacters())
        {
            var cDefaultIsAttackableBy = c.isAttackableBy;
            c.isAttackableBy = (attacker) =>
                {
                    if (currentBlockCount > 0 && attacker != character)
                    {
                        Tile attackerTile = board.GetTileByCharacter(attacker);
                        Tile cTile = board.GetTileByCharacter(c);
                        Tile characterTile = board.GetTileByCharacter(character);
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
}