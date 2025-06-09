using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Data.ChartData;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine.UI;
using UtilityCode.Singleton;

namespace Scenes.Select
{
    public class ChartList : MonoBehaviourSingleton<ChartList>
    {
        public ChartItem chartItemPrefab;
        public List<ChartItem> chartItems;
        public Image illustrationPreview;
        public TMP_Text chartInformation;

        private async void Start()
        {
            await UniTask.WaitUntil(() => GlobalData.Instance.isInited);
            RefreshList();
        }

        public void RefreshList()
        {
            foreach (ChartItem item in chartItems)
            {
                Destroy(item.gameObject);
            }

            chartItems.Clear();
            string[] chartPaths = Directory.GetDirectories($"{Applicationm.streamingAssetsPath}");
            foreach (string chartPath in chartPaths)
            {
                string chartJsonPath = $"{chartPath}/ChartFile/{GlobalData.Instance.currentHard}/MetaData.json";
                if (!File.Exists(chartJsonPath))
                {
                    continue;
                }

                string rawData = File.ReadAllText(new Uri(chartJsonPath).LocalPath, Encoding.UTF8);
                ChartItem newChartItem = Instantiate(chartItemPrefab, transform);
                newChartItem.metaData = JsonConvert.DeserializeObject<MetaData>(rawData);
                newChartItem.musicName.text = newChartItem.metaData.musicName;
                newChartItem.currentChartIndex = Path.GetFileName(chartPath);
                newChartItem.illustrationPreview = illustrationPreview;
                newChartItem.chartInfomation = chartInformation;
                chartItems.Add(newChartItem);
            }
        }
    }
}