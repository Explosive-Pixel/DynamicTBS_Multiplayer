using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

public class CSVDataOrganizer : MonoBehaviour
{
    private string path = "";
    TextAsset balancingStats = Resources.Load<TextAsset>("Spreadsheet");

    private void Awake()
    {
        path = Application.dataPath + "/Resources/GameRecords/Spreadsheet.csv";
    }

    // Activated via event on fight phase start
    private void WriteNewGameData(List<Character> unitList)
    {
        // Read out the desired (!) values in the string.
        StreamReader streamReader = new StreamReader(path);
        // Read out blue game count, since blue game count, pink game count and master game count are always the same.
        string data = streamReader.ReadLine(); // How do I tell it which line to read?

        // Convert data into int.
        int gameCountInput = int.Parse(data);

        // Add 1 and then convert data back to string.
        string gameCountOutput = (gameCountInput + 1).ToString();

        // Write new string value into all 3 correct cells.
        TextWriter tw = new StreamWriter(path, true);
        tw.WriteLine("," + gameCountOutput + "/n" + "," + gameCountOutput + "/n" + "," + gameCountOutput);
        tw.Close();

        // Iterates over the units list to extract the following data:
        // Add 1 to the "Games" row of the correct unit designation for every participating unit.
        // Add 1 to the correct quantity table cell for the correct count of every unit type in this game.
    }

    // Activated via event on game over
    private void WriteGameOverData(Player winningSide)
    {
        // Read out necessary data from the file.
        // Add 1 to "Wins" of winningSide and 1 to "Losses" of the other side.
        // Add 1 to "Wins" of winningSide units and 1 to "Losses" of the units of the other side.
    }

    // TODO: Create structure for online play, so all games record to the same file.
    // TODO: How to record the specific team combination of units and check if it's unique?
    // TODO: What other data could be useful to record?
}