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

    private void WriteMovesToString(Action action)
    {
        string newLine = GetMoveCountString() + ": ";

        if (action.PlayerActionType != null)
        {
            switch (action.PlayerActionType)
            {
                case PlayerActionType.Skip:
                    newLine += TranslatePlayerSide(action.ExecutingPlayer) + " ended their turn";
                    break;
                case PlayerActionType.Refresh:
                    newLine += TranslatePlayerSide(action.ExecutingPlayer) + " refreshed their active abilities";
                    break;
            }
        }
        else if (action.ActionSteps != null)
        {
            int i = 0;
            while (i < action.ActionSteps.Count)
            {
                ActionStep actionStep = action.ActionSteps[i];
                newLine += TranslateCharacterName(actionStep.CharacterInAction) +
                    "on " +
                    TranslateTilePosition(actionStep.CharacterInitialPosition) +
                    TranslateActionType(actionStep.ActionType, actionStep.CharacterInAction) +
                    TranslateTilePosition(actionStep.ActionDestinationPosition);

                while ((i + 1) < action.ActionSteps.Count && action.ActionSteps[i + 1].ActionType == actionStep.ActionType && action.ActionSteps[i + 1].CharacterInAction == actionStep.CharacterInAction)
                {
                    newLine += " and " + TranslateTilePosition(action.ActionSteps[i + 1].ActionDestinationPosition);
                    i++;
                }

                i++;

                if (i < action.ActionSteps.Count)
                    newLine += " and ";
            }
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
            return " used " + character.ActiveAbility.AbilityType.Description() + " on ";
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
