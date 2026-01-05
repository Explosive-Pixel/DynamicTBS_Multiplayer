using System.Collections.Generic;
using UnityEngine;

public class RepairAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject patternPrefab;
    [SerializeField] private GameObject selectFloorPrefab;
    [SerializeField] private GameObject selectHolePrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private Tile selectedFloor = null;
    private Tile selectedHole = null;

    private List<GameObject> actionDestinations = new();
    public List<GameObject> ActionDestinations { get { return actionDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetTilesInAllDirections(characterTile, RepairAA.pattern, RepairAA.range).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, patternPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> floorPositions = FindSelectableFloorPositions(character);
        List<Vector3> holePositions = FindSelectableHolePositions(character);

        return floorPositions.Count * holePositions.Count;
    }

    public void CreateActionDestinations(Character character)
    {
        if (CountActionDestinations(character) == 0)
            return;

        List<Vector3> changeFloorPositions = selectedFloor == null ? FindSelectableFloorPositions(character) : FindSelectableHolePositions(character);

        if (changeFloorPositions != null && changeFloorPositions.Count > 0)
        {
            actionDestinations = ActionUtils.InstantiateActionPositions(this, changeFloorPositions, selectedFloor == null ? selectFloorPrefab : selectHolePrefab);
            characterInAction = character;
        }
    }

    public ActionStep BuildAction(GameObject actionDestination)
    {
        Vector3 initialPosition = characterInAction.gameObject.transform.position;

        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (selectedFloor == null)
            {
                selectedFloor = tile;
                ActionUtils.Clear(actionDestinations);
                CreateActionDestinations(characterInAction);
            }
            else
            {
                selectedHole = tile;
                ActionUtils.Clear(actionDestinations);
            }
        }

        return new ActionStep()
        {
            ActionType = ActionType,
            CharacterInAction = CharacterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestination.transform.position,
            ActionFinished = selectedHole != null
        };
    }

    public void ExecuteAction(Action action)
    {
        if (!action.IsAction(ActionType) || action.ActionSteps.Count < 2)
            return;

        Tile floor = Board.GetTileByPosition(action.ActionSteps[0].ActionDestinationPosition.Value);
        Tile hole = Board.GetTileByPosition(action.ActionSteps[1].ActionDestinationPosition.Value);

        floor.Transform(OtherTileType(floor.TileType));
        hole.Transform(OtherTileType(hole.TileType));

        if (!action.ActionSteps[0].CharacterInAction.IsHypnotized())
            GameplayEvents.ActionFinished(action);
    }

    public void AbortAction()
    {
        ActionUtils.Clear(actionDestinations);
        ActionRegistry.Remove(this);
        characterInAction = null;
        selectedFloor = null;
        selectedHole = null;
    }

    private TileType OtherTileType(TileType tileType)
    {
        return tileType == TileType.EmptyTile ? TileType.FloorTile : TileType.EmptyTile;
    }

    private List<Vector3> FindSelectableFloorPositions(Character character)
    {
        return Board.GetTilesUntilClosestCharactersInAllDirections(Board.GetTileByCharacter(character), RepairAA.pattern, RepairAA.range)
            .FindAll(tile => tile.isChangeable() && tile.IsFloor() && !tile.IsOccupied())
            .ConvertAll(tile => tile.gameObject.transform.position);
    }

    private List<Vector3> FindSelectableHolePositions(Character character)
    {
        return Board.GetTilesUntilClosestCharactersInAllDirections(Board.GetTileByCharacter(character), RepairAA.pattern, RepairAA.range)
            .FindAll(tile => tile.isChangeable() && tile.IsHole())
            .ConvertAll(tile => tile.gameObject.transform.position);
    }
}
