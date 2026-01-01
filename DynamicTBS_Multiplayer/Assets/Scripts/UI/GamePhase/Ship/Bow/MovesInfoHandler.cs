using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public class MovesInfoHandler : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshPro displayText;
    [SerializeField] private Color blue;
    [SerializeField] private Color pink;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString endTurnText;
    [SerializeField] private LocalizedString refreshAAText;
    [SerializeField] private LocalizedString turnAbortedText;
    [SerializeField] private LocalizedString turnAbortedReasonNoAvailableActionText;
    [SerializeField] private LocalizedString turnAbortedReasonTimeoutText;
    [SerializeField] private LocalizedString andText;
    [SerializeField] private LocalizedString onText;
    [SerializeField] private LocalizedString actionMoveText;
    [SerializeField] private LocalizedString actionAttackText;
    [SerializeField] private LocalizedString actionActiveAAText;

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

        if (action.ActionSteps == null || action.ActionSteps.Count == 0)
        {
            switch (action.PlayerActionType)
            {
                case PlayerActionType.Skip:
                    newLine += Format(endTurnText, ("player", TranslatePlayerSide(action.ExecutingPlayer)));
                    break;

                case PlayerActionType.Refresh:
                    newLine += Format(refreshAAText, ("player", TranslatePlayerSide(action.ExecutingPlayer)));
                    break;
            }
        }
        else
        {
            int i = 0;
            while (i < action.ActionSteps.Count)
            {
                ActionStep actionStep = action.ActionSteps[i];

                string actionDestinationsText = TranslateTilePosition(actionStep.ActionDestinationPosition);
                while ((i + 1) < action.ActionSteps.Count && action.ActionSteps[i + 1].ActionType == actionStep.ActionType && action.ActionSteps[i + 1].CharacterInAction == actionStep.CharacterInAction)
                {
                    actionDestinationsText += " " + GetLocalized(andText) + " " + TranslateTilePosition(action.ActionSteps[i + 1].ActionDestinationPosition);
                    i++;
                }

                newLine += TranslateCharacterName(actionStep.CharacterInAction) +
                    GetLocalized(onText) + " " +
                    TranslateTilePosition(actionStep.CharacterInitialPosition) + " " +
                    TranslateActionType(actionStep.ActionType, actionStep.CharacterInAction, actionDestinationsText);

                i++;

                if (i < action.ActionSteps.Count)
                    newLine += " " + GetLocalized(andText) + " ";
            }
        }

        newLine += "\n";

        DisplayMoves(newLine);
    }

    private void WriteAbortTurnToString(PlayerType abortedTurnPlayer, int remainingActions, AbortTurnCondition abortTurnCondition)
    {
        string newLine = Format(turnAbortedText,
            ("player", TranslatePlayerSide(abortedTurnPlayer).Trim()),
            ("reason", abortTurnCondition == AbortTurnCondition.NO_AVAILABLE_ACTION ? GetLocalized(turnAbortedReasonNoAvailableActionText) : GetLocalized(turnAbortedReasonTimeoutText))
            );

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
            displayText.text += "<color=#" + (movesList[i].StartsWith("B") ? ColorUtility.ToHtmlStringRGB(blue) : ColorUtility.ToHtmlStringRGB(pink)) + ">" + movesList[i] + "</color>";
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

    private string TranslateActionType(ActionType actiontype, Character character, string position)
    {
        if (actiontype == ActionType.Move)
            return Format(actionMoveText, ("position", position));

        if (actiontype == ActionType.Attack)
            return Format(actionAttackText, ("position", position));

        if (actiontype == ActionType.ActiveAbility)
        {
            return Format(actionActiveAAText, ("activeAbilityType", character.ActiveAbility.AbilityType.LocalizedDescription()), ("position", position));
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
        return PlayerSetup.GetSideName(playerType);
    }

    private void OnDestroy()
    {
        GameEvents.OnGamePhaseStart -= SetActive;
        GameplayEvents.OnFinishAction -= WriteMovesToString;
        GameplayEvents.OnPlayerTurnAborted -= WriteAbortTurnToString;
    }

    // -----------------------------------------------------------
    //  Localized Helpers
    // -----------------------------------------------------------

    private string GetLocalized(LocalizedString s)
    {
        if (s == null) return "";
        var handle = s.GetLocalizedStringAsync();
        handle.WaitForCompletion(); // synchron warten (kurzes Blockieren möglich)
        return handle.Result ?? "";
    }

    private string Format(LocalizedString s, params (string key, object value)[] args)
    {
        if (s == null) return "";

        // Baue ein anonymes Objekt mit den named placeholders
        // SmartFormat in Localization akzeptiert ein Objekt mit Properties -> wir erzeugen ein Dictionary,
        // das wir als einzelnes Objekt übergeben.
        var dict = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;
        foreach (var (key, value) in args)
            dict[key] = value;

        var handle = s.GetLocalizedStringAsync(dict);
        handle.WaitForCompletion();
        return handle.Result ?? "";
    }
}
