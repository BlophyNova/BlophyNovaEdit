using System;
using System.IO;
using Newtonsoft.Json;
using Scenes.DontDestoryOnLoad;
using UnityEngine;
using UnityEngine.Serialization;
using UtilityCode.Singleton;
namespace Data.ArchiveData
{
    public class ArchiveData : MonoBehaviourSingleton<ArchiveData>
    {
        public Archive archive = new();
        private void Start()
        {
            if (File.Exists($"{Application.persistentDataPath}/Archive.HuaWaterED"))
            {
                archive = JsonConvert.DeserializeObject<Archive>(File.ReadAllText($"{Application.persistentDataPath}/Archive.HuaWaterED"));
            }
            else
            {
                archive.chapterArchives = new ChapterArchive[GlobalData.Instance.chapters.Length];
                for (int i = 0; i < archive.chapterArchives.Length; i++)
                {
                    archive.chapterArchives[i] = new ChapterArchive
                    {
                        musicArchive = new MusicArchive[GlobalData.Instance.chapters[GlobalData.Instance.currentChapterIndex].musicPath.Length]
                    };
                    for (int j = 0; j < archive.chapterArchives[i].musicArchive.Length; j++)
                    {
                        archive.chapterArchives[i].musicArchive[j] = new();
                    }
                }
                SaveArchive();
            }
        }
        public void SaveArchive()
        {
            File.WriteAllText($"{Application.persistentDataPath}/Archive.HuaWaterED", JsonConvert.SerializeObject(archive));
        }
    }
    [Serializable]
    public class Archive
    {
        public ChapterArchive[] chapterArchives;
    }
    [Serializable]
    public class ChapterArchive
    {
        public MusicArchive[] musicArchive;
    }
    [Serializable]
    public class MusicArchive
    {
        [FormerlySerializedAs("score_Green")]
        public int scoreGreen;
        [FormerlySerializedAs("score_Yellow")]
        public int scoreYellow;
        [FormerlySerializedAs("score_Red")]
        public int scoreRed;
        public int this[string hard]
        {
            get => hard switch
            {
                "Green" => scoreGreen,
                "Yellow" => scoreYellow,
                "Red" => scoreRed,
                _ => throw new Exception("如果你看到这条消息,请截图并在群中@MojaveHao/Niubility748/HuaWaterED中的任意一位反馈\n" +
                    "存档系统读取方法出错,难度未找到")
            };
            set
            {
                switch (hard)
                {
                    case "Green":
                        scoreGreen = value;
                        break;
                    case "Yellow":
                        scoreYellow = value;
                        break;
                    case "Red":
                        scoreRed = value;
                        break;
                }
            }
        }
    }
}