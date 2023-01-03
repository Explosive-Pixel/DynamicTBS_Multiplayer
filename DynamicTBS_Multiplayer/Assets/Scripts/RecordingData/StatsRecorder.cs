using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StatsRecorder : MonoBehaviour
{
    private RecordableStats recordableStats;

    private string filePath;

    //private string tankShorthand = "TA";
    //private string shooterShorthand = "SH";
    //private string runnerShorthand = "RU";
    //private string mechanicShorthand = "MC";
    //private string medicShorthand = "MD";

    private string blueDraftString;
    private string pinkDraftString;
    
    private void Awake()
    {
        SubscribeEvents();
        string directory = Application.dataPath + "/Resources/GameRecords";
        Directory.CreateDirectory(directory);
        filePath = directory + "/SaveStats.json"; //TODO: Saving to a remote file online
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
        IncreaseGameNumber();
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

    private void RedordWin(PlayerType? winningPlayer)
    {
        if(winningPlayer == null)
        {
            // TODO: Record Draw
            SaveStats();
            return;
        }

        if (winningPlayer == PlayerType.blue)
        {
            // Records the blue win in general
            recordableStats.blueWins += 1;
            
            // Records the blue win for the unique draft played
        //    foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
        //    {
        //        if (uniqueDraft == blueDraftString)
        //        {
        //            int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
        //            int gamesWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

        //            gamesPlayedWithThisDraft += 1;
        //            gamesWonWithThisDraft += 1;

        //            recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gamesWonWithThisDraft);
        //        }
        //        else
        //        {
        //            recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 1));
        //        }
        //    }

        //    // Records the pink loss in the unique draft played
        //    foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
        //    {
        //        if (uniqueDraft == pinkDraftString)
        //        {
        //            int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
        //            int gameWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

        //            gamesPlayedWithThisDraft += 1;

        //            recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gameWonWithThisDraft);
        //        }
        //        else
        //        {
        //            recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 0));
        //        }
        //    }
        //}
        //else
        //{
        //    // Records the blue loss for the unique draft played
        //    foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
        //    {
        //        if (uniqueDraft == blueDraftString)
        //        {
        //            int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
        //            int gamesWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

        //            gamesPlayedWithThisDraft += 1;

        //            recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gamesWonWithThisDraft);
        //        }
        //        else
        //        {
        //            recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 0));
        //        }
        //    }

        //    // Records the pink win in the unique draft played
        //    foreach (string uniqueDraft in recordableStats.uniqueDraftsDictionary.Keys)
        //    {
        //        if (uniqueDraft == pinkDraftString)
        //        {
        //            int gamesPlayedWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item1;
        //            int gamesWonWithThisDraft = recordableStats.uniqueDraftsDictionary[uniqueDraft].Item2;

        //            gamesPlayedWithThisDraft += 1;
        //            gamesWonWithThisDraft += 1;

        //            recordableStats.uniqueDraftsDictionary[uniqueDraft] = (gamesPlayedWithThisDraft, gamesWonWithThisDraft);
        //        }
        //        else
        //        {
        //            recordableStats.uniqueDraftsDictionary.Add(blueDraftString, (1, 1));
        //        }
        //    }
        }
        
        SaveStats();
    }

    //private void IncreaseUnitInDraftCount(List<Character> draftList)
    //{
    //    // Counts how many of each character type there are in the draft of each team
    //    int blueTankCount = 0;
    //    int blueShooterCount = 0;
    //    int blueRunnerCount = 0;
    //    int blueMechanicCount = 0;
    //    int blueMedicCount = 0;

    //    int pinkTankCount = 0;
    //    int pinkShooterCount = 0;
    //    int pinkRunnerCount = 0;
    //    int pinkMechanicCount = 0;
    //    int pinkMedicCount = 0;

    //    List<Character> blueDraftList = new List<Character>();
    //    List<Character> pinkDraftList = new List<Character>();

    //    // Divides the draft list in two lists for both teams
    //    foreach (Character character in draftList)
    //    {
    //        if (character.GetSide().GetPlayerType() == PlayerType.blue)
    //        {
    //            blueDraftList.Add(character);
    //        }
    //        else
    //        {
    //            pinkDraftList.Add(character);
    //        }
    //    }

    //    // Increases the count of each character by one for each character in the list
    //    foreach (Character character in blueDraftList)
    //    {
    //        if (character.GetCharacterType() == CharacterType.TankChar)
    //            blueTankCount += 1;
    //        if (character.GetCharacterType() == CharacterType.ShooterChar)
    //            blueShooterCount += 1;
    //        if (character.GetCharacterType() == CharacterType.RunnerChar)
    //            blueRunnerCount += 1;
    //        if (character.GetCharacterType() == CharacterType.MechanicChar)
    //            blueMechanicCount += 1;
    //        if (character.GetCharacterType() == CharacterType.MedicChar)
    //            blueMedicCount += 1;
    //    }

    //    foreach (Character character in pinkDraftList)
    //    {
    //        if (character.GetCharacterType() == CharacterType.TankChar)
    //            pinkTankCount += 1;
    //        if (character.GetCharacterType() == CharacterType.ShooterChar)
    //            pinkShooterCount += 1;
    //        if (character.GetCharacterType() == CharacterType.RunnerChar)
    //            pinkRunnerCount += 1;
    //        if (character.GetCharacterType() == CharacterType.MechanicChar)
    //            pinkMechanicCount += 1;
    //        if (character.GetCharacterType() == CharacterType.MedicChar)
    //            pinkMedicCount += 1;
    //    }

    //    // Records the units in draft stats in the dictionaries in the RecordableStats class
    //    if (blueTankCount > 0)
    //        recordableStats.unitsInDraftDictionary[(blueTankCount, CharacterType.TankChar.ToString())] += 1;
    //    if (blueShooterCount > 0)
    //        recordableStats.unitsInDraftDictionary[(blueShooterCount, CharacterType.ShooterChar.ToString())] += 1;
    //    if (blueRunnerCount > 0)
    //        recordableStats.unitsInDraftDictionary[(blueRunnerCount, CharacterType.RunnerChar.ToString())] += 1;
    //    if (blueMechanicCount > 0)
    //        recordableStats.unitsInDraftDictionary[(blueMechanicCount, CharacterType.MechanicChar.ToString())] += 1;
    //    if (blueMedicCount > 0)
    //        recordableStats.unitsInDraftDictionary[(blueMedicCount, CharacterType.MedicChar.ToString())] += 1;
    //    if (pinkTankCount > 0)
    //        recordableStats.unitsInDraftDictionary[(pinkTankCount, CharacterType.TankChar.ToString())] += 1;
    //    if (pinkShooterCount > 0)
    //        recordableStats.unitsInDraftDictionary[(pinkShooterCount, CharacterType.ShooterChar.ToString())] += 1;
    //    if (pinkRunnerCount > 0)
    //        recordableStats.unitsInDraftDictionary[(pinkRunnerCount, CharacterType.RunnerChar.ToString())] += 1;
    //    if (pinkMechanicCount > 0)
    //        recordableStats.unitsInDraftDictionary[(pinkMechanicCount, CharacterType.MechanicChar.ToString())] += 1;
    //    if (pinkMedicCount > 0)
    //        recordableStats.unitsInDraftDictionary[(pinkMedicCount, CharacterType.MedicChar.ToString())] += 1;
    //}

    //private void IncreaseUniqueDraftCombinationCount(List<Character> draftList)
    //{
    //    List<string> blueShortHandList = new List<string>();
    //    List<string> pinkShortHandList = new List<string>();

    //    List<Character> blueDraftList = new List<Character>();
    //    List<Character> pinkDraftList = new List<Character>();

    //    // Divides draft list in two lists for each side
    //    foreach (Character character in draftList)
    //    {
    //        if (character.GetSide().GetPlayerType() == PlayerType.blue)
    //        {
    //            blueDraftList.Add(character);
    //        }
    //        else
    //        {
    //            pinkDraftList.Add(character);
    //        }
    //    }

    //    // Translates each side into shorthand string lists
    //    foreach (Character character in blueDraftList)
    //    {
    //        if (character.GetCharacterType() == CharacterType.TankChar)
    //            blueShortHandList.Add(tankShorthand);
    //        if (character.GetCharacterType() == CharacterType.ShooterChar)
    //            blueShortHandList.Add(shooterShorthand);
    //        if (character.GetCharacterType() == CharacterType.RunnerChar)
    //            blueShortHandList.Add(runnerShorthand);
    //        if (character.GetCharacterType() == CharacterType.MechanicChar)
    //            blueShortHandList.Add(mechanicShorthand);
    //        if (character.GetCharacterType() == CharacterType.MedicChar)
    //            blueShortHandList.Add(medicShorthand);
    //    }

    //    foreach (Character character in pinkDraftList)
    //    {
    //        if (character.GetCharacterType() == CharacterType.TankChar)
    //            pinkShortHandList.Add(tankShorthand);
    //        if (character.GetCharacterType() == CharacterType.ShooterChar)
    //            pinkShortHandList.Add(shooterShorthand);
    //        if (character.GetCharacterType() == CharacterType.RunnerChar)
    //            pinkShortHandList.Add(runnerShorthand);
    //        if (character.GetCharacterType() == CharacterType.MechanicChar)
    //            pinkShortHandList.Add(mechanicShorthand);
    //        if (character.GetCharacterType() == CharacterType.MedicChar)
    //            pinkShortHandList.Add(medicShorthand);
    //    }

    //    // Translates the shorthand lists as strings
    //    if (blueShortHandList.Count == 7)
    //    {
    //        blueDraftString = blueShortHandList[0] + ":" + blueShortHandList[1] + ":" + blueShortHandList[2] + ":" + blueShortHandList[3] + ":" + blueShortHandList[4] + ":" + blueShortHandList[5] + ":" + blueShortHandList[6];
    //        pinkDraftString = pinkShortHandList[0] + ":" + pinkShortHandList[1] + ":" + pinkShortHandList[2] + ":" + pinkShortHandList[3] + ":" + pinkShortHandList[4] + ":" + pinkShortHandList[5] + ":" + pinkShortHandList[6];
    //    }
    //}

    #region EventsRegion

    private void SubscribeEvents()
    {
        DraftEvents.OnStartDraft += LoadStats;
        //DraftEvents.OnDeliverCharacterList += IncreaseUnitInDraftCount;
        //DraftEvents.OnDeliverCharacterList += IncreaseUniqueDraftCombinationCount;
        GameplayEvents.OnGameOver += RedordWin;
    }

    private void UnsubscribeEvents()
    {
        DraftEvents.OnStartDraft -= LoadStats;
        //DraftEvents.OnDeliverCharacterList -= IncreaseUnitInDraftCount;
        //DraftEvents.OnDeliverCharacterList -= IncreaseUniqueDraftCombinationCount;
        GameplayEvents.OnGameOver -= RedordWin;
    }

    private void OnDestroy()
    {
        UnsubscribeEvents();
    }

    #endregion
}