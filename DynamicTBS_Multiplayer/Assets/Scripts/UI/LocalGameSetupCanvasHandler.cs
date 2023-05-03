using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LocalGameSetupCanvasHandler : MonoBehaviour
{
    [SerializeField] private GameObject timeSetup;
    [SerializeField] private GameObject mapSetup;

    private void Awake()
    {
        Button[] maps = mapSetup.GetComponentsInChildren<Button>();
        for(int i = 0; i < maps.Length; i++)
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
    }

    public void StartLocalGame()
    {
        GameEvents.StartGame();
    }
}
