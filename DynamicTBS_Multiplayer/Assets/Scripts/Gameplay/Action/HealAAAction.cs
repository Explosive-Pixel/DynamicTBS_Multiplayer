using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject healPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> healTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return healTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> healPositions = FindHealPositions(character);

        if (healPositions != null)
        {
            return healPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> healPositions = FindHealPositions(character);

        if(healPositions != null && healPositions.Count > 0)
        {
            healTargets = ActionUtils.InstantiateActionPositions(healPositions, healPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToHeal = tile.GetCurrentInhabitant();
            characterToHeal.Heal(HealAA.healingPoints);
        }

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(healTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindHealPositions(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> healTiles = Board.GetTilesOfClosestCharactersOfSideWithinRadius(characterTile, character.GetSide().GetPlayerType(), HealAA.healingRange)
            .FindAll(tile => tile.IsOccupied() && tile.GetCurrentInhabitant().isHealableBy(character) && !tile.GetCurrentInhabitant().HasFullHP());

        List<Vector3> healPositions = healTiles.ConvertAll(tile => tile.GetPosition());

        return healPositions;
    }
}
