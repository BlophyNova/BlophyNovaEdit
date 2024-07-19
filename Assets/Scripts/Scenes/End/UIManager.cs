using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UtilityCode.Singleton;
using AD = Data.ArchiveData.ArchiveData;
using GD = Scenes.DontDestoryOnLoad.GlobalData;
namespace Scenes.End
{
    public class UIManager : MonoBehaviourSingleton<UIManager>
    {
        public Image background;
        public Image art;
        [FormerlySerializedAs("APFC")]
        public TextMeshProUGUI apfc;
        public TextMeshProUGUI score;
        public TextMeshProUGUI perfect;
        public TextMeshProUGUI good;
        public TextMeshProUGUI bad;
        public TextMeshProUGUI miss;
        public TextMeshProUGUI maxCombo;
        public TextMeshProUGUI accuracy;
        public TextMeshProUGUI musicName;
        public TextMeshProUGUI level;
        private void Start()
        {
            background.sprite = GD.Instance.currentCp;
            art.sprite = GD.Instance.currentCph;
            Texture2D cphTexture = GD.Instance.currentCph.texture;
            art.sprite = Sprite.Create(cphTexture, new Rect((cphTexture.width - cphTexture.height) / 2, 0, cphTexture.height, cphTexture.height), new Vector2(0.5f, 0.5f));
            if (GD.Instance.isAutoplay)
            {
                apfc.text = "Autoplay";
            }
            else if (GD.Instance.score.Bad == 0 && GD.Instance.score.Miss == 0 && GD.Instance.score.Good == 0)
            {
                apfc.text = "AllPerfect";
            }
            else if (GD.Instance.score.Bad == 0 && GD.Instance.score.Miss == 0)
            {
                apfc.text = "FullCombo";
            }
            else
            {
                apfc.text = "";
            }
            score.text = $"{(int)GD.Instance.score.Score:D7}";
            perfect.text = $"{GD.Instance.score.Perfect}";
            good.text = $"{GD.Instance.score.Good}";
            bad.text = $"{GD.Instance.score.Bad}";
            miss.text = $"{GD.Instance.score.Miss}";
            maxCombo.text = $"Max Combo: {GD.Instance.score.maxCombo}";
            accuracy.text = $"Accuracy: {GD.Instance.score.Accuracy * 100f:F2}%";
            musicName.text = $"{GD.Instance.chartData.metaData.musicName}";
            level.text = $"{GD.Instance.chartData.metaData.chartLevel}";
            if (!GD.Instance.isAutoplay)
            {
                int currentArchiveMusicHardScore = AD.Instance.archive.chapterArchives[GD.Instance.currentChapterIndex].musicArchive[GD.Instance.currentMusicIndex][GD.Instance.currentHard];
                int newMusicHardScore = (int)GD.Instance.score.Score;
                AD.Instance.archive.chapterArchives[GD.Instance.currentChapterIndex].musicArchive[GD.Instance.currentMusicIndex][GD.Instance.currentHard] = newMusicHardScore > currentArchiveMusicHardScore ? newMusicHardScore : currentArchiveMusicHardScore;
                AD.Instance.SaveArchive();
            }
        }
    }
}
