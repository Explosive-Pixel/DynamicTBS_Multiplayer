using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class StatsRecorder : MonoBehaviour
{
    private RecordableStats recordableStats;

    private string filePath;

    private string tankShorthand = "TA";
    private string shooterShorthand = "SH";
    private string runnerShorthand = "RU";
    private string mechanicShorthand = "MC";
    private string medicShorthand = "MD";
    
    private void Awake()
    {
        SubscribeEvents();
        filePath = Application.dataPath + "/Resources/GameRecords/SaveStats.txt";
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
        // Count how many of each character type there are in the draft of each team
        int blueTankCount = 0;
        int blueShooterCount = 0;
        int blueRunnerCount = 0;
        int blueMechanicCount = 0;
        int blueMedicCount = 0;

        int pinkTankCount = 0;
        int pinkShooterCount = 0;
        int pinkRunnerCount = 0;
        int pinkMechanicCount = 0;
        int pinkMedicCount = 0;

        List<Character> blueDraftList = new List<Character>();
        List<Character> pinkDraftList = new List<Character>();

        foreach (Character character in draftList)
        {
            if (character.GetSide().GetPlayerType() == PlayerType.blue)
            {
                blueDraftList.Add(character);
            }
            else
            {
                pinkDraftList.Add(character);
            }
        }

        foreach (Character character in blueDraftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                blueTankCount += 1;
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                blueShooterCount += 1;
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                blueRunnerCount += 1;
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                blueMechanicCount += 1;
            if (character.GetCharacterType() == CharacterType.MedicChar)
                blueMedicCount += 1;
        }

        foreach (Character character in pinkDraftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                pinkTankCount += 1;
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                pinkShooterCount += 1;
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                pinkRunnerCount += 1;
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                pinkMechanicCount += 1;
            if (character.GetCharacterType() == CharacterType.MedicChar)
                pinkMedicCount += 1;
        }

        // Put in for loop after rewriting code above as dictionary -> HOW?
        if (blueTankCount > 0)
            recordableStats.unitsInDraftDictionary[(blueTankCount, CharacterType.TankChar.ToString())] += 1;
        if (blueShooterCount > 0)
            recordableStats.unitsInDraftDictionary[(blueShooterCount, CharacterType.ShooterChar.ToString())] += 1;
        if (blueRunnerCount > 0)
            recordableStats.unitsInDraftDictionary[(blueRunnerCount, CharacterType.RunnerChar.ToString())] += 1;
        if (blueMechanicCount > 0)
            recordableStats.unitsInDraftDictionary[(blueMechanicCount, CharacterType.MechanicChar.ToString())] += 1;
        if (blueMedicCount > 0)
            recordableStats.unitsInDraftDictionary[(blueMedicCount, CharacterType.MedicChar.ToString())] += 1;
        if (pinkTankCount > 0)
            recordableStats.unitsInDraftDictionary[(pinkTankCount, CharacterType.TankChar.ToString())] += 1;
        if (pinkShooterCount > 0)
            recordableStats.unitsInDraftDictionary[(pinkShooterCount, CharacterType.ShooterChar.ToString())] += 1;
        if (pinkRunnerCount > 0)
            recordableStats.unitsInDraftDictionary[(pinkRunnerCount, CharacterType.RunnerChar.ToString())] += 1;
        if (pinkMechanicCount > 0)
            recordableStats.unitsInDraftDictionary[(pinkMechanicCount, CharacterType.MechanicChar.ToString())] += 1;
        if (pinkMedicCount > 0)
            recordableStats.unitsInDraftDictionary[(pinkMedicCount, CharacterType.MedicChar.ToString())] += 1;
    }

    private void IncreaseUniqueDraftCombinationCount(List<Character> draftList)
    {
        // Translates the draft list to a string of shorthands for characters
        List<string> blueShortHandList = new List<string>();
        List<string> pinkShortHandList = new List<string>();

        List<Character> blueDraftList = new List<Character>();
        List<Character> pinkDraftList = new List<Character>();

        string blueDraftString;
        string pinkDraftString;

        foreach (Character character in draftList)
        {
            if (character.GetSide().GetPlayerType() == PlayerType.blue)
            {
                blueDraftList.Add(character);
            }
            else
            {
                pinkDraftList.Add(character);
            }
        }

        foreach (Character character in blueDraftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                blueShortHandList.Add(tankShorthand);
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                blueShortHandList.Add(shooterShorthand);
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                blueShortHandList.Add(runnerShorthand);
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                blueShortHandList.Add(mechanicShorthand);
            if (character.GetCharacterType() == CharacterType.MedicChar)
                blueShortHandList.Add(medicShorthand);
        }

        foreach (Character character in pinkDraftList)
        {
            if (character.GetCharacterType() == CharacterType.TankChar)
                pinkShortHandList.Add(tankShorthand);
            if (character.GetCharacterType() == CharacterType.ShooterChar)
                pinkShortHandList.Add(shooterShorthand);
            if (character.GetCharacterType() == CharacterType.RunnerChar)
                pinkShortHandList.Add(runnerShorthand);
            if (character.GetCharacterType() == CharacterType.MechanicChar)
                pinkShortHandList.Add(mechanicShorthand);
            if (character.GetCharacterType() == CharacterType.MedicChar)
                pinkShortHandList.Add(medicShorthand);
        }

        if (blueShortHandList.Count == 7)
        {
            blueDraftString = blueShortHandList[0] + ":" + blueShortHandList[1] + ":" + blueShortHandList[2] + ":" + blueShortHandList[3] + ":" + blueShortHandList[4] + ":" + blueShortHandList[5] + ":" + blueShortHandList[6];
            pinkDraftString = pinkShortHandList[0] + ":" + pinkShortHandList[1] + ":" + pinkShortHandList[2] + ":" + pinkShortHandList[3] + ":" + pinkShortHandList[4] + ":" + pinkShortHandList[5] + ":" + pinkShortHandList[6];

            foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
            {
                if (uniqueDraft == blueDraftString)
                {
                    int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
                    int gameWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

                    gamesPlayedWithThisDraft += 1;

                    recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gameWonWithThisDraft);
                }
                else
                {
                    recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 0));
                }
            }

            foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
            {
                if (uniqueDraft == pinkDraftString)
                {
                    int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
                    int gameWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

                    gamesPlayedWithThisDraft += 1;

                    recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gameWonWithThisDraft);
                }
                else
                {
                    recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 0));
                }
            }
        }

        // TODO: After game ends record the win stat of the draft that won
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