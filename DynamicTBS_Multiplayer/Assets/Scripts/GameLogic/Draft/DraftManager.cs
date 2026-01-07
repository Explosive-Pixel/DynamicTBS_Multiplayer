using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DraftManager : MonoBehaviour
{
    [SerializeField] private List<int> draftSequence;
    [SerializeField] private List<GameObject> spawnPositions;
    [SerializeField] private float characterScaling;
    [SerializeField] private GameObject unitInformation;

    private static readonly List<int> DraftSequence = new();
    private static int draftCounter;
    private static int draftSequenceIndex;
    private static int currentPlayerDraftCount;
    private static readonly List<Vector3> SpawnPositions = new();
    private static Vector3 characterScaleVector;
    private static GameObject characterInformation;

    public static int CurrentPlayerTotalDraftCount { get { return draftSequenceIndex < DraftSequence.Count ? DraftSequence[draftSequenceIndex] : 0; } }
    public static int CurrentPlayerRemainingDraftCount { get { return CurrentPlayerTotalDraftCount - currentPlayerDraftCount; } }
    public static int MaxDraftCount { get { return DraftSequence.Sum(); } }
    public static int DraftCounter { get { return draftCounter; } }

    private static readonly Dictionary<PlayerType, bool> draftRandom = new() { { PlayerType.blue, false }, { PlayerType.pink, false } };

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
        characterInformation = unitInformation;

        draftRandom[PlayerType.blue] = false;
        draftRandom[PlayerType.pink] = false;

        WSMsgUpdateServer.SendUpdateServerMessage(GamePhase.DRAFT);
    }

    private void Init()
    {
        DraftSequence.AddRange(draftSequence);
        SpawnPositions.AddRange(spawnPositions.ConvertAll(go => go.transform.position));
        characterScaleVector = new Vector3(characterScaling, characterScaling, 1);

        init = true;
    }

    public static void DraftCharacter(CharacterType type, PlayerType side)
    {
        if (GameManager.GameType == GameType.LOCAL)
        {
            ExecuteDraftCharacter(type, side);
            return;
        }

        WSMsgDraftCharacter.SendDraftCharacterMessage(type, side);
    }

    public static void ExecuteDraftCharacter(CharacterType type, PlayerType side)
    {
        if (CurrentPlayerTotalDraftCount == 0) return;

        Character character = CharacterFactory.CreateCharacter(type, side);
        character.gameObject.transform.position = SpawnPositions[draftCounter];
        character.gameObject.transform.localScale = characterScaleVector;

        GameObject hoverObject = GetHoverObject(type, side);
        if (hoverObject != null)
            character.gameObject.AddComponent<HoverHandler>().HoverObject = hoverObject;

        DraftEvents.CharacterCreated(character);

        AdvanceDraftOrder();

        DraftEvents.FinishDraftAction();

        if (draftRandom[side] && side == PlayerManager.CurrentPlayer
             && PlayerManager.ClientIsCurrentPlayer() && GetRemainingDraftCount(PlayerManager.CurrentPlayer) > 0)
            RandomDraft(side);
    }

    public static void StartRandomDraft(PlayerType side)
    {
        draftRandom[side] = true;

        if (side == PlayerManager.ExecutingPlayer && GetRemainingDraftCount(side) > 0)
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

    private static GameObject GetHoverObject(CharacterType type, PlayerType side)
    {
        CharacterInformationClass cic = characterInformation.GetComponentsInChildren<CharacterInformationClass>(true).ToList()
            .Find(cic => cic.characterType == type && cic.side == side);

        if (cic != null)
            return cic.gameObject;

        return null;
    }
}