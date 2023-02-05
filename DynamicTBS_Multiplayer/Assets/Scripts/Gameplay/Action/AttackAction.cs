using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject attackCirclePrefab;

    public ActionType ActionType { get { return ActionType.Attack; } }

    private List<GameObject> targets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return targets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new List<GameObject>();

    private void Awake()
    {
        GameplayEvents.OnGameplayPhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        int range = character.GetAttackRange();
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Vector3> patternPositions = Board.GetTilesInAllStarDirections(tile, range)
            .ConvertAll(tile => tile.GetPosition());

        patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, attackCirclePrefab);
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

        if(targetPositions != null && targetPositions.Count > 0)
        {
            characterInAction = character;
            targets = ActionUtils.InstantiateActionPositions(targetPositions, attackCirclePrefab);
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToAttack = tile.GetCurrentInhabitant();
            characterToAttack.TakeDamage(characterInAction.attackDamage);
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(targets);
        characterInAction = null;
    }

    private List<Vector3> FindTargetPositions(Character character)
    {
        int range = character.GetAttackRange();
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        PlayerType otherSide = PlayerManager.GetOtherPlayer(character.GetSide()).GetPlayerType();

        List<Vector3> targetPositions = Board.GetTilesOfClosestCharactersOfSideInAllStarDirections(tile, otherSide, range)
            .FindAll(tile => tile.IsOccupied() && tile.GetCurrentInhabitant().isAttackableBy(character))
            .ConvertAll(tile => tile.GetTileGameObject().transform.position);

        return targetPositions;
    }
    
    private void Register()
    {
        ActionRegistry.Register(this);
        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameplayPhaseStart -= Register;
    }
}
