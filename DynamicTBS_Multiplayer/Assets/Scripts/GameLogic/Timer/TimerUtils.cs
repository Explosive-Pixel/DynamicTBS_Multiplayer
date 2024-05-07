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

    public static string TimestampToString(DateTime dateTime)
    {
        return dateTime.ToString("O");
    }

    public static long TimeInMilliseconds()
    {
        return DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }

    public static float TimeSince(DateTime startTimestamp)
    {
        TimeSpan timePassed = Timestamp() - startTimestamp;
        return (float)timePassed.TotalSeconds;
    }

    public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
}
