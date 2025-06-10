using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UtilityCode.Singleton;
using Debug = UnityEngine.Debug;
namespace NativeCode.Windows
{
    public class FileOpener : MonoBehaviourSingleton<FileOpener>
    {
        void Start()
        {
            Debug.Log("Windows:FileOpener.Start()");
            if (Application.platform != RuntimePlatform.WindowsPlayer)
            {
                return;
            }
            ProcessCommandLineArguments();
        }
        // 处理命令行参数（在游戏启动时调用）
        public void ProcessCommandLineArguments()
        {
            string[] args = Environment.GetCommandLineArgs();

            // 处理文件打开请求
            foreach (string arg in args)
            {
                if (arg.EndsWith(".bec", StringComparison.OrdinalIgnoreCase))
                {
                    OpenBecFile(arg);
                    return;
                }
            }
        }

        // 打开.bec文件
        private void OpenBecFile(string filePath)
        {
            Debug.Log($"打开文件: {filePath}");

            try
            {
                // 在这里实现文件处理逻辑
                // 例如：string content = File.ReadAllText(filePath);
                // 或者根据文件内容加载游戏数据
            }
            catch (Exception ex)
            {
                Debug.LogError($"文件打开失败: {ex.Message}");
            }
        }
    }
}
