using System.Collections.Generic;
using UnityEngine;

public class MovesInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro displayText;
    [SerializeField] private Color blue;
    [SerializeField] private Color pink;

    private readonly List<string> movesList = new();
    private int actionCount = 0;

    private void Awake()
    {
        displayText.text = "";

        GameEvents.OnGamePhaseStart += SetActive;
    }

    private void SetActive(GamePhase gamePhase)
    {
        if (gamePhase != GamePhase.GAMEPLAY)
            return;

        GameplayEvents.OnFinishAction += WriteMovesToString;
        GameplayEvents.OnPlayerTurnAborted += WriteAbortTurnToString;
    }

    private void WriteMovesToString(ActionMetadata actionMetadata)
    {
        string newLine = GetMoveCountString() + ": ";

        if (actionMetadata.ExecutedActionType == ActionType.Skip)
        {
            newLine += TranslatePlayerSide(actionMetadata.ExecutingPlayer) + " ended their turn";
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

    private void WriteAbortTurnToString(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        string newLine = TranslatePlayerSide(abortedTurnPlayer).Trim() + "'s turn was aborted since " + abortedTurnPlayer + " ";
        if (abortTurnCondition == AbortTurnCondition.NO_AVAILABLE_ACTION)
        {
            newLine += "had no more available action.";
        }
        else if (abortTurnCondition == AbortTurnCondition.PLAYER_TIMEOUT)
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

        for (int i = movesList.Count - 1; i >= Mathf.Max(0, movesList.Count - 4); i--)
        {
            displayText.text += "<color=#" + (movesList[i].StartsWith("Blue") ? ColorUtility.ToHtmlStringRGB(blue) : ColorUtility.ToHtmlStringRGB(pink)) + ">" + movesList[i] + "</color>";
        }
    }

    private string GetMoveCountString()
    {
        int counter = (actionCount / (GameplayManager.MaxActionsPerRound * 2)) + 1;
        PlayerType player = GetPlayerTypeByActionCount();
        char addition = (char)((actionCount % GameplayManager.MaxActionsPerRound) + 65);

        string text = TranslatePlayerSide(player) + " " + counter.ToString() + addition;

        actionCount += 1;
        return text;
    }

    private PlayerType GetPlayerTypeByActionCount()
    {
        return actionCount % (GameplayManager.MaxActionsPerRound * 2) <= 1 ? PlayerManager.StartPlayer[GamePhase.GAMEPLAY] : PlayerManager.GetOtherSide(PlayerManager.StartPlayer[GamePhase.GAMEPLAY]);
    }

    private string TranslateTilePosition(Vector3? position)
    {
        if (position != null)
        {
            Tile tile = Board.GetTileByPosition(position.GetValueOrDefault());
            if (tile != null)
            {
                return tile.Name;
            }
        }
        return "";
    }

    private string TranslateActionType(ActionType actiontype, Character character)
    {
        if (actiontype == ActionType.Move)
            return " moved to ";

        if (actiontype == ActionType.Attack)
            return " attacked ";

        if (actiontype == ActionType.ActiveAbility)
        {
            switch (character.CharacterType)
            {
                case CharacterType.CaptainChar:
                    return " used Take Control on ";
                case CharacterType.TankChar:
                    return " used Block on ";
                case CharacterType.ShooterChar:
                    return " used Powershot on ";
                case CharacterType.RunnerChar:
                    return " used Jump on ";
                case CharacterType.MechanicChar:
                    return " used Change Floor on ";
                case CharacterType.DocChar:
                    return " used Heal on ";
            }
        }

        return "";
    }

    private string TranslateCharacterName(Character character)
    {
        if (character != null)
        {
            return character.PrettyName + " ";
        }
        return "";
    }

    private string TranslatePlayerSide(PlayerType playerType)
    {
        return playerType == PlayerType.blue ? "Blue" : "Pink";
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnFinishAction -= WriteMovesToString;
        GameplayEvents.OnPlayerTurnAborted -= WriteAbortTurnToString;
    }
}
