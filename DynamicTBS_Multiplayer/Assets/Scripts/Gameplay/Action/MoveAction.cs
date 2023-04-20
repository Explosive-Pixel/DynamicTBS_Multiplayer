using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour, IAction
{
    [SerializeField]
    private GameObject moveCirclePrefab;

    [SerializeField]
    private Board board;

    public ActionType ActionType { get { return ActionType.Move; } }

    private List<GameObject> moveDestinations = new List<GameObject>();
    public List<GameObject> ActionDestinations { get { return moveDestinations; } }

    private Character characterInAction = null;
    public Character CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new List<GameObject>();
    
    private void Awake()
    {

        PlacementEvents.OnPlacementStart += Register;
        GameplayEvents.OnGameplayPhaseStart += RegisterPattern;
    }

    public void ShowActionPattern(Character character)
    {
        List<Vector3> patternPositions = FindMovePositions(character, true);

        if (patternPositions != null)
        {
            patternTargets = ActionUtils.InstantiateActionPositions(patternPositions, moveCirclePrefab);
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

    private List<Vector3> FindMovePositions(Character character, bool pattern = false)
    {
        if(!GameplayManager.HasGameStarted())
        {
            return board.FindStartTiles(character.GetSide().GetPlayerType()).ConvertAll(tile => tile.GetPosition());
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
                    if (!visited.Contains(neighbor) && (neighbor.IsAccessible() || pattern))
                    {
                        Vector3 position = neighbor.GetTileGameObject().transform.position;
                        if (!movePositions.Contains(position))
                        {
                            movePositions.Add(position);
                        }
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

    private void RegisterPattern()
    {
        ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        PlacementEvents.OnPlacementStart -= Register;
        GameplayEvents.OnGameplayPhaseStart -= RegisterPattern;
    }
}
