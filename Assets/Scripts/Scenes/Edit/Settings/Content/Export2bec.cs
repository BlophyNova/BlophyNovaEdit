using Data.Enumerate;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Data.ChartData;
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

            GetChartData(Hard.Easy,out Data.ChartEdit.ChartData easyChartEdit, out ChartData easyChartData, out MetaData easyMetaData);
            GetChartData(Hard.Normal,out Data.ChartEdit.ChartData normalChartEdit, out ChartData normalChartData, out MetaData normalMetaData);
            GetChartData(Hard.Hard,out Data.ChartEdit.ChartData hardChartEdit, out ChartData hardChartData, out MetaData hardMetaData);
            GetChartData(Hard.Ultra,out Data.ChartEdit.ChartData ultraChartEdit, out ChartData ultraChartData, out MetaData ultraMetaData);
            GetChartData(Hard.Special,out Data.ChartEdit.ChartData specialChartEdit, out ChartData specialChartData, out MetaData specialMetaData);

            Dictionary<string, byte[]> filesToZip = new()
            {
                [$"Illustration/Background{Path.GetExtension(illustrationFullPath)}"] = illustrationBinary,
                [$"Music/Music{Path.GetExtension(musicFullPath)}"] = musicBinary,
                ["ChartFile/Intuition/ChartEdit.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(easyChartEdit)),
                ["ChartFile/Intuition/Chart.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(easyChartData)),
                ["ChartFile/Intuition/MetaData.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(easyMetaData)),
                
                ["ChartFile/Anatomy/ChartEdit.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(normalChartEdit)),
                ["ChartFile/Anatomy/Chart.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(normalChartData)),
                ["ChartFile/Anatomy/MetaData.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(normalMetaData)),
                
                ["ChartFile/Reason/ChartEdit.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hardChartEdit)),
                ["ChartFile/Reason/Chart.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hardChartData)),
                ["ChartFile/Reason/MetaData.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(hardMetaData)),
                
                ["ChartFile/Schizoid/ChartEdit.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ultraChartEdit)),
                ["ChartFile/Schizoid/Chart.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ultraChartData)),
                ["ChartFile/Schizoid/MetaData.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ultraMetaData)),
                
                ["ChartFile/Special/ChartEdit.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(specialChartEdit)),
                ["ChartFile/Special/Chart.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(specialChartData)),
                ["ChartFile/Special/MetaData.json"] = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(specialMetaData)),
            };
            byte[] zipFileBinary = CreateZip(filesToZip);
            File.WriteAllBytes(savePath, zipFileBinary);


            stateText.text = $"已导出到:{savePath}";
            path.interactable = true;
            pathBrowser.interactable = true;
            export.interactable = true;
        }

        private static void GetChartData(Hard hard, out Data.ChartEdit.ChartData chartEdit, out ChartData chartData,
            out MetaData metaData)
        {
            string chartFullPath =
                new Uri(
                        $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{hard}/Chart.json")
                    .LocalPath;
            string metaDataFullPath =
                new Uri(
                        $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{hard}/MetaData.json")
                    .LocalPath;
            chartEdit = JsonConvert.DeserializeObject<Data.ChartEdit.ChartData>(File.ReadAllText(chartFullPath,
                Encoding.UTF8));
            chartData = new()
            {
                boxes = ChartTool.ConvertChartEdit2ChartData(chartEdit.boxes)
            };
            metaData = JsonConvert.DeserializeObject<MetaData>(File.ReadAllText(metaDataFullPath, Encoding.UTF8));
            #region 重置音符数量
            metaData.noteCount = 0;
            metaData.trueNoteCount = 0;
            metaData.trueTapCount = 0;
            metaData.trueHoldCount = 0;
            metaData.trueDragCount = 0;
            metaData.trueFlickCount = 0;
            metaData.trueFullFlickCount = 0;
            metaData.truePointCount = 0;
            metaData.fakeNoteCount = 0;
            metaData.fakeTapCount = 0;
            metaData.fakeHoldCount = 0;
            metaData.fakeDragCount = 0;
            metaData.fakeFlickCount = 0;
            metaData.fakeFullFlickCount = 0;
            metaData.fakePointCount = 0;
            #endregion

            foreach (Box box in chartData.boxes)
            {
                foreach (Line line in box.lines)
                {
                    foreach (Note note in line.onlineNotes)
                    {
                        metaData.noteCount++;
                        if (!note.isFakeNote)//是真音符
                        {
                            metaData.trueNoteCount++;
                            switch (note.noteType)
                            {
                                case NoteType.Tap:
                                    metaData.trueTapCount++;
                                    break;
                                case NoteType.Hold:
                                    metaData.trueHoldCount++;
                                    break;
                                case NoteType.Drag:
                                    metaData.trueDragCount++;
                                    break;
                                case NoteType.Flick:
                                    metaData.trueFlickCount++;
                                    break;
                                case NoteType.FullFlick:
                                case NoteType.FullFlickBlue:
                                case NoteType.FullFlickPink:
                                    metaData.trueFullFlickCount++;
                                    break;
                                case NoteType.Point:
                                    metaData.truePointCount++;
                                    break;
                                default:
                                    throw new Exception("找不到音符类型");
                            }
                        }
                        else//是假音符
                        {
                            metaData.fakeNoteCount++;
                            switch (note.noteType)
                            {
                                case NoteType.Tap:
                                    metaData.fakeTapCount++;
                                    break;
                                case NoteType.Hold:
                                    metaData.fakeHoldCount++;
                                    break;
                                case NoteType.Drag:
                                    metaData.fakeDragCount++;
                                    break;
                                case NoteType.Flick:
                                    metaData.fakeFlickCount++;
                                    break;
                                case NoteType.FullFlick:
                                case NoteType.FullFlickBlue:
                                case NoteType.FullFlickPink:
                                    metaData.fakeFullFlickCount++;
                                    break;
                                case NoteType.Point:
                                    metaData.fakePointCount++;
                                    break;
                                default:
                                    throw new Exception("找不到音符类型");
                            }
                        }
                    }
                }
            }
            metaData.musicLength = GlobalData.Instance.clip.length;
            List<Data.ChartData.Note> notes = new();
            foreach (var box in chartData.boxes)
            {
                foreach (var line in box.lines)
                {
                    foreach (Data.ChartData.Note note in line.onlineNotes)
                    {
                        notes.Add(note);
                    }
                }
            }
            BubbleSort(notes);
            for (int i = 1; i < notes.Count; i++)
            {
                if (Mathf.Abs(notes[i].hitTime - notes[i - 1].hitTime) < .01f)
                {
                    notes[i].hasOther = true;
                    notes[i - 1].hasOther = true;
                }
            }
        }
        /// <summary>  
        /// 整型数组的冒泡排序  
        /// </summary>  
        /// <param name="arr"></param>  
        public static void BubbleSort(List<Data.ChartData.Note> notes)
        {
            for (int i = 0; i < notes.Count - 1; i++)
            {
                for (int j = 0; j < notes.Count - 1 - i; j++)
                {
                    if (notes[j].hitTime < notes[j + 1].hitTime)
                    {
                        (notes[j + 1], notes[j]) = (notes[j], notes[j + 1]);
                    }
                }
            }
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
