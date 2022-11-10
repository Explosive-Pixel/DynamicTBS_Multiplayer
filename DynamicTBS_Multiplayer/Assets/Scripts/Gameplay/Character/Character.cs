using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    #region Character default stats Config

    protected const PatternType defaultMovePattern = PatternType.Cross;
    protected const int defaultAttackDamage = 1;

    #endregion

    protected CharacterType characterType;
    protected int moveSpeed;
    protected int maxHitPoints;
    protected int attackRange;
    protected IActiveAbility activeAbility;
    protected IPassiveAbility passiveAbility;
    protected abstract Sprite CharacterSprite(Player side);

    protected GameObject characterGameObject;

    #region public properties (can be overwritten by its Active- and Passive abiliy)

    public Player side;
    public PatternType movePattern;
    public int attackDamage;

    // States whether the character can be targeted by another character to get attacked/healed
    public delegate bool IsTargetable(Character attacker);
    public IsTargetable isAttackableBy = (attacker) => true;
    public IsTargetable isHealableBy = (healer) => true;

    // States whether the character can take damage
    public delegate bool IsDamageable(int damage);
    public IsDamageable isDamageable = (damage) => true;

    // States whether the character is disabled, i.e. can not perform any action (move/attack/perform active ability)
    public delegate bool IsDisabled();
    public IsDisabled isDisabled = () => false;

    public int hitPoints;
    public int activeAbilityCooldown;

    #endregion

    protected Character(Player side)
    {
        this.side = side;
    }

    protected void Init()
    {
        this.characterGameObject = CreateCharacterGameObject();
        this.hitPoints = maxHitPoints;
        this.activeAbilityCooldown = 0;
        this.movePattern = defaultMovePattern;
        this.attackDamage = defaultAttackDamage;

        GameplayEvents.OnGameplayPhaseStart += ApplyPassiveAbility;
    }

    public GameObject GetCharacterGameObject() { return characterGameObject; }
    public CharacterType GetCharacterType() { return characterType; }
    public int GetMoveSpeed() { return moveSpeed; }
    public int GetAttackRange() { return attackRange; }
    public IPassiveAbility GetPassiveAbility() { return passiveAbility; }
    public Sprite GetCharacterSprite(Player side) { return CharacterSprite(side); }

    public void TakeDamage(int damage) 
    {
        CharacterEvents.CharacterTakesDamage(this, damage);
        if(isDamageable(damage))
        {
            this.hitPoints -= damage;
            Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
            if (this.hitPoints <= 0)
            {
                this.Die();
            }
        }
    }

    public void Heal(int healPoints) 
    {
        this.hitPoints += healPoints;
        if (this.hitPoints > this.maxHitPoints)
        {
            this.hitPoints = this.maxHitPoints;
        }
        Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
    }

    public bool HasFullHP()
    {
        return this.hitPoints == this.maxHitPoints;
    }

    public void SetActiveAbilityOnCooldown()
    {
        activeAbilityCooldown = activeAbility.Cooldown + 1;
        GameplayEvents.OnPlayerTurnEnded += ReduceActiveAbiliyCooldown;
    }

    public void ReduceActiveAbilityCooldown()
    {
        if(activeAbilityCooldown > 0)
        {
            activeAbilityCooldown -= 1;

            if(activeAbilityCooldown == 0)
            {
                GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
            }
        }
    }

    public bool IsActiveAbilityOnCooldown()
    {
        return activeAbilityCooldown > 0;
    }

    public virtual void Die() 
    {
        CharacterEvents.CharacterDies(this, characterGameObject.transform.position);
        GameObject.Destroy(characterGameObject);
        characterGameObject = null;
    }

    public bool IsDead()
    {
        return characterGameObject == null;
    }

    public Player GetSide()
    {
        return side;
    }

    public IActiveAbility GetActiveAbility()
    {
        return activeAbility;
    }

    private void ReduceActiveAbiliyCooldown(Player player)
    {
        if (side.Equals(player))
        {
            ReduceActiveAbilityCooldown();
        }
    }

    private void ApplyPassiveAbility()
    {
        passiveAbility.Apply();
    }

    private GameObject CreateCharacterGameObject()
    {
        GameObject character = new GameObject
        {
            name = this.GetType().Name + "_" + side.GetPlayerType().ToString()
        };

        Vector3 startPosition = new Vector3(0, 0, 0);
        Quaternion startRotation = Quaternion.identity;

        SpriteRenderer spriteRenderer = character.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = CharacterSprite(side);
        character.transform.position = startPosition;
        character.transform.rotation = startRotation;
        character.AddComponent<BoxCollider>();

        return character;
    }

    ~Character()
    {
        GameplayEvents.OnGameplayPhaseStart -= ApplyPassiveAbility;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
    }
}