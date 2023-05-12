using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TimerUtils
{
    public static DateTime Timestamp()
    {
        return DateTime.UtcNow;
    }

    public static float TimeSince(DateTime startTimestamp)
    {
        TimeSpan timePassed = Timestamp() - startTimestamp;
        return (float)timePassed.TotalSeconds;
    }
}
