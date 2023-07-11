using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject blockActionPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> blockTargets = new();
    public List<GameObject> ActionDestinations { get { return blockTargets; } }

    private CharacterMB characterInAction = null;
    public CharacterMB CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(CharacterMB character)
    {
        TileMB characterTile = BoardNew.GetTileByCharacter(character);
        List<Vector3> patternPositions = BoardNew.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, blockActionPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(CharacterMB character)
    {
        List<Vector3> floorPositions = FindFloorPositions(character);

        if (floorPositions != null)
        {
            return floorPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(CharacterMB character)
    {
        List<Vector3> floorPositions = FindFloorPositions(character);

        if (floorPositions != null && floorPositions.Count > 0)
        {
            blockTargets = ActionUtils.InstantiateActionPositions(floorPositions, blockActionPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        TileMB tile = BoardNew.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            tile.Transform(TileType.EmptyTile);
            ((BlockAA)characterInAction.ActiveAbility).ActivateBlock();
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(blockTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindFloorPositions(CharacterMB character)
    {
        TileMB characterTile = BoardNew.GetTileByCharacter(character);

        List<TileMB> floorTiles = BoardNew.GetTilesOfDistance(characterTile, BlockAA.pattern, BlockAA.distance)
            .FindAll(tile => tile.IsNormalFloor() && !tile.IsOccupied());

        List<Vector3> floorPositions = floorTiles.ConvertAll(tile => tile.gameObject.transform.position);

        return floorPositions;
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
