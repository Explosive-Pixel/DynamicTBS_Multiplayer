using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public class DraftManager : MonoBehaviour
{
    [SerializeField] private List<int> draftSequence;
    [SerializeField] private List<Vector3> spawnPositions;

    private static readonly List<int> DraftSequence = new();
    private static int draftCounter;
    private static int draftSequenceIndex;
    private static int currentPlayerDraftCount;
    private static readonly List<Vector3> SpawnPositions = new();

    public static int CurrentPlayerTotalDraftCount { get { return draftSequenceIndex < DraftSequence.Count ? DraftSequence[draftSequenceIndex] : 0; } }
    public static int MaxDraftCount { get { return DraftSequence.Sum(); } }

    private static bool init = false;

    private static DraftManager Instance;

    private void Start()
    {
        GameManager.ChangeGamePhase(GamePhase.DRAFT);
    }

    private void Awake()
    {
        if (!init)
        {
            Init();
        }

        draftCounter = 0;
        draftSequenceIndex = 0;
        currentPlayerDraftCount = 0;
    }

    private void Init()
    {
        DraftSequence.AddRange(draftSequence);
        SpawnPositions.AddRange(spawnPositions);
        Instance = gameObject.GetComponent<DraftManager>();

        init = true;
    }

    public static void DraftCharacter(CharacterType type, PlayerType side)
    {
        if (CurrentPlayerTotalDraftCount == 0) return;

        Character character = CharacterFactory.CreateCharacter(type, side);
        character.gameObject.transform.position = SpawnPositions[draftCounter];

        DraftEvents.CharacterCreated(character);

        AdvanceDraftOrder();
    }

    public static void RandomDrafts(PlayerType side)
    {
        int i = GetRemainingDraftCount(side);
        while (i-- > 0)
        {
            RandomDraft(side);
        }
    }

    public static void RandomDraft(PlayerType side)
    {
        CharacterType randomCharacterType = CharacterFactory.GetRandomCharacterType();
        DraftCharacter(randomCharacterType, side);
    }

    public static int GetRemainingDraftCount(PlayerType currentPlayer)
    {
        if (PlayerManager.CurrentPlayer != currentPlayer)
        {
            return 0;
        }

        return CurrentPlayerTotalDraftCount - currentPlayerDraftCount;
    }

    private static void AdvanceDraftOrder()
    {
        draftCounter++;
        currentPlayerDraftCount++;

        if (currentPlayerDraftCount == CurrentPlayerTotalDraftCount)
        {
            currentPlayerDraftCount = 0;
            draftSequenceIndex++;
            PlayerManager.NextPlayer();
        }


        if (draftSequenceIndex == DraftSequence.Count)
        {
            DraftCompleted();
        }
    }

    private static void DraftCompleted()
    {
        Instance.StartCoroutine(DelayGoToGameOverScreen());
    }

    private static IEnumerator DelayGoToGameOverScreen()
    {
        // TODO: Timer has to stop; need to change a lot of logic ...
        yield return new WaitForSeconds(0);
        GameEvents.EndGamePhase(GamePhase.DRAFT);
    }
}