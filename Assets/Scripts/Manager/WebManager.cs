using Data.ChartData;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
namespace Manager
{
    public class WebManager : MonoBehaviourSingleton<WebManager>
    {
        public static ChartData ChartData
        {
            get => AssetManager.Instance.chartData;
            set
            {
                AssetManager.Instance.chartData = value;
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
            //ChartData = JsonConvert.DeserializeObject<ChartData>(new StreamReader(new FileStream($"{Application.streamingAssetsPath}/-1/ChartFile/Red/Chart.json", FileMode.Open)).ReadToEnd());

            ChartData = GlobalData.Instance.chartData;

            yield return GlobalData.Instance.ReadResource();
            MusicClip = GlobalData.Instance.clip;
            Background.sprite = GlobalData.Instance.currentCp;
            LoadChartData();
        }
        public void RefreshChartData()=>LoadChartData();
        public void LoadChartData()
        {
            ChartData.globalData.musicLength = GlobalData.Instance.chartEditData.musicLength <= 1 ? MusicClip.length + GlobalData.Instance.chartEditData.offset : GlobalData.Instance.chartEditData.musicLength;
            GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
        }
    }
}
