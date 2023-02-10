using UnityEngine;
using System.IO;
using System;

public class GameRecorder : MonoBehaviour
{
    // TODO: Create structure so that all game records from online games are saved to the same location.
    private string path;
    private string filename;
    
    private void Awake()
    {
       if(GameManager.IsHost())
        SubscribeEvents();
    }

    private void SetPath()
    {
        try
        {
            filename = "GameRecord_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
            string directory = Application.dataPath + "/Resources/GameRecords";
            Directory.CreateDirectory(directory);
            path = directory + "/" + filename + ".txt";
        } catch(Exception ex)
        {
            Debug.Log("Cannot record game: " + ex.ToString());
            path = null;
            UnsubscribeEvents();
        }
    }

    private void RecordMove(ActionMetadata actionMetadata)
    {
        string recordLine = "Player: " + actionMetadata.ExecutingPlayer.GetPlayerType().ToString() + "\nPerformed action: " + actionMetadata.ExecutedActionType.ToString();
        if(actionMetadata.CharacterInAction != null)
        {
            recordLine = "\nCharacter: " + actionMetadata.CharacterInAction.ToString() + "\nOriginal position: " + TranslateTilePosition(actionMetadata.CharacterInitialPosition) + "\nTarget position: " + TranslateTilePosition(actionMetadata.ActionDestinationPosition) + "\n";
        }

        RecordLine(recordLine);
    }

    private void RecordDraft(Character character)
    {
        string recordLine = "Player " + character.GetSide().GetPlayerType().ToString() + " drafted " + character.ToString();

        RecordLine(recordLine);
    }

    private void RecordWinner(PlayerType? winningSide, GameOverCondition endGameCondition)
    {
        string recordLine = winningSide != null ? "Player " + winningSide.ToString() + " won." : "No player won the match.";

        RecordLine(recordLine);
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

    private void RecordLine(String recordLine)
    {
        if (path != null)
        {
            try
            {
                StreamWriter writer = new StreamWriter(path, true);
                writer.WriteLine(recordLine);
                writer.Close();
            }
            catch (Exception ex)
            {
                Debug.Log("Cannot record game: " + ex.ToString());
                path = null;
                UnsubscribeEvents();
            }
        }
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnStartDraft += SetPath;
        DraftEvents.OnCharacterCreated += RecordDraft;
        GameplayEvents.OnFinishAction += RecordMove;
        GameplayEvents.OnGameOver += RecordWinner;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnStartDraft -= SetPath;
        DraftEvents.OnCharacterCreated -= RecordDraft;
        GameplayEvents.OnFinishAction -= RecordMove;
        GameplayEvents.OnGameOver -= RecordWinner;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}