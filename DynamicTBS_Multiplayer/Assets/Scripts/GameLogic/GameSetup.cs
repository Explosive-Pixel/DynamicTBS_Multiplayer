using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static string Name { get; private set; } = "Player";
    public static PlayerType Side { get; private set; } = PlayerType.none;
    public static TimerSetup TimerSetup { get; private set; }
    public static MapSetup MapSetup { get; private set; }

    public static bool SetupCompleted { get { return TimerSetup != null && MapSetup != null; } }

    public static void SetupName(string name)
    {
        Name = name;
    }

    public static void SetupSide(PlayerType side)
    {
        Side = side;
    }

    public static void SetupTimer(TimerSetup timerSetup)
    {
        TimerSetup = timerSetup;
    }

    public static void SetupMap(MapSetup mapSetup)
    {
        MapSetup = mapSetup;
    }

    public static void ResetTimer()
    {
        TimerSetup = null;
    }

    private void Reset()
    {
        Name = "Player";
        Side = PlayerType.none;
        TimerSetup = null;
        MapSetup = null;
    }
}
