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
    [SerializeField] private float characterScaling;

    private static readonly List<int> DraftSequence = new();
    private static int draftCounter;
    private static int draftSequenceIndex;
    private static int currentPlayerDraftCount;
    private static readonly List<Vector3> SpawnPositions = new();
    private static Vector3 characterScaleVector;

    public static int CurrentPlayerTotalDraftCount { get { return draftSequenceIndex < DraftSequence.Count ? DraftSequence[draftSequenceIndex] : 0; } }
    public static int CurrentPlayerRemainingDraftCount { get { return CurrentPlayerTotalDraftCount - currentPlayerDraftCount; } }
    public static int MaxDraftCount { get { return DraftSequence.Sum(); } }
    public static int DraftCounter { get { return draftCounter; } }

    private static bool init = false;

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
        characterScaleVector = new Vector3(characterScaling, characterScaling, 1);

        init = true;
    }

    public static void DraftCharacter(CharacterType type, PlayerType side)
    {
        if (CurrentPlayerTotalDraftCount == 0) return;

        Character character = CharacterFactory.CreateCharacter(type, side);
        character.gameObject.transform.position = SpawnPositions[draftCounter];
        character.gameObject.transform.localScale = characterScaleVector;

        DraftEvents.CharacterCreated(character);

        AdvanceDraftOrder();

        DraftEvents.FinishDraftAction();
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

        return CurrentPlayerRemainingDraftCount;
    }

    private static void AdvanceDraftOrder()
    {
        draftCounter++;
        currentPlayerDraftCount++;

        if (currentPlayerDraftCount == CurrentPlayerTotalDraftCount)
        {
            draftSequenceIndex++;

            if (draftSequenceIndex == DraftSequence.Count)
            {
                DraftCompleted();
                return;
            }

            currentPlayerDraftCount = 0;

            PlayerManager.NextPlayer();
        }
    }

    private static void DraftCompleted()
    {
        GameEvents.EndGamePhase(GamePhase.DRAFT);
    }
}