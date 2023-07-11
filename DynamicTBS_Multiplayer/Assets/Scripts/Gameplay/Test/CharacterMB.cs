using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CharacterMB : MonoBehaviour
{
    [SerializeField] private string prettyName;
    [SerializeField] private CharacterType characterType;
    [SerializeField] private int maxHitPoints;
    [SerializeField] private int moveSpeed;
    [SerializeField] private int attackRange;
    [SerializeField] private PlayerType side;

    [SerializeField] private Animator hitPointAnimator;
    [SerializeField] private Animator cooldownAnimator;
    [SerializeField] private GameObject activeHighlight;

    private int attackDamage = AttackAction.AttackDamage;
    private PatternType movePattern = MoveAction.MovePattern;

    private bool isClickable = false;
    private int hitPoints;
    private int activeAbilityCooldown;
    private State state;

    // States whether the character can be targeted by another character to get attacked/healed
    public delegate bool IsTargetable(CharacterMB attacker);
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

    public CharacterType CharacterType { get { return characterType; } }
    public PlayerType Side { get { return side; } set { side = value; } }
    public int MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public int AttackRange { get { return attackRange; } }
    public int AttackDamage { get { return attackDamage; } set { attackDamage = value; } }
    public PatternType MovePattern { get { return movePattern; } set { movePattern = value; } }
    public IActiveAbility ActiveAbility { get { return gameObject.GetComponent<IActiveAbility>(); } }
    public IPassiveAbility PassiveAbility { get { return gameObject.GetComponent<IPassiveAbility>(); } }
    public int HitPoints { get { return hitPoints; } set { this.hitPoints = value; UpdateHitPointAnimator(); } }
    public int ActiveAbilityCooldown { get { return activeAbilityCooldown; } set { this.activeAbilityCooldown = value; UpdateCooldownAnimator(); } }
    public bool IsClickable { get { return isClickable; } set { isClickable = value; } }

    private void Awake()
    {
        SubscribeEvents();
    }

    public void TakeDamage(int damage)
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

    public void Heal(int healPoints)
    {
        this.HitPoints += healPoints;
        if (this.HitPoints > this.maxHitPoints)
        {
            this.HitPoints = this.maxHitPoints;
        }
    }

    public bool HasFullHP()
    {
        return this.HitPoints == this.maxHitPoints;
    }

    public void SetState(CharacterStateType stateType)
    {
        this.state = CharacterStateFactory.Create(stateType, gameObject);
    }

    private void ResetState()
    {
        if (state != null)
            state.Destroy();

        state = null;
    }

    public bool IsStunned()
    {
        return state != null && state.GetType() == typeof(StunnedState) && state.IsActive();
    }

    public virtual void Die()
    {
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

    private void SetActiveAbilityOnCooldown(ActionMetadata actionMetadata)
    {
        //if (actionMetadata.ExecutedActionType == ActionType.ActiveAbility && actionMetadata.CharacterInAction == this)
        //{
        //    SetActiveAbilityOnCooldown();
        //}
    }

    private void Highlight(bool highlight)
    {
        activeHighlight.SetActive(highlight);
    }

    private void UpdateHitPointAnimator()
    {
        UIUtils.UpdateAnimator(hitPointAnimator, this.HitPoints);
    }

    private void UpdateCooldownAnimator()
    {
        UIUtils.UpdateAnimator(cooldownAnimator, this.ActiveAbilityCooldown);
    }

    private void PrepareCharacter(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.GAMEPLAY)
        {
            isClickable = true;
            PassiveAbility.Apply();
        }
    }

    private void HighlightCharacter(CharacterMB character)
    {
        Highlight(character == this);
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += PrepareCharacter;
        GameplayEvents.OnFinishAction += SetActiveAbilityOnCooldown;
        //GameplayEvents.OnCharacterSelectionChange += HighlightCharacter;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= PrepareCharacter;
        GameplayEvents.OnFinishAction -= SetActiveAbilityOnCooldown;
        //GameplayEvents.OnCharacterSelectionChange -= HighlightCharacter;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
