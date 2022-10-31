using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PullDamagePA : IPassiveAbility
{
    // TODO: init
    List<Character> characters;
    Board board;
    Character owner;

    // TODO: do not use IsAttackableBy => define new delegate canTakeDamage(damage) and call this as condition in TakeDamage()
    public void Apply() 
    {
        foreach (Character character in characters)
        {
            var defaultIsAttackableBy = character.isAttackableBy;
            character.isAttackableBy = (attacker) =>
            {
                if (character.GetPassiveAbility().GetType() == typeof(PullDamagePA))
                {
                    return defaultIsAttackableBy(attacker);
                }

                if(character.GetSide() == owner.GetSide() && Neighbors(character, owner) && owner.isAttackableBy(attacker))
                {
                    return false;
                }

                return defaultIsAttackableBy(attacker);
            };
        }
    }

    // TODO: Listen to event TakeDamage and check if character is neighbor of owner -> if yes: owner takes damage

    private bool Neighbors(Character c1, Character c2) 
    {
        Tile c1Tile = board.GetTileByCharacter(c1);
        Tile c2Tile = board.GetTileByCharacter(c2);

        return (c1Tile.GetRow() == c2Tile.GetRow() && Math.Abs(c1Tile.GetColumn() - c2Tile.GetColumn()) == 1) || (c1Tile.GetColumn() == c2Tile.GetColumn() && Math.Abs(c1Tile.GetRow() - c2Tile.GetRow()) == 1);
    }
}