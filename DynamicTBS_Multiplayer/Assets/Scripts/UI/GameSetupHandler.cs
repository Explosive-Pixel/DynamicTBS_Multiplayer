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
    public bool AllSelected { get { return mapSelected; } }

    private void Awake()
    {
        Button[] maps = mapSetup.GetComponentsInChildren<Button>();
        for (int i = 0; i < maps.Length; i++)
        {
            Button button = maps[i];
            MapType mapType = button.GetComponent<MapClass>().mapType;
            button.onClick.AddListener(() => ChooseBoardDesign(button, mapType));
        }
    }

    public void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        Board.selectedMap = mapType;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        mapSelected = true;
    }

    public void SetActive(bool active)
    {
        // timeSetup.SetActive(active);
        mapSetup.SetActive(active);
    }

    public void ResetCanvas()
    {
        mapSelected = false;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
    }
}
