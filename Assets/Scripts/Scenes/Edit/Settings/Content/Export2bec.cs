using Data.Enumerate;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.ChartTool;
using static SimpleFileBrowser.FileBrowser;
using static System.IO.Directory;
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
            if (!Exists(path.text))
            {
                CreateDirectory(path.text);
            }

            string savePath = new Uri($"{path.text}/{GlobalData.Instance.currentChartIndex}.bec").LocalPath;

            string musicFullPath = GetMusicFullPath();
            string illustrationFullPath = GetIllustrationFullPath();

            byte[] musicBinary = File.ReadAllBytes(musicFullPath);
            byte[] illustrationBinary = File.ReadAllBytes(illustrationFullPath);


            Hard hard = Hard.Easy;
            string chartFullPath= new Uri($"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{hard}/Chart.json").LocalPath;
            string metaDataFullPath = new Uri($"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{hard}/MetaData.json").LocalPath;
            Data.ChartEdit.ChartData editData = JsonConvert.DeserializeObject<Data.ChartEdit.ChartData>(File.ReadAllText(chartFullPath,Encoding.UTF8));
            Data.ChartData.MetaData metaData= JsonConvert.DeserializeObject<Data.ChartData.MetaData>(File.ReadAllText(metaDataFullPath, Encoding.UTF8));

            Dictionary<string, byte[]> filesToZip = new()
            {
                [$"Illustration/Background{Path.GetExtension(illustrationFullPath)}"] = illustrationBinary,
                [$"Music/Music{Path.GetExtension(musicFullPath)}"] = musicBinary,
                ["d/document.txt"] = Encoding.UTF8.GetBytes("这是文本文档的内容"),
                ["data.csv"] = Encoding.UTF8.GetBytes("ID,Name\n1,张三\n2,李四"),
                ["image.png"] = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } // PNG 文件头
            };
            byte[] zipFileBinary = CreateZip(filesToZip);
            File.WriteAllBytes(savePath, zipFileBinary);


            stateText.text = $"已导出到:{savePath}";
            path.interactable = true;
            pathBrowser.interactable = true;
            export.interactable = true;
        }
        static string GetIllustrationFullPath()
        {
            string illustrationPath = $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/Illustration";
            string[] illustrations = GetFiles(illustrationPath);
            string illustrationFullPath = string.Empty;
            for (int i = 0; i < illustrations.Length; i++)
            {
                illustrationFullPath = Path.GetExtension(illustrations[i]).ToLower() switch
                {
                    ".jpg" => $"{illustrations[i]}",
                    ".png" => $"{illustrations[i]}",
                    _ => string.Empty
                };
                if (illustrationFullPath != string.Empty)
                {
                    break;
                }
            }

            return illustrationFullPath;
        }
        private static string GetMusicFullPath()
        {
            string musicPath = $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/Music";
            string[] musics = GetFiles(musicPath);
            string musicFullPath = string.Empty;
            for (int i = 0; i < musics.Length; i++)
            {
                musicFullPath = Path.GetExtension(musics[i]).ToLower() switch
                {
                    ".mp3" => $"{musics[i]}",
                    ".ogg" => $"{musics[i]}",
                    ".wav" => $"{musics[i]}",
                    _ => string.Empty
                };
                if (musicFullPath != string.Empty)
                {
                    break;
                }
            }

            return musicFullPath;
        }

        // 从内存创建 ZIP
        public static byte[] CreateZip(Dictionary<string, byte[]> files)
        {
            // 创建内存流作为 ZIP 容器
            using MemoryStream memoryStream = new();

            // 创建 ZIP 归档
            using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (KeyValuePair<string, byte[]> file in files)
                {
                    // 在 ZIP 中创建条目（文件）
                    ZipArchiveEntry entry = archive.CreateEntry(file.Key);

                    // 写入文件内容
                    using Stream entryStream = entry.Open();
                    entryStream.Write(file.Value, 0, file.Value.Length);
                    
                }
            } // 这里会自动关闭 ZipArchive，确保所有条目写入完成

            // 返回 ZIP 文件的字节数组
            return memoryStream.ToArray();
        }


        // 从内存读取 ZIP
        void ReadZipFromMemory(byte[] zipData)
        {
            using var memoryStream = new MemoryStream(zipData);
            using var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read);
            foreach (var entry in archive.Entries)
            {
                using var reader = new StreamReader(entry.Open());
                Console.WriteLine($"{entry.Name}: {reader.ReadToEnd()}");
            }
        }

        void pathBrowser_onClick()
        {
            ShowLoadDialog(paths => path.text=paths[0], () => { },PickMode.Folders
#if UNITY_ANDROID && !UNITY_EDITOR
                ,initialPath:"/storage/emulated/0"
#endif
            );
        }
        
    }
}
