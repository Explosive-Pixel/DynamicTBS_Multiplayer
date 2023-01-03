using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePA : IPassiveAbility
{
    private static PatternType explodePatternType = PatternType.Star;
    private static int explodeDamage = 1;

    private Character owner;

    public ExplodePA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        CharacterEvents.OnCharacterDeath += Explode;
    }

    private void Explode(Character deadCharacter, Vector3 lastPosition)
    {
        if(deadCharacter == owner)
        {
            Tile ownerLastTile = Board.GetTileByPosition(lastPosition);
            foreach(Character character in CharacterHandler.GetAllLivingCharacters())
            {
                Tile neightborTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
                if(Board.Neighbors(ownerLastTile, neightborTile, explodePatternType))
                {
                    character.TakeDamage(explodeDamage);
                }
            }

            CharacterEvents.OnCharacterDeath -= Explode;
        }
    }

    ~ExplodePA()
    {
        CharacterEvents.OnCharacterDeath -= Explode;
    }
}