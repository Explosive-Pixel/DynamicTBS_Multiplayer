using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatsRecorder : MonoBehaviour
{
    private int gameNumber;
    private int blueWins;

    private const string SAVE_SEPARATOR = "§§";

    private string tankShorthand = "TA";
    private string shooterShorthand = "SH";
    private string runnerShorthand = "RU";
    private string mechanicShorthand = "MC";
    private string medicShorthand = "MD";
    
    private void Awake()
    {
        SubscribeEvents();
    }

    // Load old save stats from file and store in variables.
    private void LoadSaveStats()
    {
        string saveString = File.ReadAllText(Application.dataPath + "/Resources/GameRecords/SaveStats.txt");
        string[] contents = saveString.Split(new[] { SAVE_SEPARATOR }, System.StringSplitOptions.None);
        int gameNumber = int.Parse(contents[0]);
        int blueWins = int.Parse(contents[1]);
        SetValues(gameNumber, blueWins);
    }
    
    private void SetValues(int gameNumber, int blueWins)
    {
        this.gameNumber = gameNumber;
        this.blueWins = blueWins;
    }

    private void IncreaseGameNumber()
    {
        gameNumber += 1;
    }

    private void IncreaseBlueWins(PlayerType winningPlayer)
    {
        if (winningPlayer == PlayerType.blue)
            blueWins += 1;
        SaveNewStats();
    }

    private void SaveNewStats()
    {
        string[] contents = new string[]
        {
            "" + gameNumber,
            "" + blueWins
        };

        string saveString = string.Join(SAVE_SEPARATOR, contents);
        
        File.WriteAllText(Application.dataPath + "/Resources/GameRecords/SaveStats.txt", saveString);
    }

    private void IncreaseUnitInDraftCount(List<Character> draftList)
    {
        // Count how many of each character type there are in the draft
        int tankCount = 0;
        int shooterCount = 0;
        int runnerCount = 0;
        int mechanicCount = 0;
        int medicCount = 0;

        foreach (Character character in draftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                tankCount += 1;
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                shooterCount += 1;
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                runnerCount += 1;
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                mechanicCount += 1;
            if (character.GetCharacterType() == CharacterType.MedicChar)
                medicCount += 1;
        }

        // Add this to other times there were this many characters of a certain type in a draft
        // Store this information in the file
    }

    private void IncreaseUniqueDraftCombinationCount(List<Character> draftList)
    {
        // Translates the draft list to a string of shorthands for characters
        List<string> shortHandList = new List<string>();

        foreach (Character character in draftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                shortHandList.Add(tankShorthand);
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                shortHandList.Add(shooterShorthand);
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                shortHandList.Add(runnerShorthand);
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                shortHandList.Add(mechanicShorthand);
            if (character.GetCharacterType() == CharacterType.MedicChar)
                shortHandList.Add(medicShorthand);
        }

        if (shortHandList.Count == 7)
        {
            string draftString = shortHandList[0] + ":" + shortHandList[1] + ":" + shortHandList[2] + ":" + shortHandList[3] + ":" + shortHandList[4] + ":" + shortHandList[5] + ":" + shortHandList[6];
        }

        // Check if this draft string is already recorded
        // If yes, increase its count by 1
        // If no, add it and set its count to 1
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameManager.OnStartRecording += LoadSaveStats;
        GameManager.OnStartRecording += IncreaseGameNumber;
        DraftEvents.OnDeliverCharacterList += IncreaseUnitInDraftCount;
        DraftEvents.OnDeliverCharacterList += IncreaseUniqueDraftCombinationCount;
        GameplayEvents.OnGameOver += IncreaseBlueWins;
    }

    private void UnsubscribeEvents()
    {
        GameManager.OnStartRecording -= LoadSaveStats;
        GameManager.OnStartRecording -= IncreaseGameNumber;
        DraftEvents.OnDeliverCharacterList -= IncreaseUnitInDraftCount;
        DraftEvents.OnDeliverCharacterList -= IncreaseUniqueDraftCombinationCount;
        GameplayEvents.OnGameOver -= IncreaseBlueWins;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}