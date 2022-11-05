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

    private void Awake()
    {
        GameplayEvents.OnGameplayPhaseStart += Register;
    }

    public void CreateActionDestinations(Character character)
    {
        int range = character.GetAttackRange();
        Tile tile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        PlayerType otherSide = PlayerManager.GetOtherPlayer(character.GetSide()).GetPlayerType();

        List<Vector3> targetPositions = Board.GetTilesOfClosestCharactersOfSideWithinRadius(tile, otherSide, range)
            .FindAll(tile => tile.IsOccupied() && tile.GetCurrentInhabitant().isAttackableBy(character))
            .ConvertAll(tile => tile.GetTileGameObject().transform.position);

        if(targetPositions.Count > 0)
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
    
    private void Register()
    {
        ActionRegistry.Register(this);
    }

    private void OnDestroy()
    {
        GameplayEvents.OnGameplayPhaseStart -= Register;
    }
}
