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
        if (Executable())
        {
            GameplayEvents.GameIsOver(character.GetSide().GetPlayerType());
        }
    }

    public int CountActionDestinations()
    {
        if (Executable())
        {
            return 1;
        }

        return 0;
    }

    private bool Executable()
    {
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        if (tile != null && tile.GetTileType().Equals(TileType.GoalTile))
        {
            return true;
        }

        return false;
    }
}