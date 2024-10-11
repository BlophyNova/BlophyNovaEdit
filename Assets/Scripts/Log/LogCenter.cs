using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Singleton;

public class LogCenter : MonoBehaviourSingleton<LogCenter>
{
    protected override void OnAwake()
    {
        base.OnAwake();
        DontDestroyOnLoad(gameObject);
    }

    public static void Log(string logContent)
    {
        Debug.Log($"User: {logContent}");
    }
}
