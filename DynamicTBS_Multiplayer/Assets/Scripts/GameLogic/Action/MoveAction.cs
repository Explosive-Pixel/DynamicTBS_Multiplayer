using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject moveCirclePrefab;

    public ActionType ActionType { get { return ActionType.Move; } }

    private List<GameObject> moveDestinations = new();
    public List<GameObject> ActionDestinations { get { return moveDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    private void Awake()
    {
        GameEvents.OnGamePhaseStart += Register;
    }

    public static void MoveCharacter(Character character, Tile tile)
    {
        Debug.Log("Move character " + character + " to tile " + tile);
        character.gameObject.transform.SetParent(tile.gameObject.transform);
        character.gameObject.transform.position = tile.gameObject.transform.position;
    }

    public void ShowActionPattern(Character character)
    {
        List<Vector3> patternPositions = FindMovePositions(character, true);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(this, patternPositions, moveCirclePrefab);
        }
    }

    public void HideActionPattern()
    {
        ActionUtils.Clear(patternTargets);
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

        if (movePositions != null)
        {
            moveDestinations = ActionUtils.InstantiateActionPositions(this, movePositions, moveCirclePrefab);
            characterInAction = character;
        }
    }

    public ActionStep BuildAction(GameObject actionDestination)
    {
        Vector3 initialPosition = CharacterInAction.gameObject.transform.position;
        Vector3 actionDestinationPosition = actionDestination.transform.position;

        return new ActionStep()
        {
            ActionType = ActionType,
            CharacterInAction = CharacterInAction,
            CharacterInitialPosition = initialPosition,
            ActionDestinationPosition = actionDestinationPosition,
            ActionFinished = true
        };
    }

    public void ExecuteAction(Action action)
    {
        if (!action.IsAction(ActionType))
            return;

        Debug.Log("Execute Move action: " + action);
        ActionStep moveActionStep = action.ActionSteps[0];
        MoveCharacter(moveActionStep.CharacterInAction, Board.GetTileByPosition(moveActionStep.ActionDestinationPosition.Value));

        GameplayEvents.ActionFinished(action);
    }

    public void AbortAction()
    {
        ActionUtils.Clear(moveDestinations);
        characterInAction = null;
    }

    private List<Vector3> FindMovePositions(Character character, bool pattern = false)
    {
        if (character.CurrentTile == null || (!GameplayManager.HasGameStarted && !pattern))
        {
            return FindAccessibleStartPositions(character);
        }

        Tile currentTile = Board.GetTileByCharacter(character);

        if (currentTile == null) return null;

        List<Vector3> movePositions = new();

        int range = character.MoveSpeed;
        Dictionary<int, Queue<Tile>> tileQueueByDistance = new();
        int distance = 0;
        tileQueueByDistance[distance] = new Queue<Tile>();
        tileQueueByDistance[distance].Enqueue(currentTile);
        List<Tile> visited = new();
        while (distance <= range && tileQueueByDistance.ContainsKey(distance) && tileQueueByDistance[distance].Count > 0)
        {
            Tile tile = tileQueueByDistance[distance].Dequeue();
            visited.Add(tile);

            if (distance + 1 <= range)
            {
                List<Tile> neighbors = Board.GetTilesOfDistance(tile, character.MovePattern, 1);
                foreach (Tile neighbor in neighbors)
                {
                    if (!visited.Contains(neighbor) && (neighbor.IsAccessible() || pattern))
                    {
                        Vector3 position = neighbor.gameObject.transform.position;
                        if (!movePositions.Contains(position))
                        {
                            movePositions.Add(position);
                        }
                        if (!tileQueueByDistance.ContainsKey(distance + 1))
                        {
                            tileQueueByDistance[distance + 1] = new();
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

    private List<Vector3> FindAccessibleStartPositions(Character character)
    {
        if (character.CurrentTile != null)
            return new();

        List<GameObject> tiles = Board.TileGameObjects;
        return Board.Tiles.FindAll(tile => tile.Side == character.Side && tile.TileType == TileType.StartTile && tile.IsAccessible())
            .ConvertAll(tile => tile.gameObject.transform.position);
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.PLACEMENT)
        {
            ActionRegistry.Register(this);
        }
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
