using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject moveCirclePrefab;

    public ActionType ActionType { get { return ActionType.Move; } }

    private List<GameObject> moveDestinations = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return moveDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private void Awake()
    {
        PlacementEvents.OnPlacementStart += Register;
    }

    public int CountActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null)
        {
            return movePositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(Character character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

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
        if(!GameplayManager.HasGameStarted())
        {
            return Board.FindStartTiles(character).ConvertAll(tile => tile.GetPosition());
        }

        Tile currentTile = Board.GetTileByCharacter(character);

        if (currentTile == null) return null;

        List<Vector3> movePositions = new List<Vector3>();

        int range = character.GetMoveSpeed();
        Dictionary<int, Queue<Tile>> tileQueueByDistance = new Dictionary<int, Queue<Tile>>();
        int distance = 0;
        tileQueueByDistance[distance] = new Queue<Tile>();
        tileQueueByDistance[distance].Enqueue(currentTile);
        List<Tile> visited = new List<Tile>();
        while (distance <= range && tileQueueByDistance.ContainsKey(distance) && tileQueueByDistance[distance].Count > 0)
        {
            Tile tile = tileQueueByDistance[distance].Dequeue();
            visited.Add(tile);

            if (distance + 1 <= range)
            {
                List<Tile> neighbors = Board.GetNeighbors(tile, character.movePattern);
                foreach (Tile neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor) && neighbor.IsAccessible())
                    {
                        movePositions.Add(neighbor.GetTileGameObject().transform.position);
                        if (!tileQueueByDistance.ContainsKey(distance + 1))
                        {
                            tileQueueByDistance[distance + 1] = new Queue<Tile>();
                        }
                        tileQueueByDistance[distance + 1].Enqueue(neighbor);
                    }
                }
            }

            if (tileQueueByDistance[distance].Count == 0) 
            {
                distance++;
            }
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
