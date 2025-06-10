public class FileTypeRegInfo
{
    public FileTypeRegInfo() { }
    public FileTypeRegInfo(string extendName)
    {
        this.ExtendName = extendName;
    }
    /// <summary>
    /// 自定义文件扩展名
    /// </summary>
    public string ExtendName;  //".vedu"
    /// <summary>
    ///自定义文件说明
    /// </summary>
    public string Description; //"Vivision编辑器生成的文件"
    /// <summary>
    /// 自定义文件关联的图标
    /// </summary>
    public string IcoPath;
    /// <summary>
    /// 关联自定义文件的应用程序
    /// </summary>
    public string ExePath;
}
