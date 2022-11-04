using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject moveCirclePrefab;

    private void Awake()
    {
        PlacementEvents.OnPlacementStart += Register;
    }

    private List<GameObject> moveDestinations = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return moveDestinations; } }

    private Character characterInAction = null;

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> movePositions = GameManager.HasGameStarted() ? FindMovePositions(character) : Board.FindStartTiles(character).ConvertAll(tile => tile.GetPosition());

        if(movePositions != null)
        {
            moveDestinations = ActionUtils.InstantiateActionPositions(movePositions, moveCirclePrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        Vector3 oldPosition = characterInAction.GetCharacterGameObject().transform.position;
        characterInAction.GetCharacterGameObject().transform.position = actionDestination.transform.position;

        Board.UpdateTilesAfterMove(oldPosition, characterInAction);

        AbortAction();
    }

    public void AbortAction() {
        ActionUtils.Clear(moveDestinations);
        characterInAction = null; 
    }

    private List<Vector3> FindMovePositions(Character character)
    {
        Tile currentTile = Board.GetTileByCharacter(character);

        if (currentTile == null) return null;

        List<Vector3> movePositions = new List<Vector3>();

        int range = character.GetMoveSpeed();
        Queue<Tile> queue = new Queue<Tile>();
        List<Tile> visited = new List<Tile>();
        queue.Enqueue(currentTile);
        while (queue.Count > 0 && range > 0)
        {
            Tile tile = queue.Dequeue();
            visited.Add(tile);

            List<Tile> neighbors = Board.GetNeighbors(tile, character.movePattern);
            foreach (Tile neighbor in neighbors)
            {
                if (!visited.Contains(neighbor) && neighbor.IsAccessible())
                {
                    movePositions.Add(neighbor.GetTileGameObject().transform.position);
                    queue.Enqueue(neighbor);
                }
            }
            range--;
        }

        return movePositions;
    }

    private void Register()
    {
        ActionRegistry.Register(this);
    }

    private void OnDestroy()
    {
        PlacementEvents.OnPlacementStart -= Register;
    }
}
