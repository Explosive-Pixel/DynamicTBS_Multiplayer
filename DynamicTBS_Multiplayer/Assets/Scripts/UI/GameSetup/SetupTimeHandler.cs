using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class SetupTimeHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private TMP_InputField draftAndPlacementTime;
    [SerializeField] private TMP_InputField gameplayTime;

    public bool SetupCompleted { get { return IsValidTime(draftAndPlacementTime.text) && IsValidTime(gameplayTime.text); } }

    private void Awake()
    {
        if (GameSetup.TimerSetup != null)
        {
            draftAndPlacementTime.text = GameSetup.TimerSetup.DraftAndPlacementTimeFormatted;
            gameplayTime.text = GameSetup.TimerSetup.GameplayTimeFormatted;
        }

        draftAndPlacementTime.onValueChanged.AddListener(delegate { SetTime(); });
        gameplayTime.onValueChanged.AddListener(delegate { SetTime(); });

        SetTime();
    }

    private void SetTime()
    {
        GameSetup.ResetTimer();
        if (SetupCompleted)
        {
            GameSetup.SetupTimer(new TimerSetup(ConvertTimeToSeconds(draftAndPlacementTime.text), ConvertTimeToSeconds(gameplayTime.text)));
        }
    }

    private bool IsValidTime(string input)
    {
        string pattern = @"^(0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$";
        return Regex.IsMatch(input, pattern);
    }

    private int ConvertTimeToSeconds(string timeInput)
    {
        if (!IsValidTime(timeInput))
            return -1;

        TimeSpan timeSpan = TimeSpan.ParseExact(timeInput, "mm\\:ss", null);
        return (int)timeSpan.TotalSeconds;
    }
}
