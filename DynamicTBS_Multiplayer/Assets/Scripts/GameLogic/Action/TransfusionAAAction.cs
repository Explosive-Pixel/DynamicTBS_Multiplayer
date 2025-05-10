using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransfusionAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject patternPrefab;
    [SerializeField] private GameObject stealHPCharacterPrefab;
    [SerializeField] private GameObject giveHPCharacterPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private Tile stealHPCharacter = null;
    private Tile giveHPCharacter = null;

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
        List<Vector3> changeFloorPositions = FindCharacterToTransfuseHPPositions(character);

        if (stealHPCharacter != null)
            changeFloorPositions = changeFloorPositions.FindAll(pos => pos != stealHPCharacter.gameObject.transform.position);

        if (changeFloorPositions != null && changeFloorPositions.Count > 0)
        {
            actionDestinations = ActionUtils.InstantiateActionPositions(this, changeFloorPositions, stealHPCharacter == null ? stealHPCharacterPrefab : giveHPCharacterPrefab);
            characterInAction = character;
        }
    }

    public bool ExecuteAction(GameObject actionDestination)
    {
        /*Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            if (selectedFloor == null)
            {
                selectedFloor = tile;
                ActionUtils.Clear(actionDestinations);
                CreateActionDestinations(characterInAction);
            }
            else
            {
                selectedHole = tile;
                selectedFloor.Transform(OtherTileType(selectedFloor.TileType));
                selectedHole.Transform(OtherTileType(selectedHole.TileType));
            }

            return selectedHole != null;
        }

        AbortAction();*/
        return false;
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
