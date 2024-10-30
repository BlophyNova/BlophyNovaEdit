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

    public static string Log(string logContent,string role="User")
    {
        Debug.Log($"{role}: {logContent}");
        return logContent;
    }
}
