using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{   
    protected int moveSpeed;
    protected int maxHitPoints;
    protected int attackRange;
    protected IActiveAbility activeAbility;
    protected IPassiveAbility passiveAbility;

    protected int hitPoints;
    protected Player side;
    protected GameObject characterGameObject;
    protected Sprite characterSprite;

    protected bool hasGameStarted;

    protected Character(Player side)
    {
        this.side = side;
    }

    protected void Init()
    {
        this.characterGameObject = CreateCharacterGameObject();
        this.hitPoints = maxHitPoints;
    }

    public GameObject GetCharacterGameObject() { return characterGameObject; }
    public int GetMoveSpeed() { return moveSpeed; }
    public int GetAttackRange() { return attackRange; }

    public void GetAttacked() 
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage) {
        this.hitPoints -= damage;
        Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
        if (this.hitPoints <= 0) {
            this.Die();
        }
    }

    public void Heal(int healPoints) {
        this.hitPoints += healPoints;
        if (this.hitPoints > this.maxHitPoints) {
            this.hitPoints = this.maxHitPoints;
        }
    }

    public virtual void Die() 
    {
        CharacterEvents.CharacterDies(this, characterGameObject.transform.position);
        GameObject.Destroy(characterGameObject);
    }

    public void SwitchSide(Player newSide) {
        this.side = newSide;
    }

    public Player GetSide()
    {
        return side;
    }

    private GameObject CreateCharacterGameObject()
    {
        GameObject character = new GameObject();
        character.name = this.GetType().Name + "_" + side.GetPlayerType().ToString();

        Vector3 startPosition = new Vector3(0, 0, 0);
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = character.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = this.characterSprite;
        character.transform.position = startPosition;
        character.transform.rotation = startRotation;
        character.AddComponent<BoxCollider>();

        return character;
    }
}