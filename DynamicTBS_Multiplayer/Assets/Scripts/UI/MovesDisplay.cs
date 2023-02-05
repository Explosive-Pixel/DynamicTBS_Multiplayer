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
        string newLine = "";

        if (actionMetadata.ExecutedActionType == ActionType.Skip)
        {
            newLine = "";
        }
        else
        {
            newLine = TranslateCharacterName(actionMetadata.CharacterInAction)
            + "on "
            + TranslateTilePosition(actionMetadata.CharacterInitialPosition)
            + TranslateActionType(actionMetadata.ExecutedActionType, actionMetadata.CharacterInAction)
            + TranslateTilePosition(actionMetadata.ActionDestinationPosition)
            + "\n";
        }
        
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
        string text = "";
        actionCount += 1;



        return text;
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
    }
    #endregion

    #region EventsRegion
    private void SubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart += ActivateMovesDisplay;
        GameplayEvents.OnGameplayPhaseStart += ActivateRecordingSubscription;
        GameplayEvents.OnRestartGame += ActivateMovesDisplay;
        GameplayEvents.OnGameOver += EmptyList;
        GameplayEvents.OnGameOver += DeactivateMovesDisplay;
    }

    private void UnsubscribeEvents()
    {
        GameplayEvents.OnGameplayPhaseStart -= ActivateMovesDisplay;
        GameplayEvents.OnGameplayPhaseStart -= ActivateRecordingSubscription;
        GameplayEvents.OnRestartGame -= ActivateMovesDisplay;
        GameplayEvents.OnGameOver -= DeactivateMovesDisplay;
        GameplayEvents.OnGameOver -= EmptyList;
        GameplayEvents.OnFinishAction -= WriteMovesToString;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }
    #endregion
}