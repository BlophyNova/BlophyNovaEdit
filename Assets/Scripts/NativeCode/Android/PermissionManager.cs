using System;
using UnityEngine;
using UtilityCode.Singleton;

namespace NativeCode.Android
{
    public class PermissionManager : MonoBehaviourSingleton<PermissionManager>
    {
        private AndroidJavaClass unityPlayer;

        private AndroidJavaObject currentActivity;
        private AndroidJavaClass permissionManager;
        
        // Start is called before the first frame update
        private void Start()
        {
            if (Application.platform != RuntimePlatform.Android)
            {
                return;
            }

            try
            {
                unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
                // 调用修改后的 Java 方法
                permissionManager = new AndroidJavaClass("NativeCode.Android.Java.PermissionManager");
                permissionManager.CallStatic("GetStoragePermission", currentActivity);
            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
    }
}