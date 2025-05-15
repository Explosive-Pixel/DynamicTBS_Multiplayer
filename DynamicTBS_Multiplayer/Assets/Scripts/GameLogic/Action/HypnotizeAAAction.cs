using System.Collections.Generic;
using UnityEngine;

public class HypnotizeAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject selectCharacterPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private Character selectedCharacter = null;

    private List<GameObject> actionDestinations = new();
    public List<GameObject> ActionDestinations { get { return actionDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetTilesInAllDirections(characterTile, HypnotizeAA.pattern, HypnotizeAA.range).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, selectCharacterPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Character> charactersToSelect = FindSelectableCharacters(character);

        return charactersToSelect.FindAll(c => c.CanPerformAction()).Count;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> charactersToSelectPositions = FindSelectableCharacterPositions(character);

        if (charactersToSelectPositions != null && charactersToSelectPositions.Count > 0)
        {
            actionDestinations = ActionUtils.InstantiateActionPositions(this, charactersToSelectPositions, selectCharacterPrefab);
            characterInAction = character;
        }
    }

    public bool ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            selectedCharacter = tile.CurrentInhabitant;
            HypnotizedState.Create(selectedCharacter.gameObject, characterInAction);
        }

        ActionUtils.Clear(actionDestinations);

        return false;
    }

    public void AbortAction()
    {
        ActionUtils.Clear(actionDestinations);
        ActionRegistry.Remove(this);
        characterInAction = null;
        selectedCharacter = null;
    }

    private List<Character> FindSelectableCharacters(Character character)
    {
        return Board.GetTilesOfClosestCharactersOfSideInAllDirections(Board.GetTileByCharacter(character), PlayerManager.GetOtherSide(character.Side), HypnotizeAA.pattern, HypnotizeAA.range)
            .FindAll(tile => tile.IsOccupied() && tile.CurrentInhabitant.ActiveAbility.GetType() != typeof(HypnotizeAA) && tile.CurrentInhabitant.CanPerformAction())
            .ConvertAll(tile => tile.CurrentInhabitant);
    }

    private List<Vector3> FindSelectableCharacterPositions(Character character)
    {
        return FindSelectableCharacters(character).ConvertAll(c => c.gameObject.transform.position);
    }
}
