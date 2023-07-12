using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameSetupHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeSetup;
    [SerializeField] private GameObject mapSetup;

    private bool mapSelected = false;
    private bool timeSelected = false;
    public bool AllSelected { get { return mapSelected && timeSelected; } }

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

        Timer.InitTime(timerOptions.timerSetup);
        timeSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        timeSelected = true;
    }

    public void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        BoardNew.selectedMapType = mapType;
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
