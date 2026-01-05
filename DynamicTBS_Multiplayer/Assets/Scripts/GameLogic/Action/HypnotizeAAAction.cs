using System.Collections.Generic;
using System.Linq;
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

    public ActionStep BuildAction(GameObject actionDestination)
    {
        Vector3 initialPosition = characterInAction.gameObject.transform.position;
        Character character = CharacterInAction;

        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            selectedCharacter = tile.CurrentInhabitant;
            HypnotizedState.Create(selectedCharacter.gameObject);
        }

        ActionStep actionStep = new()
        {
            ActionType = ActionType,
            CharacterInAction = character,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestination.transform.position,
            ActionFinished = false
        };

        ActionUtils.Clear(actionDestinations);

        return actionStep;
    }

    public void ExecuteAction(Action action)
    {
        if (!action.IsAction(ActionType) || action.ActionSteps.Count < 2)
            return;

        Tile tile = Board.GetTileByPosition(action.ActionSteps[0].ActionDestinationPosition.Value);
        if (tile == null)
            return;

        HypnotizedState.Create(tile.CurrentInhabitant.gameObject);

        Action secondAction = new()
        {
            ExecutingPlayer = action.ExecutingPlayer,
            ActionSteps = action.ActionSteps.Skip(1).ToList()
        };
        ActionHandler.Instance.ExecuteAction(secondAction);

        GameplayEvents.ActionFinished(action);
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
