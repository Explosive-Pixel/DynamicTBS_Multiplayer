using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAAAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject jumpPrefab;

    private List<GameObject> jumpTargets = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return jumpTargets; } }

    private Character characterInAction = null;

    public void CreateActionDestinations(Character character)
    {
        Tile characterTile = Board.GetTileByPosition(character.GetCharacterGameObject().transform.position);

        List<Tile> moveTiles = Board.GetTilesOfDistance(characterTile, JumpAA.movePattern, JumpAA.distance);

        List<Vector3> movePositions = moveTiles
            .FindAll(tile => tile.IsAccessible())
            .ConvertAll(tile => tile.GetPosition());

        if (movePositions.Count > 0)
        {
            jumpTargets = ActionUtils.InstantiateActionPositions(movePositions, jumpPrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Vector3 oldPosition = characterInAction.GetCharacterGameObject().transform.position;
            characterInAction.GetCharacterGameObject().transform.position = actionDestination.transform.position;
            Board.UpdateTilesAfterMove(oldPosition, characterInAction);
        }

        characterInAction.SetActiveAbilityOnCooldown();
        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(jumpTargets);
        characterInAction = null;
        ActionRegistry.Remove(this);
    }
}
