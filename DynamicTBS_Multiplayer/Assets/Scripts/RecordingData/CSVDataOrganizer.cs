using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class CSVDataOrganizer : MonoBehaviour
{
    private string path = "";

    private void Awake()
    {
        path = Application.dataPath + "/Resources/GameRecords/Spreadsheet.csv";
    }

    // Activated via event on new game start
    private void WriteNewGameData()
    {
        // Adds 1 to the "Game" row of "Blue", "Pink" and "Master".
    }

    // Activated via event on game over
    private void WriteGameOverData(Player winningSide)
    {
        // Add 1 to "Wins" of winningSide and 1 to "Losses" of the other side.
        // Add 1 to "Wins" of winningSide units and 1 to "Losses" of the units of the other side.
    }

    // Activated via event at the end of draft
    private void WriteDraftData(List<Character> unitList)
    {
        // At the end of the draft iterates over the units list to extract the following data:
        // Add 1 to the "Games" row of the correct unit designation for every participating unit.
        // Add 1 to the correct table cell for the correct count of every unit type in this game.
    }

    // TODO: Find out how to write additively to csv-file and to specific cell in that file.
    // TODO: Create structure for online play, so all games record to the same file.
    // TODO: What other data could be useful to record?
}