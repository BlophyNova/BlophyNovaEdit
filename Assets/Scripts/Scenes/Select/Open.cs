using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Hook;
using Scenes.PublicScripts;
using SimpleFileBrowser;
using UnityEngine;
using UtilityCode.TimeUtility;
using static SimpleFileBrowser.FileBrowser;
namespace Scenes.Select
{
    public class Open : PublicButton
    {
        public string path;
        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                #region 唤出文件选择框
                ShowLoadDialog(paths =>
                    {
                        path = paths[0];
                        ReadBecContent();
                    }, () => { },PickMode.Files
#if UNITY_ANDROID && !UNITY_EDITOR
                ,initialPath:"/storage/emulated/0"
#endif
                );
                #endregion

                
            });
        }

        void ReadBecContent()
        {
            /*
            string[] paths = {
                //"Illustration/Background",
                //"Music/Music",
                "ChartFile/Intuition/ChartEdit.json",
                "ChartFile/Intuition/MetaData.json",
                    
                "ChartFile/Anatomy/ChartEdit.json",
                "ChartFile/Anatomy/MetaData.json",
                    
                "ChartFile/Reason/ChartEdit.json",
                "ChartFile/Reason/MetaData.json",
                    
                "ChartFile/Schizoid/ChartEdit.json",
                "ChartFile/Schizoid/MetaData.json",
                    
                "ChartFile/Special/ChartEdit.json",
                "ChartFile/Special/MetaData.json",
            };
            List<byte[]> content= ReadZipFromMemory(File.ReadAllBytes(path), paths);
            */
            byte[] zipBytes = File.ReadAllBytes(path);
            using MemoryStream memoryStream = new(zipBytes);
            using ZipArchive zipArchive = new(memoryStream, ZipArchiveMode.Update);

            zipArchive.GetEntry("ChartFile/Intuition/Chart.json")!.Delete();
            zipArchive.GetEntry("ChartFile/Anatomy/Chart.json")!.Delete();
            zipArchive.GetEntry("ChartFile/Reason/Chart.json")!.Delete();
            zipArchive.GetEntry("ChartFile/Schizoid/Chart.json")!.Delete();
            zipArchive.GetEntry("ChartFile/Special/Chart.json")!.Delete();
            
            RenameFile(zipArchive, "ChartFile/Intuition/ChartEdit.json", "ChartFile/Easy/Chart.json");
            RenameFile(zipArchive, "ChartFile/Intuition/MetaData.json", "ChartFile/Easy/MetaData.json");
            
            RenameFile(zipArchive, "ChartFile/Anatomy/ChartEdit.json", "ChartFile/Normal/Chart.json");
            RenameFile(zipArchive, "ChartFile/Anatomy/MetaData.json", "ChartFile/Normal/MetaData.json");
            
            RenameFile(zipArchive, "ChartFile/Reason/ChartEdit.json", "ChartFile/Hard/Chart.json");
            RenameFile(zipArchive, "ChartFile/Reason/MetaData.json", "ChartFile/Hard/MetaData.json");
            
            RenameFile(zipArchive, "ChartFile/Schizoid/ChartEdit.json", "ChartFile/Ultra/Chart.json");
            RenameFile(zipArchive, "ChartFile/Schizoid/MetaData.json", "ChartFile/Ultra/MetaData.json");
            
            RenameFile(zipArchive, "ChartFile/Special/ChartEdit.json", "ChartFile/Special/Chart.json");
            RenameFile(zipArchive, "ChartFile/Special/MetaData.json", "ChartFile/Special/MetaData.json");
            
            string extractPath = $"{Applicationm.streamingAssetsPath}/{TimeUtility.GetCurrentTime()}";
            Directory.CreateDirectory(extractPath);
            zipArchive.ExtractToDirectory(extractPath);
            ChartList.Instance.RefreshList();
        }

        private static void RenameFile(ZipArchive zipArchive, string oldFile, string newFile)
        {
            ZipArchiveEntry entry = zipArchive.GetEntry(oldFile);
            if (entry != null)
            {
                ZipArchiveEntry newEntry = zipArchive.CreateEntry(newFile);
                using (Stream src = entry.Open())
                using (Stream dst = newEntry.Open())
                    src.CopyTo(dst);
                entry.Delete();
            }
        }

        // 从内存读取 ZIP
        List<byte[]> ReadZipFromMemory(byte[] zipData,string[] paths)
        {
            
            using MemoryStream memoryStream = new (zipData);
            using ZipArchive archive = new (memoryStream, ZipArchiveMode.Read);
            List<byte[]> result = new();
            foreach (string path in paths)
            {
                using BinaryReader streamReader = new(archive.GetEntry(path)!.Open(),Encoding.UTF8);
                byte[] bytes= streamReader.ReadBytes(int.MaxValue);
                result.Add(bytes);
            }

            return result;
        }
    }
}
