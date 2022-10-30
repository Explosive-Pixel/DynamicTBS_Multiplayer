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
    protected int activeAbilityCooldown;
    protected bool disabled;
    protected bool canReceiveDamage;
    protected bool canBeHealed;
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
        this.activeAbilityCooldown = 0;
        this.disabled = false;
        this.canReceiveDamage = true;
        this.canBeHealed = true;
    }

    public GameObject GetCharacterGameObject() { return characterGameObject; }
    public int GetMoveSpeed() { return moveSpeed; }
    public int GetAttackRange() { return attackRange; }

    public void GetAttacked()
    {
        TakeDamage(1);
    }

    public void TakeDamage(int damage) {
        if (canReceiveDamage)
        {
            this.hitPoints -= damage;
            Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
            if (this.hitPoints <= 0)
            {
                this.Die();
            }
        }
    }

    public void Heal(int healPoints) {
        if (canBeHealed)
        {
            this.hitPoints += healPoints;
            if (this.hitPoints > this.maxHitPoints)
            {
                this.hitPoints = this.maxHitPoints;
            }
            Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
        }
    }

    public bool HasFullHP()
    {
        return this.hitPoints == this.maxHitPoints;
    }

    public int GetActiveAbilityCooldown()
    {
        return activeAbilityCooldown;
    }

    public void SetActiveAbilityOnCooldown()
    {
        activeAbilityCooldown = activeAbility.Cooldown + 1;
    }

    public void ReduceActiveAbilityCooldown()
    {
        if(activeAbilityCooldown > 0)
        {
            activeAbilityCooldown -= 1;
        }
    }

    public bool IsActiveAbilityOnCooldown()
    {
        return activeAbilityCooldown > 0;
    }

    public bool IsDisabled()
    {
        return disabled;
    }

    public void SetDisabled(bool disabled)
    {
        this.disabled = disabled;
    }

    public bool CanReceiveDamage()
    {
        return canReceiveDamage;
    }

    public void SetCanReceiveDamage(bool canReceiveDamage)
    {
        this.canReceiveDamage = canReceiveDamage;
    }

    public bool CanBeHealed()
    {
        return canBeHealed;
    }

    public void SetCanBeHealed(bool canBeHealed)
    {
        this.canBeHealed = canBeHealed;
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

    public IActiveAbility GetActiveAbility()
    {
        return activeAbility;
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