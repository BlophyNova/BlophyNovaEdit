using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UtilityCode.Singleton;

public class ChartList : MonoBehaviourSingleton<ChartList>
{
    public ChartItem chartItemPrefab;
    public List<ChartItem> chartItems;
    private void Start()
    {
        RefreshList();
    }
    public void RefreshList()
    {
        string indexJSONPath = $"{Application.streamingAssetsPath}/index.json";
        if (File.Exists(indexJSONPath))
        {
            foreach (var item in chartItems)
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
                chartItems.Add(newChartItem);
            }
        }
    }
}
