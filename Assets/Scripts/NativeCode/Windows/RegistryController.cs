using Antlr4.Runtime.Misc;
using Hook;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class RegistryController : MonoBehaviour
{
    private void Start()
    {
        if (Application.platform != RuntimePlatform.WindowsPlayer)
        {
            return;
        }
        //SetupFileAssociation(true);
        //Register();
    }

    // 文件关联配置
    private const string FileExtension = ".bec";
    private const string FileTypeName = "BlophyNovaEdit_BECFile";
    private const string FileDescription = "Blophy谱面文件";
    private const string RegistryKeyName = "BlophyNovaEdit_FileAssociation";

    [DllImport("shell32.dll")]
    private static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

    // 检查并创建文件关联
    public void SetupFileAssociation(bool setAsDefault)
    {
        if (!IsAdministrator())
        {
            // 请求管理员权限重新启动应用
            RestartAsAdministrator();
            return;
        }

        try
        {
            CreateFileAssociation(setAsDefault);
            Debug.Log("文件关联创建成功！");

            // 刷新系统图标缓存
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);

            // 保存关联状态
            PlayerPrefs.SetInt(RegistryKeyName, 1);
            PlayerPrefs.Save();
        }
        catch (Exception ex)
        {
            Debug.LogError($"文件关联失败: {ex.Message}");
        }
    }
    // 创建文件关联
    private void CreateFileAssociation(bool setAsDefault)
    {
        string appPath = Process.GetCurrentProcess().MainModule.FileName;

        // 1. 创建文件扩展名关联
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(FileExtension))
        {
            key.SetValue("", setAsDefault ? FileTypeName : "");
        }

        // 2. 创建文件类型描述
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(FileTypeName))
        {
            key.SetValue("", FileDescription);
        }

        // 3. 设置图标
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"{FileTypeName}\\DefaultIcon"))
        {
            key.SetValue("", $"\"{appPath}\",0"); // 使用程序自带的第一个图标
        }

        // 4. 设置打开命令
        using (RegistryKey key = Registry.ClassesRoot.CreateSubKey($"{FileTypeName}\\shell\\open\\command"))
        {
            key.SetValue("", $"\"{appPath}\" \"%1\""); // 传递文件路径参数
        }
    }

    // 处理命令行参数（在游戏启动时调用）
    public void ProcessCommandLineArguments()
    {
        string[] args = Environment.GetCommandLineArgs();


        // 处理文件打开请求
        foreach (string arg in args)
        {
            if (arg.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
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

    // 检查关联是否已存在
    public bool IsAssociationSet()
    {
        try
        {
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(FileExtension))
            {
                return key != null;
            }
        }
        catch
        {
            return false;
        }
    }

    // 删除文件关联
    public void RemoveFileAssociation()
    {
        if (!IsAdministrator())
        {
            RestartAsAdministrator();
            return;
        }

        try
        {
            // 删除注册表项
            Registry.ClassesRoot.DeleteSubKeyTree(FileExtension, false);
            Registry.ClassesRoot.DeleteSubKeyTree(FileTypeName, false);

            Debug.Log("文件关联已移除");

            // 刷新系统图标缓存
            SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);

            // 清除关联状态
            PlayerPrefs.DeleteKey(RegistryKeyName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"移除关联失败: {ex.Message}");
        }
    }



    #region

    /// <summary>
    /// 获取路径
    /// </summary>
    /// <returns></returns>
    public string S_GetPath()
    {
        string path = "";
        if (path.Length < 3)
        {
            string[] sss = System.Reflection.Assembly.GetExecutingAssembly().Location.Split('\\');
            string ss = "";
            for (int i = 0; i < sss.Length - 3; i++)
            {
                ss += sss[i] + "\\";
            }
            path = ss;
        }
        return path;
    }
    private void Register()
    {
        if (!FileTypeRegister.FileTypeRegistered(".bec"))
        {
            return;
        }
        if (!IsAdministrator())
        {
            // 请求管理员权限重新启动应用
            RestartAsAdministrator();
            return;
        }
        FileTypeRegInfo info = new();
        info.ExtendName = ".bec";
        info.Description = "Blophy谱面文件";
        info.IcoPath = new Uri($"{Applicationm.streamingAssetsPath}/bec.ico").LocalPath;
        info.ExePath = new Uri($"{S_GetPath()}/BlophyNovaEdit.exe").LocalPath;
        Debug.Log($"IcoPath:{info.IcoPath}");
        Debug.Log($"ExePath:{info.ExePath}");
        FileTypeRegister.RegisterFileType(info);
    }
    private bool IsAdministrator()
    {
        try
        {
            // 尝试写入受保护区域来检测权限
            using RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
            return true;
        }
        catch
        {
            return false;
        }
    }
    // 以管理员权限重新启动应用
    private void RestartAsAdministrator()
    {
        try
        {
            string currentExePath = Process.GetCurrentProcess().MainModule.FileName;
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = currentExePath,
                UseShellExecute = true,
                Verb = "runas", // 请求管理员权限
                Arguments = "-setupFileAssociation" // 添加特殊参数
            };

            Process.Start(startInfo);
            System.Threading.Thread.Sleep(1000);
            
            Application.Quit(); // 关闭当前实例
        }
        catch (Exception ex)
        {
            Debug.LogError($"提权失败: {ex.Message}");
        }
    }
    #endregion
}
