using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodePA : IPassiveAbility
{
    private static PatternType explodePatternType = PatternType.Cross;
    private static int explodeDamage = 1;

    private Character owner;
    private CharacterHandler characterHandler;
    private Board board;

    public ExplodePA(Character character)
    {
        owner = character;
    }

    public void Apply() 
    {
        characterHandler = GameObject.Find("GameplayCanvas").GetComponent<CharacterHandler>();
        board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        CharacterEvents.OnCharacterDeath += Explode;
    }

    private void Explode(Character deadCharacter, Vector3 lastPosition)
    {
        if(deadCharacter == owner)
        {
            Tile ownerLastTile = board.GetTileByPosition(lastPosition);
            foreach(Character character in characterHandler.GetAllLivingCharacters())
            {
                Tile neightborTile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
                if(board.Neighbors(ownerLastTile, neightborTile, explodePatternType))
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