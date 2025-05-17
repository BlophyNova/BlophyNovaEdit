using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtility
{
    public static ulong time;

    public static string GetCurrentTime()
    {
        string result =
            $"{DateTime.Now.Year}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}{DateTime.Now.Millisecond}";
        ulong currentTime = ulong.Parse(result);
        if (currentTime > time)
        {
            time = currentTime;
        }
        else
        {
            time++;
            result = $"{time}";
        }
        return result;
    }
}
