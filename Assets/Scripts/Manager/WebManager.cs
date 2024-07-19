using Data.ChartData;
using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;
using GlobalData = Scenes.DontDestoryOnLoad.GlobalData;
namespace Manager
{
    public class WebManager : MonoBehaviourSingleton<WebManager>
    {
        public static ChartData ChartData
        {
            //get => AssetManager.Instance.chartData;
            set
            {
                AssetManager.Instance.chartData = value;
                GlobalData.Instance.score.Reset();
                GlobalData.Instance.score.tapCount = value.globalData.tapCount;
                GlobalData.Instance.score.holdCount = value.globalData.holdCount;
                GlobalData.Instance.score.dragCount = value.globalData.dragCount;
                GlobalData.Instance.score.flickCount = value.globalData.flickCount;
                GlobalData.Instance.score.fullFlickCount = value.globalData.fullFlickCount;
                GlobalData.Instance.score.pointCount = value.globalData.pointCount;
                UIManager.Instance.musicName.text = value.metaData.musicName;
                UIManager.Instance.level.text = value.metaData.chartLevel;
                TextManager.Instance.Init(value.texts);
            }

        }
        public static AudioClip MusicClip
        {
            [UsedImplicitly] get => AssetManager.Instance.musicPlayer.clip;
            set => AssetManager.Instance.musicPlayer.clip = value;
        }
        public static Image Background
        {
            get => AssetManager.Instance.background;
            [UsedImplicitly] set => AssetManager.Instance.background = value; // 这里不能注释掉 不然后期没办法改背景了
        }
        private IEnumerator Start()
        {

            yield return GlobalData.Instance.ReadResource();
            ChartData = GlobalData.Instance.chartData;
            MusicClip = GlobalData.Instance.clip;
            Background.sprite = GlobalData.Instance.currentCp;
        }
    }
}
