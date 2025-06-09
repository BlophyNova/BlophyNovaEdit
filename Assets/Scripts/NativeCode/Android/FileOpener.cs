using System;
using UnityEngine;
using UtilityCode.Singleton;

namespace NativeCode.Android
{
    public class FileOpener : MonoBehaviourSingleton<FileOpener>
    {
        void Start()
        {
            Debug.Log("FileOpener.Start()");
            GetFilePathFromIntent();
        }

        public void GetFilePathFromIntent()
        {
            if (Application.platform!=RuntimePlatform.Android)
            {
                return;
            }

            try
            {
                // 获取当前 Android Activity
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                // 获取 Intent 和 Action
                AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
                string action = intent.Call<string>("getAction");

                Debug.Log($"action:{action}");

                // 检查是否是文件打开请求
                if (action != "android.intent.action.VIEW")
                {
                    return;
                }

                // 获取 URI
                AndroidJavaObject uri = intent.Call<AndroidJavaObject>("getData");
                if (uri == null)
                {
                    return;
                }

                // 获取文件路径字符串
                string filePath = GetFilePathFromURI(currentActivity, uri);

                // 在这里您可以使用 filePath 字符串
                OnFilePathReceived(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error getting file path: " + e.Message);
            }
        }

        // 核心方法：从 URI 获取文件路径字符串
        private string GetFilePathFromURI(AndroidJavaObject context, AndroidJavaObject uri)
        {
            string uriString = uri.Call<string>("toString");
        
            // 处理 file:// 开头的 URI
            if (uriString.StartsWith("file://"))
            {
                return uriString.Replace("file://", "");
            }
        
            // 处理 content:// 开头的 URI
            if (uriString.StartsWith("content://"))
            {
                try
                {
                    // 使用 ContentResolver 查询真实路径
                    AndroidJavaObject contentResolver = context.Call<AndroidJavaObject>("getContentResolver");
                    string[] projection = { "_data" };
                
                    AndroidJavaObject cursor = contentResolver.Call<AndroidJavaObject>("query", 
                        uri, projection, null, null, null);
                
                    if (cursor != null && cursor.Call<bool>("moveToFirst"))
                    {
                        int columnIndex = cursor.Call<int>("getColumnIndexOrThrow", "_data");
                        string path = cursor.Call<string>("getString", columnIndex);
                        cursor.Call("close");
                    
                        if (!string.IsNullOrEmpty(path))
                        {
                            return path;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Error querying content resolver: " + e.Message);
                }
            
                // 如果无法直接获取路径，返回原始 URI
                return uriString;
            }
        
            // 其他类型的 URI 直接返回
            return uriString;
        }
    
        // 处理接收到的文件路径
        private void OnFilePathReceived(string filePath)
        {
            // 在这里添加您自己的文件处理逻辑
            Debug.Log("Processing file at path: " + filePath);
        
            // 示例：您可以直接使用 System.IO 读取文件
            // string fileContent = System.IO.File.ReadAllText(filePath);
        }
    }
}