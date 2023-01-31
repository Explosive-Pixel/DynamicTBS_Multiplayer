using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AudioEvents
{
    public delegate void InMainMenu();
    public static event InMainMenu OnMainMenuEnter;

    public delegate void ButtonPress();
    public static event ButtonPress OnButtonPress;

    public delegate void Explosion();
    public static event Explosion OnExplode;

    public delegate void Adrenalin();
    public static event Adrenalin OnAdrenalin;

    public static void EnteringMainMenu()
    {
        if (OnMainMenuEnter != null)
            OnMainMenuEnter();
    }

    public static void PressingButton()
    {
        if (OnButtonPress != null)
            OnButtonPress();
    }

    public static void Exploding()
    {
        if (OnExplode != null)
            OnExplode();
    }

    public static void AdrenalinRelease()
    {
        if (OnAdrenalin != null)
            OnAdrenalin();
    }
}