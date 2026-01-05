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
    [SerializeField] private float resetTime = -1;

    [Header("GameObjects")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject boardGameObject;
    [SerializeField] private GameObject activeAbilityButton;
    [SerializeField] private GameObject refreshButton;
    [SerializeField] private GameObject refreshButton_disabled;
    [SerializeField] private GameObject refreshButton_enabled;
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
        if (!active || resetTime <= 0)
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

        ActionHandler.Instance.InstantiateAllActionPositions(mainCharacter);
        GameplayEvents.ChangeCharacterSelection(mainCharacter);
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

        float size = tilePrefab
            .GetComponent<SpriteRenderer>()
            .sprite.bounds.size.x;

        int rowIndex = 0;

        foreach (RowLayout row in layout.rows)
        {
            int columnIndex = 0;

            foreach (TileLayout tileLayout in row.tiles)
            {
                GameObject tileGO = Instantiate(tilePrefab);
                tileGO.SetActive(true);

                tileGO.transform.SetParent(boardGameObject.transform, false);

                tileGO.transform.localPosition = new Vector3(
                    startX + columnIndex * size,
                    startY + rowIndex * size,
                    1
                );

                tileGO.transform.localScale = Vector3.one;

                Tile tile = tileGO.GetComponent<Tile>();
                tile.Init(tileLayout.tileType, tileLayout.side, rowIndex, columnIndex);
                tiles.Add(tile);

                columnIndex++;
            }

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
            GameplayEvents.OnCharacterSelectionChange += UpdateAbilityButton;
        }
        activeAbilityButton.SetActive(availableActions.Contains(ActionType.ActiveAbility));

        if (showRefreshButton)
        {
            refreshButton_blue.SetActive(mainCharacter.Side == PlayerType.blue);
            refreshButton_pink.SetActive(mainCharacter.Side == PlayerType.pink);
            refreshButton_disabled.SetActive(true);
            refreshButton_enabled.SetActive(false);
            GameplayEvents.OnFinishAction += UpdateRefreshButton;
        }

        refreshButton.SetActive(showRefreshButton);
    }

    private void UpdateAbilityButton(Character character)
    {
        if (!active)
            return;

        if (character == null)
        {
            activeAbilityButton.SetActive(false);
            return;
        }

        List<AbilityClass> activeAbilityIcons = activeAbilityButton.GetComponentsInChildren<AbilityClass>(true).ToList();
        activeAbilityIcons.ForEach(aaIcon => aaIcon.gameObject.SetActive(aaIcon.activeAbilityType == character.ActiveAbility.AbilityType && aaIcon.disabled == !character.MayPerformActiveAbility() && (aaIcon.side == character.Side || aaIcon.disabled)));
        activeAbilityButton.SetActive(true);
    }

    private void UpdateRefreshButton(Action action)
    {
        refreshButton_disabled.SetActive(!mainCharacter.IsActiveAbilityOnCooldown());
        refreshButton_enabled.SetActive(mainCharacter.IsActiveAbilityOnCooldown());
    }

    private void Delete()
    {
        GameplayEvents.ChangeCharacterSelection(null);

        active = false;

        GameplayEvents.OnCharacterSelectionChange -= UpdateAbilityButton;
        GameplayEvents.OnFinishAction -= UpdateRefreshButton;

        for (int i = 0; i < tiles.Count; i++)
        {
            Destroy(tiles[i].gameObject);
        }

        tiles.Clear();
        mainCharacter = null;
    }

    private void OnDestroy()
    {
        GameplayEvents.OnCharacterSelectionChange -= UpdateAbilityButton;
        GameplayEvents.OnFinishAction -= UpdateRefreshButton;
    }
}
