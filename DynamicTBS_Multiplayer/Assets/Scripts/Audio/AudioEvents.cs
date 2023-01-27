using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioEvents
{
    public delegate void InMainMenu();
    public static event InMainMenu OnMainMenuEnter;

    public static void EnteringMainMenu()
    {
        if (OnMainMenuEnter != null)
            OnMainMenuEnter();
    }
}