using System;
using System.IO;
using UnityEngine;
using UtilityCode.Singleton;

namespace NativeCode.MacOS
{
    public class FileOpener : MonoBehaviourSingleton<FileOpener>//好像MacOS平台要关联文件需要签名，感觉好麻烦，回头再写吧，先鸽了
    {
        void Start()
        {
            Debug.Log("MacOS:FileOpener.Start()");
            if(Application.platform != RuntimePlatform.OSXPlayer)
            {
                return;
            }

            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.EndsWith(".bec", StringComparison.OrdinalIgnoreCase))
                {
                    Debug.Log("Opening .bec file: " + arg);
                    ProcessBecFile(arg);
                }
            }
        }

        private void ProcessBecFile(string filePath)
        {
            // 1. 验证文件存在
            if (!File.Exists(filePath))
            {
                Debug.LogError("File not found: " + filePath);
                return;
            }
            Debug.Log("Processing file at path: " + filePath);
        }
    }
}