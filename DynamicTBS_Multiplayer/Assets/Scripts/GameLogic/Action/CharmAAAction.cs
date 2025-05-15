using System.Collections.Generic;
using UnityEngine;

public class CharmAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject charmPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> charmTargets = new();
    public List<GameObject> ActionDestinations { get { return charmTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile tile = Board.GetTileByCharacter(character);

        List<Vector3> patternPositions = Board.GetTilesInAllDirections(tile, CharmAA.pattern, CharmAA.range)
            .ConvertAll(tile => tile.gameObject.transform.position);

        patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, charmPrefab);
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> charmPositions = FindCharmPositions(character);

        if (charmPositions != null)
        {
            return charmPositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> charmPositions = FindCharmPositions(character);

        if (charmPositions != null && charmPositions.Count > 0)
        {
            charmTargets = ActionUtils.InstantiateActionPositions(this, charmPositions, charmPrefab);
            characterInAction = character;
        }
    }

    public bool ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Character characterToCharm = tile.CurrentInhabitant;
            CharmedState.Create(characterToCharm.gameObject, CharmAA.duration);
        }

        AbortAction();

        return true;
    }

    public void AbortAction()
    {
        ActionUtils.Clear(charmTargets);
        ActionRegistry.Remove(this);
        characterInAction = null;
    }

    private List<Vector3> FindCharmPositions(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> charmTiles = Board.GetTilesOfClosestCharactersOfSideInAllDirections(characterTile, PlayerManager.GetOtherSide(character.Side), CharmAA.pattern, CharmAA.range)
            .FindAll(tile => tile.IsOccupied() && tile.CurrentInhabitant.ActiveAbility.GetType() != typeof(CharmAA));

        List<Vector3> charmPositions = charmTiles.ConvertAll(tile => tile.gameObject.transform.position);

        return charmPositions;
    }
}
