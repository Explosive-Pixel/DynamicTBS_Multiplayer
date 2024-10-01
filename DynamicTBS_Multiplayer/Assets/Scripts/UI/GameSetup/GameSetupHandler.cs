using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSetupHandler : MonoBehaviour
{
    [SerializeField] private TMP_InputField draftAndPlacementTime;
    [SerializeField] private TMP_InputField gameplayTime;

    [SerializeField] private GameObject mapSetup;

    private bool mapSelected = false;
    public bool Completed { get { return mapSelected && IsValidTime(draftAndPlacementTime.text) && IsValidTime(gameplayTime.text); } }

    private void Awake()
    {
        MapClass[] maps = mapSetup.GetComponentsInChildren<MapClass>();
        for (int i = 0; i < maps.Length; i++)
        {
            MapClass map = maps[i];
            Button button = map.GetComponent<Button>();
            button.onClick.AddListener(() => ChooseBoardDesign(button, map.mapType));
        }

        draftAndPlacementTime.onValueChanged.AddListener(delegate { SetDraftAndPlacementTime(); });
        gameplayTime.onValueChanged.AddListener(delegate { SetGameplayTime(); });
    }

    private void SetDraftAndPlacementTime()
    {
        Debug.Log("Draft and Placement time changed");
        if (IsValidTime(draftAndPlacementTime.text))
        {
            Debug.Log("Valid input!");
            TimerConfig.DraftAndPlacementTime = ConvertTimeToSeconds(draftAndPlacementTime.text);
            Debug.Log("Number of seconds: " + TimerConfig.DraftAndPlacementTime);
        }
    }

    private void SetGameplayTime()
    {
        if (IsValidTime(gameplayTime.text))
        {
            TimerConfig.GameplayTime = ConvertTimeToSeconds(gameplayTime.text);
        }
    }

    private void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        Board.selectedMapType = mapType;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        mapSelected = true;
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
