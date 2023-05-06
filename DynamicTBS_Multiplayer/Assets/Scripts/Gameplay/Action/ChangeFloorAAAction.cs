using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFloorAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject changeFloorPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> changeFloorTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return changeFloorTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new List<GameObject>();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);
        List<Vector3> patternPositions = Board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radius).ConvertAll(tile => tile.GetPosition());

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, changeFloorPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> changeFloorPositions = FindChangeFloorPositions(character);

        if (changeFloorPositions != null)
        {
            return changeFloorPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> changeFloorPositions = FindChangeFloorPositions(character);

        if (changeFloorPositions != null && changeFloorPositions.Count > 0)
        {
            changeFloorTargets = ActionUtils.InstantiateActionPositions(changeFloorPositions, changeFloorPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (tile.IsOccupied())
            {
                tile.GetCurrentInhabitant().Die();
            }

            tile.Transform(OtherTileType(tile.GetTileType()));
            Tile tileBelow = Board.GetTileByCoordinates(tile.GetRow() + 1, tile.GetColumn());
            if(tileBelow != null && tileBelow.GetTileType() == TileType.EmptyTile)
            {
                tileBelow.Transform(TileType.EmptyTile);
            }
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(changeFloorTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private TileType OtherTileType(TileType tileType)
    {
        return tileType == TileType.EmptyTile ? TileType.FloorTile : TileType.EmptyTile;
    }

    private List<Vector3> FindChangeFloorPositions(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> changeFlootWithInhabitantTiles = Board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radiusWithInhabitants);
        List<Tile> changeFloorWithoutInhabitantTiles = Board.GetAllTilesWithinRadius(characterTile, ChangeFloorAA.radius)
            .FindAll(tile => !changeFlootWithInhabitantTiles.Contains(tile) && !tile.IsOccupied());

        List<Vector3> changeFloorPositions = Enumerable.Union(changeFlootWithInhabitantTiles, changeFloorWithoutInhabitantTiles)
            .ToList()
            .FindAll(tile => tile.isChangeable())
            .ConvertAll(tile => tile.GetPosition());

        return changeFloorPositions;
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
