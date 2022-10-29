using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAAHandler : MonoBehaviour
{
    private Board board;

    private Character character;
    private bool waitForMoveTarget;

    private void Awake()
    {
        SubscribeEvents();
        waitForMoveTarget = false;
    }

    public void ExecuteJumpAA(Character character)
    {
        this.character = character;

        if (board == null)
            board = GameObject.Find("GameplayCanvas").GetComponent<Board>();

        Tile characterTile = board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> moveTiles = board.GetTilesOfDistance(characterTile, JumpAA.movePattern, JumpAA.distance);

        List<Vector3> movePositions = moveTiles
            .FindAll(tile => tile.IsAccessible())
            .ConvertAll(tile => tile.GetPosition());

        UIEvents.PassActionPositionsList(movePositions, UIActionType.ActiveAbility_Jump);
        waitForMoveTarget = true;
    }

    private void PerformMove(Vector3 position, UIActionType type)
    {
        if (waitForMoveTarget && type == UIActionType.ActiveAbility_Jump)
        {
            Tile tile = board.GetTileByPosition(position);
            if (tile != null)
            {
                Vector3 oldPosition = character.GetCharacterGameObject().transform.position;
                character.GetCharacterGameObject().transform.position = position;
                UIEvents.MoveOver(oldPosition, character);
            }
            waitForMoveTarget = false;
            GameplayEvents.ActionFinished(UIActionType.ActiveAbility_Jump);
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        UIEvents.OnPassActionDestination += PerformMove;
    }

    private void UnsubscribeEvents()
    {
        UIEvents.OnPassActionDestination -= PerformMove;
    }

    #endregion

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
}
