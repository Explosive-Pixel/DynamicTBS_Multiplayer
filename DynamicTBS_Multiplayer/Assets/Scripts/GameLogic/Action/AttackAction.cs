using System.Collections.Generic;
using UnityEngine;

public class AttackAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject attackCirclePrefab;

    [SerializeField] private int attackDamage;
    [SerializeField] private PatternType attackPattern;

    public ActionType ActionType { get { return ActionType.Attack; } }

    private List<GameObject> targets = new();
    public List<GameObject> ActionDestinations { get { return targets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public static int AttackDamage;

    private void Awake()
    {
        AttackDamage = attackDamage;

        GameEvents.OnGamePhaseStart += Register;
    }

    public void ShowActionPattern(Character character)
    {
        int range = character.AttackRange;
        Tile tile = Board.GetTileByCharacter(character);

        List<Vector3> patternPositions = Board.GetTilesInAllDirections(tile, attackPattern, range)
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
            targets = ActionUtils.InstantiateActionPositions(this, targetPositions, attackCirclePrefab);
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToAttack = tile.CurrentInhabitant;
            characterToAttack.TakeDamage(characterInAction.AttackDamage);
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
        int range = character.AttackRange;
        Tile tile = Board.GetTileByCharacter(character);

        PlayerType otherSide = PlayerManager.GetOtherSide(character.Side);

        List<Vector3> targetPositions = Board.GetTilesOfClosestCharactersOfSideInAllDirections(tile, otherSide, attackPattern, range)
            .FindAll(tile => tile.IsOccupied() && tile.CurrentInhabitant.isAttackableBy(character))
            .ConvertAll(tile => tile.gameObject.transform.position);

        return targetPositions;
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        ActionRegistry.Register(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
