using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode; // 适用于macOS的命名空间
using System.IO;

public class PostProcessBuild_AddFileAssociation
{
    [PostProcessBuild(100)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.StandaloneOSX)
        {
            string plistPath = path + "/Contents/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            // 添加文件类型关联
            PlistElementArray bundleDocTypes = rootDict.CreateArray("CFBundleDocumentTypes");
            PlistElementDict fileTypeDict = bundleDocTypes.AddDict();
            
            fileTypeDict.SetString("CFBundleTypeName", "BEC File");
            fileTypeDict.SetString("CFBundleTypeRole", "Viewer"); // 或 "Editor"
            fileTypeDict.SetBoolean("LSTypeIsPackage", false);
            fileTypeDict.SetString("LSHandlerRank", "Owner");
            
            PlistElementArray extensions = fileTypeDict.CreateArray("CFBundleTypeExtensions");
            extensions.AddString("bec");
            
            // 可选：设置图标
            // fileTypeDict.SetString("CFBundleTypeIconFile", "bec_icon.icns");

            plist.WriteToFile(plistPath);
        }
    }
}