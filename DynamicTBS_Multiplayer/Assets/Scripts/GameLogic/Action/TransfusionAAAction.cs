using System.Collections.Generic;
using UnityEngine;

public class TransfusionAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject patternPrefab;
    [SerializeField] private GameObject stealHPCharacterPrefab;
    [SerializeField] private GameObject giveHPCharacterPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private Character stealHPCharacter = null;
    private Character giveHPCharacter = null;

    private List<GameObject> actionDestinations = new();
    public List<GameObject> ActionDestinations { get { return actionDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        Tile characterTile = Board.GetTileByCharacter(character);
        List<Vector3> patternPositions = Board.GetTilesInAllDirections(characterTile, TransfusionAA.pattern, TransfusionAA.range).ConvertAll(tile => tile.gameObject.transform.position);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, patternPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        int characterPositionsCount = FindCharacterToTransfuseHPPositions(character).Count;

        return characterPositionsCount * (characterPositionsCount - 1);
    }

    public void CreateActionDestinations(Character character)
    {
        if (CountActionDestinations(character) == 0)
            return;

        List<Vector3> changeFloorPositions = FindCharacterToTransfuseHPPositions(character);

        if (stealHPCharacter != null)
            changeFloorPositions = changeFloorPositions.FindAll(pos => pos != stealHPCharacter.CurrentTile.gameObject.transform.position);

        if (changeFloorPositions != null && changeFloorPositions.Count > 0)
        {
            actionDestinations = ActionUtils.InstantiateActionPositions(this, changeFloorPositions, stealHPCharacter == null ? stealHPCharacterPrefab : giveHPCharacterPrefab);
            characterInAction = character;
        }
    }

    public ActionStep BuildAction(GameObject actionDestination)
    {
        Vector3 initialPosition = characterInAction.gameObject.transform.position;

        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (stealHPCharacter == null)
            {
                stealHPCharacter = tile.CurrentInhabitant;
                ActionUtils.Clear(actionDestinations);
                CreateActionDestinations(characterInAction);
            }
            else
            {
                giveHPCharacter = tile.CurrentInhabitant;
            }
        }

        return new ActionStep()
        {
            ActionType = ActionType,
            CharacterInAction = CharacterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestination.transform.position,
            ActionFinished = giveHPCharacter != null
        };
    }

    public void ExecuteAction(Action action)
    {
        if (!action.IsAction(ActionType) || action.ActionSteps.Count < 2)
            return;

        Tile steal = Board.GetTileByPosition(action.ActionSteps[0].ActionDestinationPosition.Value);
        Tile give = Board.GetTileByPosition(action.ActionSteps[1].ActionDestinationPosition.Value);

        if (steal == null || give == null)
            return;

        bool isHypnotized = action.ActionSteps[0].CharacterInAction.IsHypnotized();

        Character stealHPCharacter = steal.CurrentInhabitant;
        Character giveHPCharacter = give.CurrentInhabitant;
        stealHPCharacter.TakeDamage(TransfusionAA.hpCount);
        giveHPCharacter.Heal(TransfusionAA.hpCount);

        if (!isHypnotized)
            GameplayEvents.ActionFinished(action);
    }

    public void AbortAction()
    {
        ActionUtils.Clear(actionDestinations);
        ActionRegistry.Remove(this);
        characterInAction = null;
        stealHPCharacter = null;
        giveHPCharacter = null;
    }


    private List<Vector3> FindCharacterToTransfuseHPPositions(Character character)
    {
        return Board.GetTilesOfClosestCharactersOfSideInAllDirections(Board.GetTileByCharacter(character), PlayerType.none, TransfusionAA.pattern, TransfusionAA.range)
            .ConvertAll(tile => tile.gameObject.transform.position);
    }
}
