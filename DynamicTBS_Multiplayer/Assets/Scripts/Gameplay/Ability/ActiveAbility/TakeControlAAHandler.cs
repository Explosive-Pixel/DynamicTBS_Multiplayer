using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlAAHandler : MonoBehaviour
{
    private Board board;
    public void ExecuteTakeControlAA(Character character)
    {
        if (board == null)
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        Tile tile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        if (tile.GetType().Equals(TileType.GoalTile))
        {
            GameplayEvents.GameIsOver(character.GetSide().GetPlayerType());
        }
    }
}
