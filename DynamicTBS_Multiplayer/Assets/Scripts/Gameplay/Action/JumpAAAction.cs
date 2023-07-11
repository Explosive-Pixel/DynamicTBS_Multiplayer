using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject jumpPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> jumpTargets = new();
    public List<GameObject> ActionDestinations { get { return jumpTargets; } }

    private CharacterMB characterInAction = null;
    public CharacterMB CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(CharacterMB character)
    {
        List<Vector3> patternPositions = FindMovePositions(character, true);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, jumpPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(CharacterMB character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null)
        {
            return movePositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(CharacterMB character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null && movePositions.Count > 0)
        {
            jumpTargets = ActionUtils.InstantiateActionPositions(movePositions, jumpPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        TileMB tile = BoardNew.GetTileByPosition(actionDestination.transform.position);
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

    private List<Vector3> FindMovePositions(CharacterMB character, bool pattern = false)
    {
        TileMB characterTile = BoardNew.GetTileByCharacter(character);

        List<TileMB> moveTiles = BoardNew.GetTilesOfDistance(characterTile, JumpAA.movePattern, JumpAA.distance);

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
