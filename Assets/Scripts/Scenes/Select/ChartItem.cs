using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ChartItem : PublicButton
{
    public TMP_Text musicName;
    public ChartFileIndex thisChartFileIndex;
    private void Start()
    {
        thisButton.onClick.AddListener(() => 
        {
            GlobalData.Instance.chartEditData = JsonConvert.DeserializeObject<Data.ChartEdit.ChartData>(File.ReadAllText($"{Application.streamingAssetsPath}/{thisChartFileIndex.index}/ChartFile/{GlobalData.Instance.currentHard}/Chart.json"));
            GlobalData.Instance.currentChartIndex = thisChartFileIndex.index;
        });
    }
}
