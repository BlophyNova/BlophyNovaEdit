using System.Collections.Generic;
using System.IO;
using Data;
using Newtonsoft.Json;
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
            string indexJSONPath = $"{Application.streamingAssetsPath}/index.json";
            if (File.Exists(indexJSONPath))
            {
                foreach (ChartItem item in chartItems)
                {
                    Destroy(item.gameObject);
                }

                chartItems.Clear();

                string rawData = File.ReadAllText(indexJSONPath);
                List<ChartFileIndex> chartFileIndices = JsonConvert.DeserializeObject<List<ChartFileIndex>>(rawData);
                foreach (ChartFileIndex item in chartFileIndices)
                {
                    ChartItem newChartItem = Instantiate(chartItemPrefab, transform);
                    newChartItem.musicName.text = item.musicName;
                    newChartItem.thisChartFileIndex = item;
                    newChartItem.illustrationPreview = illustrationPreview;
                    newChartItem.chartInfomation = chartInfomation;
                    chartItems.Add(newChartItem);
                }
            }
        }
    }
}