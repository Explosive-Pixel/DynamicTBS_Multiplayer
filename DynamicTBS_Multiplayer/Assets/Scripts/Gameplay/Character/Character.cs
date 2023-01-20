using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Character als Prefab anlegen um in Unity anpassen zu können
public abstract class Character //: MonoBehaviour
{
    #region Character default stats Config

    protected const PatternType defaultMovePattern = PatternType.Cross;
    protected const int defaultAttackDamage = 1;

    #endregion

    protected CharacterType characterType;
    protected int moveSpeed;
    protected int maxHitPoints;
    protected int attackRange;
    protected GameObject characterPrefab;
    protected IActiveAbility activeAbility;
    protected IPassiveAbility passiveAbility;
    protected abstract Sprite CharacterSprite(Player side);

    protected GameObject characterGameObject;

    #region public properties (can be overwritten by its Active- and Passive abiliy)

    public Player side;
    public PatternType movePattern;
    public int attackDamage;

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

    public int hitPoints;
    public int activeAbilityCooldown;

    #endregion

    protected Character(Player side)
    {
        this.side = side;
    }

    protected void Init()
    {
        //this.characterGameObject = GameObject.Instantiate(characterPrefab) as GameObject;
        this.characterGameObject = characterPrefab;
        this.hitPoints = maxHitPoints;
        this.activeAbilityCooldown = 0;
        this.movePattern = defaultMovePattern;
        this.attackDamage = defaultAttackDamage;


        //load leben + cooldown prefab -> TODO über entity in unity anpassbar machen
        //GameObject lebenObject = Resources.Load<GameObject>("Prefabs/CharacterCooldown");
        //GameObject cooldownObject = Resources.Load<GameObject>("Prefabs/CharacterLeben");

        //TODO: "leben" als String übergeben
        //spawn Leben Prefab als child und setze animation auf aktuelle lebenspunkte
        //Instantiate(lebenObject, this.characterGameObject.transform.position, Quaternion.identity, this.characterGameObject.transform);


        //TODO: "cooldown" als string übergeben
        //spawn cooldown Prefab als child und setze animation auf 0
        //Instantiate(cooldownObject, this.characterGameObject.transform.position, Quaternion.identity, this.characterGameObject.transform);


        //TODO: in script in Prefab setzen
        for (int i = 0; i < this.characterGameObject.transform.childCount; i++)
        {
            this.characterGameObject.transform.GetChild(i).GetComponent<Animator>().SetInteger("leben", this.hitPoints);
            this.characterGameObject.transform.GetChild(i).GetComponent<Animator>().SetInteger("cooldown", 0);
        }

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
        if (!IsDead())
        {
            int actualDamage = netDamage(damage);
            if (isDamageable(damage) && actualDamage > 0)
            {
                this.hitPoints -= actualDamage;
                this.characterGameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("leben", this.hitPoints);
                Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");

                CharacterEvents.CharacterTakesDamage(this, actualDamage);

                if (this.hitPoints <= 0)
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
            this.hitPoints += healPoints;
            if (this.hitPoints > this.maxHitPoints)
            {
                this.hitPoints = this.maxHitPoints;
            }
            this.characterGameObject.transform.GetChild(0).GetComponent<Animator>().SetInteger("leben", this.hitPoints);
            Debug.Log("Character " + characterGameObject.name + " now has " + hitPoints + " hit points remaining.");
        }
    }

    public bool HasFullHP()
    {
        return this.hitPoints == this.maxHitPoints;
    }

    public void SetActiveAbilityOnCooldown()
    {
        if(!IsDead())
        {
            activeAbilityCooldown = activeAbility.Cooldown + 1;
            this.characterGameObject.transform.GetChild(1).GetComponent<Animator>().SetInteger("cooldown", activeAbility.Cooldown);
            GameplayEvents.OnPlayerTurnEnded += ReduceActiveAbiliyCooldown;
        }
    }

    public void ReduceActiveAbilityCooldown()
    {
        if(activeAbilityCooldown > 0 && !IsDead())
        {
            activeAbilityCooldown -= 1;
            this.characterGameObject.transform.GetChild(1).GetComponent<Animator>().SetInteger("cooldown", activeAbilityCooldown);
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

    ~Character()
    {
        GameplayEvents.OnGameplayPhaseStart -= ApplyPassiveAbility;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
    }
}