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
            int index = i;
            button.onClick.AddListener(() => ChooseBoardDesign(button, index));
        }
    }

    public void ChooseBoardDesign(Button button, int index)
    {
        Board.boardDesignIndex = index;
        mapSetup.GetComponentsInChildren<Button>().ToList().ForEach(button => button.interactable = true);
        button.interactable = false;
        mapSelected = true;

        UIEvents.MapSelected();
    }

    public void SetActive(bool active)
    {
        timeSetup.SetActive(active);
        mapSetup.SetActive(active);
    }
}
