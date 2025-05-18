using System.Collections.Generic;
using System.IO;
using Data.ChartData;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.Singleton;

namespace Scenes.Select
{
    public class ChartList : MonoBehaviourSingleton<ChartList>
    {
        public ChartItem chartItemPrefab;
        public List<ChartItem> chartItems;
        public Image illustrationPreview;
        public TMP_Text chartInfomation;

        private void Start()
        {
            RefreshList();
        }

        public void RefreshList()
        {
            foreach (ChartItem item in chartItems)
            {
                Destroy(item.gameObject);
            }

            chartItems.Clear();
            string[] chartPaths = Directory.GetDirectories($"{Application.streamingAssetsPath}");
            foreach (string chartPath in chartPaths)
            {
                string chartJsonPath = $"{chartPath}/ChartFile/{GlobalData.Instance.currentHard}/MetaData.json";
                if (!File.Exists(chartJsonPath))
                {
                    continue;
                }


                string rawData = File.ReadAllText(chartJsonPath);
                ChartItem newChartItem = Instantiate(chartItemPrefab, transform);
                newChartItem.metaData = JsonConvert.DeserializeObject<MetaData>(rawData);
                newChartItem.musicName.text = newChartItem.metaData.musicName;
                newChartItem.currentChartIndex = Path.GetFileName(chartPath);
                newChartItem.illustrationPreview = illustrationPreview;
                newChartItem.chartInfomation = chartInfomation;
                chartItems.Add(newChartItem);
            }
        }
    }
}