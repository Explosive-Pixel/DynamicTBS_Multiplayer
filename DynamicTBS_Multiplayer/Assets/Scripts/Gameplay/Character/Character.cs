using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour
{
    [SerializeField] private string prettyName;
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int moveSpeed;
    [SerializeField] private int attackRange;

    protected CharacterType characterType;
    private PlayerType side;

    private int attackDamage = AttackAction.AttackDamage;
    private PatternType movePattern = MoveAction.MovePattern;

    private bool isClickable = false;
    private int hitPoints;
    private int activeAbilityCooldown;

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

    // States whether the character is disabled (stunned), i.e. can not perform any action (move/attack/perform active ability)
    public delegate bool IsDisabled();
    public IsDisabled isDisabled = () => false;

    public string PrettyName { get { return prettyName; } }
    public CharacterType CharacterType { get { return characterType; } set { characterType = value; } }
    public PlayerType Side { get { return side; } set { side = value; UpdateSide(); } }
    public int MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public int AttackRange { get { return attackRange; } }
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public PatternType MovePattern { get { return movePattern; } set { movePattern = value; } }
    public IActiveAbility ActiveAbility;
    public IPassiveAbility PassiveAbility;
    public int MaxHitPoints { get { return maxHitPoints; } }
    public int HitPoints { get { return hitPoints; } set { hitPoints = Mathf.Max(value, 0); UpdateHitPoints(); } }
    public int ActiveAbilityCooldown { get { return activeAbilityCooldown; } set { activeAbilityCooldown = value; UpdateCooldown(); } }
    public bool IsClickable { get { return isClickable; } set { isClickable = value; } }

    public void Init(CharacterType type, PlayerType side)
    {
        CharacterType = type;
        Side = side;

        ActiveAbility = gameObject.GetComponent<IActiveAbility>();
        PassiveAbility = gameObject.GetComponent<IPassiveAbility>();

        HitPoints = maxHitPoints;
        ActiveAbilityCooldown = 0;

        SubscribeEvents();
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = netDamage(damage);
        if (isDamageable(damage) && actualDamage > 0)
        {
            HitPoints -= actualDamage;

            CharacterEvents.CharacterTakesDamage(this, actualDamage);

            if (IsDead())
            {
                Die();
            }
        }
        CharacterEvents.CharacterReceivesDamage(this, damage);
    }

    public void Heal(int healPoints)
    {
        hitPoints += healPoints;
        HitPoints = Mathf.Min(maxHitPoints, hitPoints);
    }

    public bool HasFullHP()
    {
        return HitPoints == maxHitPoints;
    }

    public bool IsDead()
    {
        return HitPoints <= 0;
    }

    private void ResetStates()
    {
        gameObject.GetComponents<IState>().ToList().ForEach(state => state.Destroy());
    }

    public virtual void Die()
    {
        ResetStates();
        CharacterEvents.CharacterDies(this, gameObject.transform.position);
        Destroy(gameObject);
    }

    public void ExecuteActiveAbility()
    {
        if (CanPerformActiveAbility())
        {
            ActiveAbility.Execute();
            GameplayEvents.StartExecuteActiveAbility(this);
        }
    }

    public bool IsActiveAbilityOnCooldown()
    {
        return ActiveAbilityCooldown > 0;
    }

    public bool MayPerformActiveAbility()
    {
        return !isDisabled() && !IsActiveAbilityOnCooldown();
    }

    public bool CanPerformActiveAbility()
    {
        return MayPerformActiveAbility() && ActiveAbility.CountActionDestinations() > 0;
    }

    public bool CanPerformAction()
    {
        return !isDisabled() && (ActionUtils.CountAllActionDestinations(this) > 0 || CanPerformActiveAbility());
    }

    public void SetActiveAbilityOnCooldown()
    {
        ActiveAbilityCooldown = ActiveAbility.Cooldown + 1;
        GameplayEvents.OnPlayerTurnEnded += ReduceActiveAbiliyCooldown;
    }

    public void ReduceActiveAbilityCooldown()
    {
        if (ActiveAbilityCooldown > 0)
        {
            ActiveAbilityCooldown -= 1;
            if (ActiveAbilityCooldown == 0)
            {
                GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
            }
        }
    }

    private void SetActiveAbilityOnCooldown(ActionMetadata actionMetadata)
    {
        if (actionMetadata.ExecutedActionType == ActionType.ActiveAbility && actionMetadata.CharacterInAction == this)
        {
            SetActiveAbilityOnCooldown();
        }
    }

    private void ReduceActiveAbiliyCooldown(PlayerType player)
    {
        if (Side.Equals(player))
        {
            ReduceActiveAbilityCooldown();
        }
    }

    private void PrepareCharacter(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.PLACEMENT && gamePhase != GamePhase.GAMEPLAY)
            return;

        isClickable = true;

        if (gamePhase == GamePhase.GAMEPLAY)
        {
            PassiveAbility.Apply();
        }
    }

    private void UpdateSide()
    {
        gameObject.GetComponentsInChildren<SideHandler>(true).ToList().ForEach(sideHandler => sideHandler.SetSide(side));
    }

    private void UpdateHitPoints()
    {
        gameObject.GetComponentInChildren<HealthBarHandler>().UpdateHP(hitPoints);
    }

    private void UpdateCooldown()
    {
        gameObject.GetComponentInChildren<CooldownBarHandler>().UpdateCooldown(activeAbilityCooldown);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += PrepareCharacter;
        GameplayEvents.OnFinishAction += SetActiveAbilityOnCooldown;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= PrepareCharacter;
        GameplayEvents.OnFinishAction -= SetActiveAbilityOnCooldown;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
    }

    #endregion

    private void OnDestroy()
    {
        if (UIClickHandler.CurrentCharacter == this)
            GameplayEvents.ChangeCharacterSelection(null);

        UnsubscribeEvents();
    }
}
