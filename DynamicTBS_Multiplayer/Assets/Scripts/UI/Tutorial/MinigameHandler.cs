using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TileLayout
{
    public TileType tileType;
    public PlayerType side;
}

[Serializable]
public class RowLayout
{
    public List<TileLayout> tiles;
}

[Serializable]
public class BoardLayout
{
    public List<RowLayout> rows;
    public float startX;
    public float startY;
}

[Serializable]
public class CharacterLayout
{
    public CharacterType characterType;
    public PlayerType side;
    public int startRow;
    public int startColumn;
}

public class MinigameHandler : MonoBehaviour
{
    [Header("Layout")]
    [SerializeField] private BoardLayout boardSetup;
    [SerializeField] private List<CharacterLayout> characterSetup;
    [SerializeField] private int characterInActionIndex = 0;
    [SerializeField] private List<ActionType> availableActions;
    [SerializeField] private bool showRefreshButton;

    [Header("ResetTime")]
    [SerializeField] private float resetTime = 10;

    [Header("GameObjects")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject boardGameObject;
    [SerializeField] private GameObject activeAbilityButton;
    [SerializeField] private GameObject refreshButton;
    [SerializeField] private GameObject refreshButton_pink;
    [SerializeField] private GameObject refreshButton_blue;

    private readonly List<Tile> tiles = new();
    private Character mainCharacter = null;

    private bool started = false;
    private bool active = false;

    private float timer;

    private void Start()
    {
        Init();
        started = true;
    }

    private void OnEnable()
    {
        if (!started)
            return;

        Init();
    }

    private void OnDisable()
    {
        Delete();
    }

    private void Update()
    {
        if (!active)
            return;

        timer += Time.deltaTime;

        if (timer >= resetTime)
        {
            timer -= resetTime;
            Reset();
        }
    }

    private void Reset()
    {
        Delete();
        Init();
    }

    private void Init()
    {
        InitBoard(boardSetup, tilePrefab);
        CreateCharacter(characterSetup);
        AddButtons(availableActions, showRefreshButton);

        UpdateGlobalScripts();

        active = true;
    }

    private void UpdateGlobalScripts()
    {
        GameManager.GameType = GameType.LOCAL;
        PlayerManager.CurrentPlayer = characterSetup[characterInActionIndex].side;
        Board.InitBoard(tiles);
        AddActions(availableActions);
    }

    private void InitBoard(BoardLayout layout, GameObject tilePrefab)
    {
        float startX = layout.startX;
        float startY = layout.startY;
        float size = tilePrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        float x = startX;
        float y = startY;
        int rowIndex = 0;
        int columnIndex;

        foreach (RowLayout row in layout.rows)
        {
            columnIndex = 0;
            foreach (TileLayout tileLayout in row.tiles)
            {
                GameObject tileGameObject = Instantiate(tilePrefab);
                tileGameObject.transform.SetParent(boardGameObject.transform);
                tileGameObject.transform.position = new Vector3(x, y, 1);
                tileGameObject.SetActive(true);
                Tile tile = tileGameObject.GetComponent<Tile>();
                tile.Init(tileLayout.tileType, tileLayout.side, rowIndex, columnIndex);
                tiles.Add(tile);
                x += size;
                columnIndex++;
            }
            x = startX;
            y += size;
            rowIndex++;
        }
    }

    private void CreateCharacter(List<CharacterLayout> characterSetup)
    {
        Board.InitBoard(tiles);

        int index = 0;
        foreach (CharacterLayout characterLayout in characterSetup)
        {
            Character character = CharacterFactory.CreateCharacter(characterLayout.characterType, characterLayout.side);
            character.gameObject.GetComponent<Transform>().localScale = new Vector3(0.3125f, 0.3125f, 0.3125f);
            character.IsClickable = index == characterInActionIndex;
            if (index == characterInActionIndex)
                mainCharacter = character;
            Tile tile = Board.GetTileByCoordinates(characterLayout.startRow, characterLayout.startColumn);
            MoveAction.MoveCharacter(character, tile);
            index++;
        }
    }

    private void AddActions(List<ActionType> availableActions)
    {
        ActionRegistry.RemoveAll();

        if (availableActions.Contains(ActionType.Move))
        {
            MoveAction moveAction = GameObject.Find("ActionRegistry").GetComponent<MoveAction>();
            ActionRegistry.Register(moveAction);
        }

        if (availableActions.Contains(ActionType.Attack))
        {
            AttackAction attackAction = GameObject.Find("ActionRegistry").GetComponent<AttackAction>();
            ActionRegistry.Register(attackAction);
        }
    }

    private void AddButtons(List<ActionType> availableActions, bool showRefreshButton)
    {
        if (availableActions.Contains(ActionType.ActiveAbility))
        {
            List<AbilityClass> activeAbilityIcons = activeAbilityButton.GetComponentsInChildren<AbilityClass>(true).ToList();
            activeAbilityIcons.ForEach(aaIcon => aaIcon.gameObject.SetActive(aaIcon.activeAbilityType == mainCharacter.ActiveAbility.AbilityType && aaIcon.disabled == !mainCharacter.MayPerformActiveAbility() && (aaIcon.side == mainCharacter.Side || aaIcon.disabled)));
        }
        activeAbilityButton.SetActive(availableActions.Contains(ActionType.ActiveAbility));

        refreshButton_blue.SetActive(mainCharacter.Side == PlayerType.blue);
        refreshButton_pink.SetActive(mainCharacter.Side == PlayerType.pink);
        refreshButton.SetActive(showRefreshButton);
    }

    private void Delete()
    {
        active = false;

        GameplayEvents.ChangeCharacterSelection(null);
        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i].gameObject);
        }

        tiles.Clear();
        mainCharacter = null;
    }
}
