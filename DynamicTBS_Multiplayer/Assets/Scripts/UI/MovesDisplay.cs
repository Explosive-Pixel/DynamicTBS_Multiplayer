using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovesDisplay : MonoBehaviour
{
    [SerializeField] private GameObject movesDisplayObject;
    [SerializeField] private Text displayText;
    private List<string> movesList = new List<string>();

    private int actionCount = 0;

    private void Awake()
    {
        SubscribeEvents();
        movesDisplayObject.SetActive(false);
    }

    private void WriteMovesToString(ActionMetadata actionMetadata)
    {
        string newLine = GetMoveCountString() + ": ";

        if (actionMetadata.ExecutedActionType == ActionType.Skip)
        {
            newLine += TranslatePlayerSide(actionMetadata.ExecutingPlayer.GetPlayerType()) + "ended their turn";
        }
        else
        {
            newLine += TranslateCharacterName(actionMetadata.CharacterInAction)
            + "on "
            + TranslateTilePosition(actionMetadata.CharacterInitialPosition)
            + TranslateActionType(actionMetadata.ExecutedActionType, actionMetadata.CharacterInAction)
            + TranslateTilePosition(actionMetadata.ActionDestinationPosition);
        }

        newLine += "\n";

        DisplayMoves(newLine);
    }

    private void WriteAbortTurnToString(int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        PlayerType player = GetPlayerTypeByActionCount();
        string newLine = TranslatePlayerSide(player).Trim() + "'s turn was aborted since " + player + " ";
        if(abortTurnCondition == AbortTurnCondition.NO_AVAILABLE_ACTION)
        {
            newLine += "had no more available action.";
        } else if(abortTurnCondition == AbortTurnCondition.PLAYER_TIMEOUT)
        {
            newLine += "ran out of time.";
        }

        newLine += "\n";

        actionCount += remainingActions;

        DisplayMoves(newLine);
    }

    private void DisplayMoves(string newMove)
    {
        movesList.Add(newMove);
        displayText.text = "";

        for (int i = movesList.Count - 1; i >= 0; i--)
        {
            displayText.text += movesList[i];
        }
    }

    private void EmptyList(PlayerType? winner, GameOverCondition endGameCondition)
    {
        movesList.Clear();
    }

    private string GetMoveCountString()
    {
        int counter = (actionCount / (GameplayManager.maxActionsPerRound * 2)) + 1;
        PlayerType player = GetPlayerTypeByActionCount();
        char addition = (char)((actionCount % GameplayManager.maxActionsPerRound) + 65);

        string text = TranslatePlayerSide(player) + " " + counter.ToString() + addition;

        actionCount += 1;
        return text;
    }

    private PlayerType GetPlayerTypeByActionCount()
    {
        return actionCount % (GameplayManager.maxActionsPerRound * 2) <= 1 ? PlayerManager.StartPlayer[GamePhase.GAMEPLAY] : PlayerManager.GetOtherSide(PlayerManager.StartPlayer[GamePhase.GAMEPLAY]);
    }

    private string TranslateTilePosition(Vector3? position)
    {
        string text = "";

        if (position != null)
        {
            Tile tile = Board.GetTileByPosition(position.GetValueOrDefault());
            if (tile != null)
            {
                text = tile.GetTileName();
            }
        }
        return text;
    }

    private string TranslateActionType(ActionType actiontype, Character character)
    {
        string text = "";

        if (actiontype == ActionType.Move)
            text = " moved to ";
        if (actiontype == ActionType.Attack)
            text = " attacked ";
        if (actiontype == ActionType.ActiveAbility)
        {
            if (character.GetCharacterType() == CharacterType.MasterChar)
                text = " used Take Control on ";
            if (character.GetCharacterType() == CharacterType.TankChar)
                text = " used Block on ";
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                text = " used Powershot on ";
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                text = " used Jump on ";
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                text = " used Change Floor on ";
            if (character.GetCharacterType() == CharacterType.MedicChar)
                text = " used Heal on ";
        }
        return text;
    }

    private string TranslateCharacterName(Character character)
    {
        string text = "";

        if (character != null)
        {
            if (character.GetCharacterType() == CharacterType.MasterChar)
                text = "Captain ";
            if (character.GetCharacterType() == CharacterType.TankChar)
                text = "Tank ";
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                text = "Shooter ";
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                text = "Runner ";
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                text = "Mechanic ";
            if (character.GetCharacterType() == CharacterType.MedicChar)
                text = "Doc ";
        }
        return text;
    }

    private string TranslatePlayerSide(PlayerType playerType)
    {
        string text = "";
        
        if (playerType == PlayerType.blue)
        {
            text = "Blue ";
        }
        else
        {
            text = "Pink ";
        }

        return text;
    }

    private void OnGameplayPhaseStarts(GamePhase gamePhase)
    {
        if(gamePhase == GamePhase.GAMEPLAY)
        {
            ActivateMovesDisplay();
            ActivateRecordingSubscription();
        }
    }

    #region ActivationRegion
    private void ActivateMovesDisplay()
    {
        movesDisplayObject.SetActive(true);
    }

    private void DeactivateMovesDisplay(PlayerType? winner, GameOverCondition endGameCondition)
    {
        movesDisplayObject.SetActive(false);
    }

    private void ActivateRecordingSubscription()
    {
        GameplayEvents.OnFinishAction += WriteMovesToString;
        GameplayEvents.OnPlayerTurnAborted += WriteAbortTurnToString;
    }
    #endregion

    #region EventsRegion
    private void SubscribeEvents()
    {
        GameEvents.OnGamePhaseStart += OnGameplayPhaseStarts;
        GameplayEvents.OnRestartGame += ActivateMovesDisplay;
        GameplayEvents.OnGameOver += EmptyList;
        GameplayEvents.OnGameOver += DeactivateMovesDisplay;
    }

    private void UnsubscribeEvents()
    {
        GameEvents.OnGamePhaseStart -= OnGameplayPhaseStarts;
        GameplayEvents.OnRestartGame -= ActivateMovesDisplay;
        GameplayEvents.OnGameOver -= DeactivateMovesDisplay;
        GameplayEvents.OnGameOver -= EmptyList;
        GameplayEvents.OnFinishAction -= WriteMovesToString;
        GameplayEvents.OnPlayerTurnAborted -= WriteAbortTurnToString;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion
}