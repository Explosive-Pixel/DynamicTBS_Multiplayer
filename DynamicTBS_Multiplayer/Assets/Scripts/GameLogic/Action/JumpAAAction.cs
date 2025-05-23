using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject jumpPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> jumpTargets = new();
    public List<GameObject> ActionDestinations { get { return jumpTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        List<Vector3> patternPositions = FindMovePositions(character, true);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, jumpPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null)
        {
            return movePositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null && movePositions.Count > 0)
        {
            jumpTargets = ActionUtils.InstantiateActionPositions(this, movePositions, jumpPrefab);
            characterInAction = character;
        }
    }

    public bool ExecuteAction(GameObject actionDestination)
    {
        Vector3 initialPosition = characterInAction.gameObject.transform.position;

        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            MoveAction.MoveCharacter(characterInAction, tile);
        }

        GameplayEvents.ActionFinished(new ActionMetadata
        {
            ExecutingPlayer = characterInAction.Side,
            ExecutedActionType = ActionType,
            CharacterInAction = characterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestination.transform.position
        });

        AbortAction();

        return true;
    }

    public void AbortAction()
    {
        ActionUtils.Clear(jumpTargets);
        characterInAction = null;
        ActionRegistry.Remove(this);
    }

    private List<Vector3> FindMovePositions(Character character, bool pattern = false)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> moveTiles = Board.GetTilesInAllDirections(characterTile, JumpAA.movePattern, JumpAA.distance);

        List<Vector3> movePositions = moveTiles
            .FindAll(tile => tile.IsAccessible() || pattern)
            .ConvertAll(tile => tile.gameObject.transform.position);

        return movePositions;
    }
}
