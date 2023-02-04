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
            GameplayEvents.ActionFinished(new ActionMetadata
            {
                ExecutingPlayer = character.GetSide(),
                ExecutedActionType = ActionType.ActiveAbility,
                CharacterInAction = character,
                CharacterInitialPosition = character.GetCharacterGameObject().transform.position,
                ActionDestinationPosition = null
            });
            GameplayEvents.GameIsOver(character.GetSide().GetPlayerType(), GameOverCondition.MASTER_TOOK_CONTROL);
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

    public void ShowActionPattern()
    {

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