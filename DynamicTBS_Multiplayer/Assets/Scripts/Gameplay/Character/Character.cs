using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Character : MonoBehaviour
{
    [SerializeField] private string prettyName;
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int moveSpeed;
    [SerializeField] private int attackRange;
    [SerializeField] private PlayerType side;

    [SerializeField] private Animator hitPointAnimator;
    [SerializeField] private Animator cooldownAnimator;
    [SerializeField] private GameObject activeHighlight;

    protected CharacterType characterType;

    private int attackDamage = AttackAction.AttackDamage;
    private PatternType movePattern = MoveAction.MovePattern;

    private bool isClickable = false;
    private int hitPoints;
    private int activeAbilityCooldown;
    private State state;

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
    public CharacterType CharacterType { get { return characterType; } }
    public PlayerType Side { get { return side; } set { side = value; } }
    public int MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public int AttackRange { get { return attackRange; } }
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public PatternType MovePattern { get { return movePattern; } set { movePattern = value; } }
    public IActiveAbility ActiveAbility;
    public IPassiveAbility PassiveAbility;
    public int HitPoints { get { return hitPoints; } set { hitPoints = value; UpdateHitPointAnimator(); } }
    public int ActiveAbilityCooldown { get { return activeAbilityCooldown; } set { activeAbilityCooldown = value; UpdateCooldownAnimator(); } }
    public bool IsClickable { get { return isClickable; } set { isClickable = value; } }

    private void Awake()
    {
        HitPoints = maxHitPoints;
        ActiveAbilityCooldown = 0;

        ActiveAbility = gameObject.GetComponent<IActiveAbility>();
        PassiveAbility = gameObject.GetComponent<IPassiveAbility>();

        SubscribeEvents();
    }

    public void TakeDamage(int damage)
    {
        int actualDamage = netDamage(damage);
        if (isDamageable(damage) && actualDamage > 0)
        {
            HitPoints -= actualDamage;

            CharacterEvents.CharacterTakesDamage(this, actualDamage);

            if (HitPoints <= 0)
            {
                Die();
            }
        }
        CharacterEvents.CharacterReceivesDamage(this, damage);
    }

    public void Heal(int healPoints)
    {
        HitPoints += healPoints;
        if (HitPoints > maxHitPoints)
        {
            HitPoints = maxHitPoints;
        }
    }

    public bool HasFullHP()
    {
        return HitPoints == maxHitPoints;
    }

    public void SetState(CharacterStateType stateType)
    {
        state = CharacterStateFactory.Create(stateType, gameObject);
    }

    private void ResetState()
    {
        state?.Destroy();
        state = null;
    }

    public bool IsStunned()
    {
        return state != null && state.GetType() == typeof(StunnedState) && state.IsActive();
    }

    public virtual void Die()
    {
        ResetState();
        CharacterEvents.CharacterDies(this, gameObject.transform.position);
        Destroy(gameObject);
    }

    public bool IsActiveAbilityOnCooldown()
    {
        return ActiveAbilityCooldown > 0;
    }

    public bool CanPerformActiveAbility()
    {
        return !IsActiveAbilityOnCooldown() && ActiveAbility.CountActionDestinations() > 0;
    }

    public bool CanPerformAction()
    {
        return !IsStunned() && (ActionUtils.CountAllActionDestinations(this) > 0 || CanPerformActiveAbility());
    }

    public void SetActiveAbilityOnCooldown()
    {
        ActiveAbilityCooldown = ActiveAbility.Cooldown + 1;
        UIUtils.UpdateAnimator(cooldownAnimator, ActiveAbilityCooldown - 1);
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

    private void UpdateHitPointAnimator()
    {
        UIUtils.UpdateAnimator(hitPointAnimator, HitPoints);
    }

    private void UpdateCooldownAnimator()
    {
        UIUtils.UpdateAnimator(cooldownAnimator, ActiveAbilityCooldown);
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

    private void HighlightCharacter(Character character)
    {
        Highlight(character == this);
    }

    private void Highlight(bool highlight)
    {
        activeHighlight.SetActive(highlight);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += PrepareCharacter;
        GameplayEvents.OnFinishAction += SetActiveAbilityOnCooldown;
        GameplayEvents.OnCharacterSelectionChange += HighlightCharacter;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= PrepareCharacter;
        GameplayEvents.OnFinishAction -= SetActiveAbilityOnCooldown;
        GameplayEvents.OnCharacterSelectionChange -= HighlightCharacter;
        GameplayEvents.OnPlayerTurnEnded -= ReduceActiveAbiliyCooldown;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
