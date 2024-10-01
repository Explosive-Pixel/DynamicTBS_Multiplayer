using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SetupMapHandler : MonoBehaviour, ISetupHandler
{
    private bool mapSelected = false;

    public bool SetupCompleted { get { return mapSelected; } }

    private void Awake()
    {
        MapClass[] maps = gameObject.GetComponentsInChildren<MapClass>(true);
        for (int i = 0; i < maps.Length; i++)
        {
            MapClass map = maps[i];
            Button button = map.GetComponent<Button>();
            button.onClick.AddListener(() => ChooseBoardDesign(button, map.mapType));
        }
    }

    private void ChooseBoardDesign(Button button, MapType mapType)
    {
        AudioEvents.PressingButton();

        GameSetup.SetupMap(new MapSetup(mapType));
        mapSelected = true;

        gameObject.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
    }
}
