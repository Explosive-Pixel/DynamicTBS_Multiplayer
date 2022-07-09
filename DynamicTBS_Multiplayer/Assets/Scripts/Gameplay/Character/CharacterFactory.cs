using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFactory : MonoBehaviour
{
    private PlayerManager playerManager;

    private void Awake()
    {
        playerManager = GameObject.Find("PlayerManager").GetComponent<PlayerManager>();
    }


    public Character CreateCharacter(CharacterType type, Player side) 
    {
        GameObject gameObject = CreateCharacterGameObject(type, side);

        switch (type) 
        {
            case CharacterType.MasterChar: 
                return new MasterChar(side, gameObject);
            case CharacterType.TankChar:
                return new TankChar(side, gameObject);
            case CharacterType.ShooterChar:
                return new ShooterChar(side, gameObject);
            case CharacterType.RunnerChar:
                return new RunnerChar(side, gameObject);
            case CharacterType.MechanicChar:
                return new MechanicChar(side, gameObject);
            case CharacterType.MedicChar:
                return new MedicChar(side, gameObject);
        }

        return null;
    }

    private GameObject CreateCharacterGameObject(CharacterType type, Player side)
    {
        GameObject character = new GameObject();

        Vector3 startPosition = new Vector3(0, 0, 0);
        Vector3 startScale = new Vector3(100, 0, 100);
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = character.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = ChooseCorrectSprite(side, type);
        character.transform.localScale = startScale;
        character.transform.position = startPosition;
        character.transform.rotation = startRotation;


        Instantiate(character);

        return character;
    }

    private Sprite ChooseCorrectSprite(Player side, CharacterType type)
    {
        if (playerManager.IsBluePlayer(side))
        {
            if (type == CharacterType.MasterChar)
                return SpriteManager.BLUE_MASTER_SPRITE;
            if (type == CharacterType.MechanicChar)
                return SpriteManager.BLUE_MECHANIC_SPRITE;
            if (type == CharacterType.MedicChar)
                return SpriteManager.BLUE_MEDIC_SPRITE;
            if (type == CharacterType.RunnerChar)
                return SpriteManager.BLUE_RUNNER_SPRITE;
            if (type == CharacterType.ShooterChar)
                return SpriteManager.BLUE_SHOOTER_SPRITE;
            if (type == CharacterType.TankChar)
                return SpriteManager.BLUE_TANK_SPRITE;
        }
        else
        {
            if (type == CharacterType.MasterChar)
                return SpriteManager.PINK_MASTER_SPRITE;
            if (type == CharacterType.MechanicChar)
                return SpriteManager.PINK_MECHANIC_SPRITE;
            if (type == CharacterType.MedicChar)
                return SpriteManager.PINK_MEDIC_SPRITE;
            if (type == CharacterType.RunnerChar)
                return SpriteManager.PINK_RUNNER_SPRITE;
            if (type == CharacterType.ShooterChar)
                return SpriteManager.PINK_SHOOTER_SPRITE;
            if (type == CharacterType.TankChar)
                return SpriteManager.PINK_TANK_SPRITE;
        }

        return null;
    }
}