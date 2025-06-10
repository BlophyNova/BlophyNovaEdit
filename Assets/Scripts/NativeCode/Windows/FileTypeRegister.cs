using Microsoft.Win32;

public class FileTypeRegister
{

    #region RegisterFileType 注册文件类型
    /// <summary>
    /// RegisterFileType 注册自定义文件类型，关联图标和默认打开程序。
    /// </summary>        
    public static void RegisterFileType(FileTypeRegInfo regInfo)
    {
        string relationName = regInfo.ExtendName.Substring(1, regInfo.ExtendName.Length - 1).ToUpper() + "_FileType";

        RegistryKey fileTypeKey = Registry.ClassesRoot.CreateSubKey(regInfo.ExtendName);
        fileTypeKey.SetValue("", relationName);
        fileTypeKey.Close();

        RegistryKey relationKey = Registry.ClassesRoot.CreateSubKey(relationName);
        relationKey.SetValue("", regInfo.Description);

        RegistryKey iconKey = relationKey.CreateSubKey("DefaultIcon");
        iconKey.SetValue("", regInfo.IcoPath);

        RegistryKey shellKey = relationKey.CreateSubKey("Shell");
        RegistryKey openKey = shellKey.CreateSubKey("Open");
        RegistryKey commandKey = openKey.CreateSubKey("Command");
        commandKey.SetValue("", regInfo.ExePath + " %1");

        relationKey.Close();
    }

    /// <summary>
    /// GetFileTypeRegInfo 获取自定义文件信息
    /// </summary>        
    public static FileTypeRegInfo GetFileTypeRegInfo(string extendName)
    {
        FileTypeRegInfo regInfo = new FileTypeRegInfo(extendName);

        string relationName = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
        RegistryKey relationKey = Registry.ClassesRoot.OpenSubKey(relationName);
        regInfo.Description = relationKey.GetValue("").ToString();

        RegistryKey iconKey = relationKey.OpenSubKey("DefaultIcon");
        regInfo.IcoPath = iconKey.GetValue("").ToString();

        RegistryKey shellKey = relationKey.OpenSubKey("Shell");
        RegistryKey openKey = shellKey.OpenSubKey("Open");
        RegistryKey commandKey = openKey.OpenSubKey("Command");
        string temp = commandKey.GetValue("").ToString();
        regInfo.ExePath = temp.Substring(0, temp.Length - 3);

        return regInfo;
    }

    /// <summary>
    /// UpdateFileTypeRegInfo 更新自定义文件信息
    /// </summary>    
    public static bool UpdateFileTypeRegInfo(FileTypeRegInfo regInfo)
    {
        string extendName = regInfo.ExtendName;
        string relationName = extendName.Substring(1, extendName.Length - 1).ToUpper() + "_FileType";
        RegistryKey relationKey = Registry.ClassesRoot.OpenSubKey(relationName, true);
        relationKey.SetValue("", regInfo.Description);

        RegistryKey iconKey = relationKey.OpenSubKey("DefaultIcon", true);
        iconKey.SetValue("", regInfo.IcoPath);

        RegistryKey shellKey = relationKey.OpenSubKey("Shell");
        RegistryKey openKey = shellKey.OpenSubKey("Open");
        RegistryKey commandKey = openKey.OpenSubKey("Command", true);
        commandKey.SetValue("", regInfo.ExePath + " %1");
        relationKey.Close();
        return true;
    }
    /// <summary>
    /// FileTypeRegistered  检测自定义图标是否被注册
    /// </summary>        
    public static bool FileTypeRegistered(string extendName)
    {
        RegistryKey softwareKey = Registry.ClassesRoot.OpenSubKey(extendName);
        if (softwareKey != null)
        {
            return true;
        }
        return false;
    }
    #endregion
}

