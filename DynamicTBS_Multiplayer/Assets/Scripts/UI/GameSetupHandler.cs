using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.ComponentModel;

public class GameSetupHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeSetup;
    [SerializeField] private GameObject mapSetup;

    private bool mapSelected = false;
    private bool timeSelected = false;
    public bool AllSelected { get { return mapSelected && timeSelected; } }

    public enum TimerType
    {
        DRAFT_AND_PLACEMENT,
        GAMEPLAY
    }

    public enum TimerSetupType
    {
        [Description("Fast")]
        FAST = 1,
        [Description("Standard")]
        STANDARD = 2,
        [Description("Slow")]
        SLOW = 3
    }

    private static readonly Dictionary<TimerSetupType, Dictionary<TimerType, float>> timeSetups = new()
    {
        { TimerSetupType.FAST,
            new() {
                { TimerType.DRAFT_AND_PLACEMENT, 120 },
                { TimerType.GAMEPLAY, 60 }
            }
        },
        {
            TimerSetupType.STANDARD,
            new()
            {
                { TimerType.DRAFT_AND_PLACEMENT, 300 },
                { TimerType.GAMEPLAY, 90 }
            }
        },
        {
            TimerSetupType.SLOW,
            new()
            {
                { TimerType.DRAFT_AND_PLACEMENT, 420 },
                { TimerType.GAMEPLAY, 120 }
            }
        }
    };

    private void Awake()
    {
        Button[] timerOptions = timeSetup.GetComponentsInChildren<Button>();
        for (int i = 0; i < timerOptions.Length; i++)
        {
            Button button = timerOptions[i];
            TimerClass timerClass = button.GetComponent<TimerClass>();
            button.onClick.AddListener(() => ChooseTimer(button, timerClass));
        }

        Button[] maps = mapSetup.GetComponentsInChildren<Button>();
        for (int i = 0; i < maps.Length; i++)
        {
            Button button = maps[i];
            MapType mapType = button.GetComponent<MapClass>().mapType;
            button.onClick.AddListener(() => ChooseBoardDesign(button, mapType));
        }
    }

    public void ChooseTimer(Button button, TimerClass timerOptions)
    {
        AudioEvents.PressingButton();

        InitTimer(timerOptions.timerSetup);
        timeSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        timeSelected = true;
    }

    public void InitTimer(TimerSetupType timerSetupType)
    {
        var selectedTimeSetup = timeSetups[timerSetupType];
        TimerConfig.Init(selectedTimeSetup[TimerType.DRAFT_AND_PLACEMENT], selectedTimeSetup[TimerType.GAMEPLAY]);
    }

    public void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        Board.selectedMapType = mapType;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        mapSelected = true;
    }

    public void SetActive(bool active)
    {
        timeSetup.SetActive(active);
        mapSetup.SetActive(active);
    }

    public void ResetCanvas()
    {
        timeSelected = false;
        timeSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);

        mapSelected = false;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
    }
}
