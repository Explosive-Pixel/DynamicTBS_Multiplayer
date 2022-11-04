using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlAA : IActiveAbility
{
    public int Cooldown { get { return 0; } }

    Character character;

    public TakeControlAA(Character character)
    {
        this.character = character;
    }

    public void Execute() 
    {
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        if (tile.GetType().Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(character.GetSide().GetPlayerType());
        }
    }
}