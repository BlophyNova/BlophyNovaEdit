using Data.ArchiveData;
using Scenes.DontDestoryOnLoad;
using TMPro;
using UtilityCode.Singleton;
namespace Scenes.SelectMusic
{
    public class UIManager : MonoBehaviourSingleton<UIManager>
    {
        public TextMeshProUGUI userName;
        public TextMeshProUGUI musicName;
        public TextMeshProUGUI musicWriter;
        public TextMeshProUGUI chartWriter;
        public TextMeshProUGUI artWriter;
        public TextMeshProUGUI bestScore;
        public void SelectMusic(string musicName, string musicWriter, string chartWriter, string artWriter)
        {
            this.musicName.text = musicName;
            this.musicWriter.text = musicWriter;
            this.chartWriter.text = chartWriter;
            this.artWriter.text = artWriter;
            //最高分，从存档系统获取
            bestScore.text = $"{ArchiveData.Instance.archive.chapterArchives[GlobalData.Instance.currentChapterIndex].musicArchive[GlobalData.Instance.currentMusicIndex][GlobalData.Instance.currentHard]:D7}";
        }
    }
}
