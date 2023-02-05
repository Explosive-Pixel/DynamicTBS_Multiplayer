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
    protected int maxHitPoints;
    protected int attackRange;
    protected IActiveAbility activeAbility;
    protected IPassiveAbility passiveAbility;
    protected abstract GameObject CharacterPrefab(Player side);

    protected int hitPoints;
    protected int activeAbilityCooldown;
    protected Animator hitPointAnimator;
    protected Animator cooldownAnimator;

    protected GameObject characterGameObject;

    #region public properties (can be overwritten by its Active- and Passive abiliy)

    public Player side;
    public PatternType movePattern;
    public int attackDamage;
    public int moveSpeed;

    public bool isClickable = true;

    // States whether the character can be targeted by another character to get attacked/healed
    public delegate bool IsTargetable(Character attacker);
    public IsTargetable isAttackableBy = (attacker) => true;
    public IsTargetable isHealableBy = (healer) => true;

    // States whether the character can take damage
    public delegate bool IsDamageable(int damage);
    public IsDamageable isDamageable = (damage) => true;
    // States how much damage a character actually receives when attacked
    public delegate int NetDamage(int damage);
    public NetDamage netDamage = (damage) => damage;

    // States whether the character is disabled, i.e. can not perform any action (move/attack/perform active ability)
    public delegate bool IsDisabled();
    public IsDisabled isDisabled = () => false;

    public int HitPoints { get { return hitPoints; } set { this.hitPoints = value; UpdateHitPointAnimator(); } }
    public int ActiveAbilityCooldown { get { return activeAbilityCooldown; } set { this.activeAbilityCooldown = value; UpdateCooldownAnimator(); } }

    #endregion

    protected Character(Player side)
    {
        this.side = side;
    }

    protected void Init()
    {
        InitCharacterGameObject();

        this.HitPoints = maxHitPoints;
        this.ActiveAbilityCooldown = 0;
        this.movePattern = defaultMovePattern;
        this.attackDamage = defaultAttackDamage;

        GameplayEvents.OnGameplayPhaseStart += ApplyPassiveAbility;
    }

    public GameObject GetCharacterGameObject() { return characterGameObject; }
    public CharacterType GetCharacterType() { return characterType; }
    public int GetMoveSpeed() { return moveSpeed; }
    public int GetAttackRange() { return attackRange; }
    public IPassiveAbility GetPassiveAbility() { return passiveAbility; }
    public GameObject GetCharacterPrefab(Player side) { return CharacterPrefab(side); }
    public Sprite GetCharacterSprite(Player side) 
    {
        GameObject prefab = CharacterPrefab(side);
        GameObject characterSprite = UIUtils.FindChildGameObject(prefab, "CharacterSprite");
        return characterSprite.GetComponent<SpriteRenderer>().sprite;
    }

    public void TakeDamage(int damage) 
    {
        if (!IsDead())
        {
            int actualDamage = netDamage(damage);
            if (isDamageable(damage) && actualDamage > 0)
            {
                this.HitPoints -= actualDamage;

                CharacterEvents.CharacterTakesDamage(this, actualDamage);

                if (this.HitPoints <= 0)
                {
                    this.Die();
                }
            }
            CharacterEvents.CharacterReceivesDamage(this, damage);
        }
    }

    public void Heal(int healPoints) 
    {
        if (!IsDead())
        {
            this.HitPoints += healPoints;
            if (this.HitPoints > this.maxHitPoints)
            {
                this.HitPoints = this.maxHitPoints;
            }
        }
    }

    public bool HasFullHP()
    {
        return this.HitPoints == this.maxHitPoints;
    }

    public void SetActiveAbilityOnCooldown()
    {
        if(!IsDead())
        {
            ActiveAbilityCooldown = activeAbility.Cooldown + 1;
            UIUtils.UpdateAnimator(cooldownAnimator, this.ActiveAbilityCooldown - 1);
            GameplayEvents.OnPlayerTurnEnded += ReduceActiveAbiliyCooldown;
        }
    }

    public void ReduceActiveAbilityCooldown()
    {
        if(ActiveAbilityCooldown > 0 && !IsDead())
        {
            ActiveAbilityCooldown -= 1;
            if(ActiveAbilityCooldown == 0)
            {
                GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
            }
        }
    }

    public void Highlight(bool highlight)
    {
        UIUtils.FindChildGameObject(characterGameObject, "ActiveCharacter Highlight").SetActive(highlight);
    }

    public bool IsActiveAbilityOnCooldown()
    {
        return ActiveAbilityCooldown > 0;
    }

    public bool CanPerformActiveAbility()
    {
        return !IsActiveAbilityOnCooldown() && activeAbility.CountActionDestinations() > 0; 
    }

    public bool CanPerformAction()
    {
        return ActionUtils.CountAllActionDestinations(this) > 0 || CanPerformActiveAbility();
    }

    public virtual void Die() 
    {
        CharacterEvents.CharacterDies(this, characterGameObject.transform.position);
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
        GameObject.Destroy(characterGameObject);
        characterGameObject = null;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
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

    private void UpdateHitPointAnimator()
    {
        UIUtils.UpdateAnimator(hitPointAnimator, this.HitPoints);
    }

    private void UpdateCooldownAnimator()
    {
        UIUtils.UpdateAnimator(cooldownAnimator, this.ActiveAbilityCooldown);
    }

    private void InitCharacterGameObject()
    {
        this.characterGameObject = GameObject.Instantiate(this.CharacterPrefab(this.side));
        this.characterGameObject.transform.SetParent(GameObject.Find("GameplayObjects").transform);

        hitPointAnimator = UIUtils.FindChildGameObject(this.characterGameObject, "Leben").GetComponent<Animator>();
        cooldownAnimator = UIUtils.FindChildGameObject(this.characterGameObject, "Cooldown").GetComponent<Animator>();
    }

    ~Character()
    {
        GameplayEvents.OnGameplayPhaseStart -= ApplyPassiveAbility;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
    }
}