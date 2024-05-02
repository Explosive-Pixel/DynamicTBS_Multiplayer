using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectrifyAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject electrifyPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> electrifyTargets = new();
    public List<GameObject> ActionDestinations { get { return electrifyTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetAllTilesWithinRadius(characterTile, ElectrifyAA.radius).ConvertAll(tile => tile.gameObject.transform.position);

        if (characterTile.TileType == TileType.GoalTile)
        {
            patternPositions.Add(characterTile.gameObject.transform.position);
        }

        patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, electrifyPrefab);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> floorPositions = FindElectrifyPositions(character);

        if (floorPositions != null)
        {
            return floorPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> floorPositions = FindElectrifyPositions(character);

        if (floorPositions != null && floorPositions.Count > 0)
        {
            electrifyTargets = ActionUtils.InstantiateActionPositions(this, floorPositions, electrifyPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (tile.TileType.Equals(TileType.GoalTile))
            {
                GameplayEvents.GameIsOver(characterInAction.Side, GameOverCondition.CAPTAIN_TOOK_CONTROL);
            }

            ((ElectrifyAA)characterInAction.ActiveAbility).ElectrifyTile(tile);
            //tile.SetState(TileStateType.ELECTRIFIED);
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(electrifyTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindElectrifyPositions(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> floorTiles = Board.GetAllTilesWithinRadius(characterTile, ElectrifyAA.radius)
            .FindAll(tile => tile.IsNormalFloor());

        if (characterTile.TileType == TileType.GoalTile)
        {
            floorTiles.Add(characterTile);
        }

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
