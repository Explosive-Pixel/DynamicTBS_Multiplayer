using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject moveCirclePrefab;

    [SerializeField] private PatternType movePattern;

    public ActionType ActionType { get { return ActionType.Move; } }

    private List<GameObject> moveDestinations = new();
    public List<GameObject> ActionDestinations { get { return moveDestinations; } }

    private CharacterMB characterInAction = null;
    public CharacterMB CharacterInAction { get { return characterInAction; } }

    private List<GameObject> patternTargets = new();

    public static PatternType MovePattern;

    private void Awake()
    {
        MovePattern = movePattern;

        GameEvents.OnGamePhaseStart += Register;
    }

    public static void MoveCharacter(CharacterMB character, TileMB tile)
    {
        character.gameObject.transform.SetParent(tile.gameObject.transform);
        character.gameObject.transform.position = tile.gameObject.transform.position;
    }

    public void ShowActionPattern(CharacterMB character)
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

    public int CountActionDestinations(CharacterMB character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null)
        {
            return movePositions.Count;
        }

        return 0;
    }

    public void CreateActionDestinations(CharacterMB character)
    {
        List<Vector3> movePositions = FindMovePositions(character);

        if (movePositions != null)
        {
            moveDestinations = ActionUtils.InstantiateActionPositions(movePositions, moveCirclePrefab);
            characterInAction = character;
        }
    }

    public void ExecuteAction(GameObject actionDestination)
    {
        MoveCharacter(characterInAction, BoardNew.GetTileByPosition(actionDestination.transform.position));

        AbortAction();
    }

    public void AbortAction()
    {
        ActionUtils.Clear(moveDestinations);
        characterInAction = null;
    }

    private List<Vector3> FindMovePositions(CharacterMB character, bool pattern = false)
    {
        if (!GameplayManager.HasGameStarted)
        {
            return FindAccessibleStartPositions(character.Side);
        }

        TileMB currentTile = BoardNew.GetTileByCharacter(character);
        Debug.Log("current Tile: " + currentTile);

        if (currentTile == null) return null;

        List<Vector3> movePositions = new();

        int range = character.MoveSpeed;
        Debug.Log("Range: " + range);
        Dictionary<int, Queue<TileMB>> tileQueueByDistance = new();
        int distance = 0;
        tileQueueByDistance[distance] = new Queue<TileMB>();
        tileQueueByDistance[distance].Enqueue(currentTile);
        List<TileMB> visited = new();
        while (distance <= range && tileQueueByDistance.ContainsKey(distance) && tileQueueByDistance[distance].Count > 0)
        {
            TileMB tile = tileQueueByDistance[distance].Dequeue();
            visited.Add(tile);

            if (distance + 1 <= range)
            {
                List<TileMB> neighbors = BoardNew.GetTilesOfDistance(tile, movePattern, 1);
                foreach (TileMB neighbor in neighbors)
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

        Debug.Log("Move Positions: " + movePositions);

        return movePositions;
    }

    private List<Vector3> FindAccessibleStartPositions(PlayerType side)
    {
        List<GameObject> tiles = BoardNew.TileGameObjects;
        return BoardNew.Tiles.FindAll(tile => tile.Side == side && tile.TileType == TileType.StartTile && tile.IsAccessible())
            .ConvertAll(tile => tile.gameObject.transform.position);
    }

    private void Register(GamePhase gamePhase)
    {
        if (gamePhase == GamePhase.PLACEMENT)
            ActionRegistry.Register(this);

        if (gamePhase == GamePhase.GAMEPLAY)
            ActionRegistry.RegisterPatternAction(this);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= Register;
    }
}
