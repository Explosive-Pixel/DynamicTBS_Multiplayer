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

    public CharacterType CharacterType { get { return characterType; } }
    public PlayerType Side { get { return side; } }
    public int HitPoints { get { return hitPoints; } set { this.hitPoints = value; UpdateHitPointAnimator(); } }
    public int ActiveAbilityCooldown { get { return activeAbilityCooldown; } set { this.activeAbilityCooldown = value; UpdateCooldownAnimator(); } }
    public bool IsClickable { get { return isClickable; } set { isClickable = value; } }

    private void Awake()
    {
        SubscribeEvents();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
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
            isClickable = true;
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
