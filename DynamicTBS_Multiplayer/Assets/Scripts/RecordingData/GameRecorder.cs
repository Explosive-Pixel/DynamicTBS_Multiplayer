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
        path = "Assets/Resources/GameRecords/" + filename + ".txt";
    }

    private void RecordMove(Character character, ActionType actionType, Vector3 characterInitialPosition, Vector3? actionDestinationPosition)
    {
        string recordLine = "Player: " + character.GetSide().GetPlayerType().ToString() + "\nCharacter: " + character.ToString() + "\nPerformed action: " + actionType.ToString() + "\nOriginal position: " + TranslateTilePosition(characterInitialPosition) + "\nTarget position: " + TranslateTilePosition(actionDestinationPosition) + "\n";

        if (path != null)
        {
            StreamWriter writer = new StreamWriter(path, true);
            writer.WriteLine(recordLine);
            writer.Close();
        }
        
        Debug.Log(recordLine);
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

    private void RecordWinner(PlayerType winningSide)
    {
        string recordLine = "Player " + winningSide.ToString() + " won.";

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
        GameManager.OnStartRecording += SetPath;
        DraftEvents.OnCharacterCreated += RecordDraft;
        GameplayEvents.OnFinishAction += RecordMove;
        GameplayEvents.OnGameOver += RecordWinner;
    }

    private void UnsubscribeEvents()
    {
        GameManager.OnStartRecording -= SetPath;
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