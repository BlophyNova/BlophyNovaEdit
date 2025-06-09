using UnityEngine;
using UtilityCode.Singleton;

namespace Log
{
    public class LogCenter : MonoBehaviourSingleton<LogCenter>
    {
        public static string Log(string logContent, string role = "User")
        {
            Debug.Log($"{role}: {logContent}");
            return logContent;
        }
    }
}