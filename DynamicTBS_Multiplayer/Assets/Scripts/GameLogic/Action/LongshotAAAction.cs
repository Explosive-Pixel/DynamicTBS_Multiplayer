using System.Collections.Generic;
using UnityEngine;

public class LongshotAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject attackCirclePrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> longshotTargets = new();
    public List<GameObject> ActionDestinations { get { return longshotTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        int range = character.AttackRange;
        Tile tile = Board.GetTileByCharacter(character);

        List<Vector3> patternPositions = Board.GetTilesInAllDirections(tile, LongshotAA.pattern, Board.Columns)
            .ConvertAll(tile => tile.gameObject.transform.position);

        patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, attackCirclePrefab);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> targetPositions = FindTargetPositions(character);

        if (targetPositions != null)
        {
            return targetPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> targetPositions = FindTargetPositions(character);

        if (targetPositions != null && targetPositions.Count > 0)
        {
            characterInAction = character;
            longshotTargets = ActionUtils.InstantiateActionPositions(this, targetPositions, attackCirclePrefab);
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToAttack = tile.CurrentInhabitant;
            characterToAttack.TakeDamage(LongshotAA.damage);
            characterInAction.TakeDamage(LongshotAA.selfDamage);
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(longshotTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindTargetPositions(Character character)
    {
        return AttackAction.FindAttackTargets(LongshotAA.pattern, Board.Columns, character);
    }
}
