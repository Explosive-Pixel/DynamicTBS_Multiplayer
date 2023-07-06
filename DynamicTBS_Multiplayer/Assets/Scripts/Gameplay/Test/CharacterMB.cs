using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMB : MonoBehaviour
{
    [SerializeField]
    private int defaultAttackDamage;

    public string prettyName;
    public CharacterType characterType;
    public int maxHitPoints;
    public int moveSpeed;
    public int attackRange;

    public ActiveAbility activeAbilityType;

    public PlayerType side;

    private IActiveAbility activeAbility;

    private void Awake()
    {
        // ActiveAbilityFactory.Create(activeAbilityType, this);
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
