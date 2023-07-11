using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeControlAA : MonoBehaviour, IActiveAbility
{
    [SerializeField] private int aaCooldown; // 0

    public int Cooldown { get { return aaCooldown; } }

    CharacterMB character;

    private void Awake()
    {
        this.character = gameObject.GetComponent<CharacterMB>();
    }

    public void Execute()
    {
        if (Executable())
        {
            GameplayEvents.ActionFinished(new ActionMetadata
            {
                ExecutingPlayer = character.Side,
                ExecutedActionType = ActionType.ActiveAbility,
                CharacterInAction = character,
                CharacterInitialPosition = character.gameObject.transform.position,
                ActionDestinationPosition = null
            });
            GameplayEvents.GameIsOver(character.Side, GameOverCondition.MASTER_TOOK_CONTROL);
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
        TileMB tile = BoardNew.GetTileByCharacter(character);
        if (tile != null && tile.TileType.Equals(TileType.GoalTile))
        {
            return true;
        }

        return false;
    }
}