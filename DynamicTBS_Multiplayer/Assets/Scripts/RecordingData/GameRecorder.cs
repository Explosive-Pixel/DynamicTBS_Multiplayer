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
       SubscribeEvents();
    }

    private void SetPath()
    {
        filename = "GameRecord_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
        string directory = Application.dataPath + "/Resources/GameRecords";
        Directory.CreateDirectory(directory);
        path = directory + "/" + filename + ".txt"; 
    }

    private void RecordMove(ActionMetadata actionMetadata)
    {
        string recordLine = "Player: " + actionMetadata.ExecutingPlayer.GetPlayerType().ToString() + "\nPerformed action: " + actionMetadata.ExecutedActionType.ToString();
        if(actionMetadata.CharacterInAction != null)
        {
            recordLine = "\nCharacter: " + actionMetadata.CharacterInAction.ToString() + "\nOriginal position: " + TranslateTilePosition(actionMetadata.CharacterInitialPosition) + "\nTarget position: " + TranslateTilePosition(actionMetadata.ActionDestinationPosition) + "\n";
        }

        if (path != null)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(recordLine);
            writer.Close();
        }
        
        //Debug.Log(recordLine);
    }

    private void RecordDraft(Character character)
    {
        string recordLine = "Player " + character.GetSide().GetPlayerType().ToString() + " drafted " + character.ToString();

        if (path != null)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(recordLine);
            writer.Close();
        }

        Debug.Log(recordLine);
    }

    private void RecordWinner(PlayerType? winningSide)
    {
        string recordLine = winningSide != null ? "Player " + winningSide.ToString() + " won." : "No player won the match.";

        if (path != null)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(recordLine);
            writer.Close();
        }

        Debug.Log(recordLine);
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