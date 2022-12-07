using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatsRecorder : MonoBehaviour
{
    private RecordableStats recordableStats;

    private string filePath = Application.dataPath + "/Resources/GameRecords/SaveStats.txt";
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
    private void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string loadString = File.ReadAllText(filePath);
            recordableStats = JsonUtility.FromJson<RecordableStats>(loadString);
        }
        else
        {
            recordableStats = new RecordableStats();
            File.WriteAllText(filePath, "");
        }
    }

    private void SaveStats()
    {
        string saveString = JsonUtility.ToJson(recordableStats);
        File.WriteAllText(filePath, saveString);
    }

    private void IncreaseGameNumber()
    {
        recordableStats.gameNumber += 1;
    }

    private void IncreaseBlueWins(PlayerType winningPlayer)
    {
        if (winningPlayer == PlayerType.blue)
            recordableStats.blueWins += 1;
        SaveStats();
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
        // TODO: differentiate between the two teams!

        // Put in for loop after rewriting code above as dictionary.
        recordableStats.unitsInDraftDictionary[(tankCount, CharacterType.TankChar.ToString())] += 1;

        // Add this to other times there were this many characters of a certain type in a draft
        // Store this information in the file
    }

    private void IncreaseUniqueDraftCombinationCount(List<Character> draftList)
    {
        // Translates the draft list to a string of shorthands for characters
        List<string> shortHandList = new List<string>();

        // TODO: Differentiate between pink and blue draft and record both separately
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
            string blueDraftString = shortHandList[0] + ":" + shortHandList[1] + ":" + shortHandList[2] + ":" + shortHandList[3] + ":" + shortHandList[4] + ":" + shortHandList[5] + ":" + shortHandList[6];
        }

        // Check if this draft string is already recorded
        // If yes, increase its count by 1
        // If no, add it and set its count to 1
    }

    #region EventsRegion

    private void SubscribeEvents()
    {
        GameManager.OnStartRecording += LoadStats;
        GameManager.OnStartRecording += IncreaseGameNumber;
        DraftEvents.OnDeliverCharacterList += IncreaseUnitInDraftCount;
        DraftEvents.OnDeliverCharacterList += IncreaseUniqueDraftCombinationCount;
        GameplayEvents.OnGameOver += IncreaseBlueWins;
    }

    private void UnsubscribeEvents()
    {
        GameManager.OnStartRecording -= LoadStats;
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