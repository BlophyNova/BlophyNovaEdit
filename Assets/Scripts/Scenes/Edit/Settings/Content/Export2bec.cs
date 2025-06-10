using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SimpleFileBrowser.FileBrowser;
namespace Scenes.Edit.Settings.Content
{
    public class Export2bec : MonoBehaviour
    {
        public TMP_InputField path;

        public Button pathBrowser;

        public Button export;

        public TextMeshProUGUI stateText;
        // Start is called before the first frame update
        void Start()
        {
            pathBrowser.onClick.AddListener(pathBrowser_onClick);
            export.onClick.AddListener(export_onClick);
        }

        private void export_onClick()
        {
            path.interactable = false;
            pathBrowser.interactable = false;
            export.interactable = false;
            stateText.text = "导出中...";
            Export();
        }

        void Export()
        {
            if (!Directory.Exists(path.text))
            {
                Directory.CreateDirectory(path.text);
            }

            string savePath = new Uri($"{path.text}/Export.bec").LocalPath;
            
        }
        
        void pathBrowser_onClick()
        {
            ShowLoadDialog(paths => path.text=paths[0], () => { },PickMode.Folders
#if UNITY_ANDROID && !UNITY_EDITOR
                ,initialPath:"/storage/emulated/0"
#endif
            );
        }
        
        public static byte[] CreateZip()
        {
            // 创建内存流作为 ZIP 容器
            using MemoryStream zipStream = new();
            // 创建 ZIP 存档（自动处理关闭和写入）
            using (ZipArchive archive = new(zipStream, ZipArchiveMode.Create, true))
            {
                // 示例 1: 添加文本文件
                ZipArchiveEntry textEntry = archive.CreateEntry("document.txt");
                using (StreamWriter writer = new(textEntry.Open()))
                {
                    writer.WriteLine("这是直接在内存中生成的文本内容");
                    writer.WriteLine($"当前时间: {DateTime.Now}");
                }

                // 示例 2: 添加二进制文件 (模拟图片)
                ZipArchiveEntry imageEntry = archive.CreateEntry("image.png");
                byte[] fakeImageData = GenerateFakeImageData();
                using (Stream entryStream = imageEntry.Open())
                {
                    entryStream.Write(fakeImageData, 0, fakeImageData.Length);
                }

                // 示例 3: 从其他内存数据添加
                byte[] csvData = Encoding.UTF8.GetBytes("ID,Name\n1,测试名称");
                ZipArchiveEntry dataEntry = archive.CreateEntry("data.csv");
                using (Stream entryStream = dataEntry.Open())
                {
                    entryStream.Write(csvData, 0, csvData.Length);
                }
            }

            // 重要：完成写入后获取 ZIP 字节数组
            return zipStream.ToArray();
        }

        // 生成模拟二进制数据 (实际使用时替换为真实数据)
        private static byte[] GenerateFakeImageData()
        {
            return new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // PNG 文件头
        }
        
    }
}
