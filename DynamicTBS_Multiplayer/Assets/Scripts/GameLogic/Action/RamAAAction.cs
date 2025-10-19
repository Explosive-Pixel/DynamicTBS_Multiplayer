using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RamAAAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject ramPrefab;

    public ActionType ActionType { get { return ActionType.ActiveAbility; } }

    private List<GameObject> ramTargets = new();
    public List<GameObject> ActionDestinations { get { return ramTargets; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public void ShowActionPattern(Character character)
    {
        List<Vector3> patternPositions = FindRamPositions(character, true);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, ramPrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindRamPositions(character);

        if (movePositions != null)
        {
            return movePositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindRamPositions(character);

        if (movePositions != null && movePositions.Count > 0)
        {
            ramTargets = ActionUtils.InstantiateActionPositions(this, movePositions, ramPrefab);
            characterInAction = character;
        }
    }

    public ActionStep ExecuteAction(GameObject actionDestination)
    {
        Vector3 initialPosition = characterInAction.gameObject.transform.position;

        Tile tile = Board.GetTileByPosition(actionDestination.transform.position);
        if (tile != null)
        {
            Tile currentTile = characterInAction.CurrentTile;
            Tile nextTile = tile;
            List<Tile> neighbors = new();

            while (nextTile != null)
            {
                neighbors.Add(nextTile);

                if (!nextTile.IsOccupied())
                    break;

                Tile neighbor = nextTile;
                nextTile = Board.GetNextTileInSameDirection(currentTile, nextTile);
                currentTile = neighbor;
            }

            if (neighbors.Last().IsOccupied())
            {
                foreach (Tile t in neighbors)
                {
                    t.CurrentInhabitant.TakeDamage(RamAA.damage);
                }
                characterInAction.TakeDamage(RamAA.selfDamage);
            }
            else
            {
                for (int i = neighbors.Count - 1; i > 0; i--)
                {
                    Character c = neighbors[i - 1].CurrentInhabitant;
                    MoveAction.MoveCharacter(c, neighbors[i]);
                    if (neighbors[i].IsHole())
                        c.Die();
                    else
                        c.TakeDamage(RamAA.damage);
                }

                MoveAction.MoveCharacter(characterInAction, tile);
                if (neighbors.Count > 1)
                    characterInAction.TakeDamage(RamAA.selfDamage);
            }
        }

        return new ActionStep()
        {
            ActionType = ActionType,
            CharacterInAction = CharacterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestination.transform.position,
            ActionFinished = true
        };
    }

    public void AbortAction()
    {
        ActionUtils.Clear(ramTargets);
        characterInAction = null;
        ActionRegistry.Remove(this);
    }

    private List<Vector3> FindRamPositions(Character character, bool pattern = false)
    {
        Tile characterTile = Board.GetTileByCharacter(character);

        List<Tile> ramTiles = Board.GetTilesInAllDirections(characterTile, RamAA.pattern, RamAA.range);

        List<Vector3> ramPositions = ramTiles
            .FindAll(tile => tile.IsFloor() || pattern)
            .ConvertAll(tile => tile.gameObject.transform.position);

        return ramPositions;
    }
}
