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

    protected Character(Player side) {
        this.side = side;
        this.hitPoints = maxHitPoints;
    }

    // public void Move() { } Don't need that here since character doesn't know its current position
    // We need a method MoveCharacter(Character character, Vector3 newPosition) in Board

    public void TakeDamage(int damage) {
        this.hitPoints -= damage;
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

    public void Die() {  }

    public void SwitchSide(Player newSide) {
        this.side = newSide;
    }
}