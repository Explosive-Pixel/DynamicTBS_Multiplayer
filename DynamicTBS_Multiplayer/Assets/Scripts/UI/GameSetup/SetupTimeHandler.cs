using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SetupTimeHandler : MonoBehaviour, ISetupHandler
{
    [SerializeField] private TMP_InputField draftAndPlacementTime;
    [SerializeField] private TMP_InputField gameplayTime;

    public bool SetupCompleted { get { return IsValidTime(draftAndPlacementTime.text) && IsValidTime(gameplayTime.text); } }

    private void Awake()
    {
        draftAndPlacementTime.onValueChanged.AddListener(delegate { SetOutline(draftAndPlacementTime); });
        gameplayTime.onValueChanged.AddListener(delegate { SetOutline(gameplayTime); });

        if (GameSetup.TimerSetup != null)
        {
            draftAndPlacementTime.text = GameSetup.TimerSetup.DraftAndPlacementTimeFormatted;
            gameplayTime.text = GameSetup.TimerSetup.GameplayTimeFormatted;
        }

        draftAndPlacementTime.onValueChanged.AddListener(delegate { SetTime(); });
        gameplayTime.onValueChanged.AddListener(delegate { SetTime(); });

        SetTime();
        SetOutline(draftAndPlacementTime);
        SetOutline(gameplayTime);
    }

    private void SetTime()
    {
        GameSetup.ResetTimer();
        if (SetupCompleted)
        {
            GameSetup.SetupTimer(new TimerSetup(ConvertTimeToSeconds(draftAndPlacementTime.text), ConvertTimeToSeconds(gameplayTime.text)));
        }
    }

    private void SetOutline(TMP_InputField tMP_Input)
    {
        tMP_Input.gameObject.GetComponent<Outline>().enabled = !IsValidTime(tMP_Input.text);
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
