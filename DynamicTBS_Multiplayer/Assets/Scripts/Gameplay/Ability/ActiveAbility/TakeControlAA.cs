using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlAA : IActiveAbility
{
    Character character;

    public TakeControlAA(Character character)
    {
        this.character = character;
    }

    public void Execute() 
    {
        Board board = GameObject.Find("GameplayCanvas").GetComponent<Board>();
        Tile tile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        if (tile.GetType().Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(character.GetSide().GetPlayerType());
        }
    }
}