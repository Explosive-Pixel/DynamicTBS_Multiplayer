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

    public delegate void MasterSpawn();
    public static event MasterSpawn OnSpawnMasters;

    public delegate void LowTime();
    public static event LowTime OnTimeLow;

    public delegate void Timeout();
    public static event Timeout OnTimeout;

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

    public static void SpawningMasters()
    {
        if (OnSpawnMasters != null)
            OnSpawnMasters();
    }

    public static void TimeIsLow()
    {
        if (OnTimeLow != null)
            OnTimeLow();
    }

    public static void TimeRanOut()
    {
        if (OnTimeout != null)
            OnTimeout();
    }
}