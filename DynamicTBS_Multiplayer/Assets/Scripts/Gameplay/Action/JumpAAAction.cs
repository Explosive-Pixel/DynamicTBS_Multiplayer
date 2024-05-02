using System.Collections;
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

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

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

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            MoveAction.MoveCharacter(characterInAction, tile);
        }

        AbortAction();
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

        List<Tile> moveTiles = Board.GetTilesOfDistance(characterTile, JumpAA.movePattern, JumpAA.distance);

        List<Vector3> movePositions = moveTiles
            .FindAll(tile => tile.IsAccessible() || pattern)
            .ConvertAll(tile => tile.gameObject.transform.position);

        return movePositions;
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
