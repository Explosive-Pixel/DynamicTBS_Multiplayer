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

    public BlockAA(Character character)
    {
        this.character = character;
        blockAAHandler = GameObject.Find("ActiveAbilityObject").GetComponent<BlockAAHandler>();
    }
    public void Execute() 
    {
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
        character.SetDisabled(true);
        character.SetCanReceiveDamage(false);
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
                character.SetDisabled(false);
                character.SetCanReceiveDamage(true);
            }
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