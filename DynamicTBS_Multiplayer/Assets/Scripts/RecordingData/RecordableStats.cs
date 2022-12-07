using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecordableStats
{
    public int gameNumber;
    public int blueWins;

    public Dictionary<(int, string), int> unitsInDraftDictionary = new Dictionary<(int, string), int>();

    public RecordableStats()
    {
        gameNumber = 0;
        blueWins = 0;

        foreach (CharacterType characterType in Enum.GetValues(typeof(CharacterType)))
        {
            for (int i = 1; i <= 7; i++)
            {
                unitsInDraftDictionary.Add((i, characterType.ToString()), 0);
            }
        }
    }
}