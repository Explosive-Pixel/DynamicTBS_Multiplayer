using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TutorialMenuManager : MonoBehaviour
{
    [CanBeNull] public GameObject SelectionMenu;
    [CanBeNull] public GameObject Returnbutton;
    public List<GameObject> Menus = new List<GameObject>();

    public void ReturnToSelection()
    {
        CloseAll();
        if (SelectionMenu != null)
            SelectionMenu.SetActive(true);
        if (Returnbutton != null)
            Returnbutton.SetActive(false);
    }

    public void OpenSubMenu(GameObject subMenu)
    {
        CloseAll();
        subMenu.SetActive(true);
        if (Returnbutton != null)
            Returnbutton.SetActive(true);
    }

    private void CloseAll()
    {
        foreach (var menu in Menus)
        {
            menu.SetActive(false);
        }
    }
}
