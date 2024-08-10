using Data.ChartData;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using File = System.IO.File;
using Text = Data.ChartData.Text;

public class CreateChart : PublicButton
{
    //public GameObject alertContent;
    public TMP_InputField musicNameText;
    public TMP_InputField musicPathText;
    public TMP_InputField illustrationPathText;
    public Transform parentObject;

    string currentLevel= "Hard";
    string ChartFilePath=>$"{Application.streamingAssetsPath}/{currentChartFileIndex}/ChartFile";
    string MusicFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Music";
    string IllustrationFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Illustration";


    public int currentChartFileIndex = -1;

    private void CreatChart()
    {
        File.Copy(musicPathText.text, $"{MusicFilePath}/Music.mp3");
        File.Copy(illustrationPathText.text, $"{IllustrationFilePath}/Background.png");
        Data.ChartEdit.ChartData chartData = new();
        ChartTool.CreateNewChart(chartData, Scenes.DontDestroyOnLoad.GlobalData.Instance.easeData);
        chartData.yScale = 6;
        chartData.beatSubdivision = 4;
        chartData.verticalSubdivision = 10;
        chartData.eventVerticalSubdivision = 10;
        chartData.playSpeed = 1;
        chartData.offset = 0;
        chartData.loopPlayBack = true;
        chartData.musicLength = -1;
        chartData.bpmList = new();
        chartData.bpmList.Add(new() { integer=0,molecule=0,denominator=1,currentBPM=60});
        File.WriteAllText($"{ChartFilePath}/{currentLevel}/Chart.json",JsonConvert.SerializeObject(chartData));
    }

    private bool VerifyLocalMusicExistence()
    {
        if (File.Exists(musicPathText.text))
        {
            return true;
        }
        else
        {
            Alert.EnableAlert( "您填写的音乐文件不存在！");
            return false;
        }
    }

    private bool VerifyLocalIllustrationExistence()
    {
        if (File.Exists(illustrationPathText.text))
        {
            return true;
        }
        else
        {
            Alert.EnableAlert( "您填写的曲绘文件不存在！");
            return false;
        }
    }

    public void OnClick() 
    {
        if (VerifyLocalMusicExistence() &
            VerifyLocalIllustrationExistence())
        {
            string indexJSONPath = $"{Application.streamingAssetsPath}/index.json";
            if (!File.Exists(indexJSONPath))
            {
                List<ChartFileIndex> chartFileIndices = new();
                CreateNewChartIndex(indexJSONPath, chartFileIndices);
            }
            else
            {
                string rawData = File.ReadAllText(indexJSONPath);
                List<ChartFileIndex> chartFileIndices = JsonConvert.DeserializeObject<List<ChartFileIndex>>(rawData);
                CreateNewChartIndex(indexJSONPath, chartFileIndices);
            }

            CreatChart();

            parentObject.gameObject.SetActive(false);
            ChartList.Instance.RefreshList();
        }
    }

    private void CreateNewChartIndex(string indexJSONPath, List<ChartFileIndex> chartFileIndices)
    {
        ChartFileIndex chartFileIndex = new();
        currentChartFileIndex = chartFileIndex.index = chartFileIndices.Count;
        chartFileIndices.Add(chartFileIndex);
        chartFileIndices[currentChartFileIndex].musicName = musicNameText.text;
        chartFileIndices[currentChartFileIndex].musicPath = musicPathText.text;
        chartFileIndices[currentChartFileIndex].IllustrationPath = illustrationPathText.text;
        File.WriteAllText(indexJSONPath, JsonConvert.SerializeObject(chartFileIndices,Formatting.Indented));
        CreateDirectory();
    }

    private void Start()
    {
        thisButton.onClick.AddListener(() => OnClick());
    }
    void CreateDirectory()
    {
        Directory.CreateDirectory($"{ChartFilePath}/Easy");
        Directory.CreateDirectory($"{ChartFilePath}/Normal");
        Directory.CreateDirectory($"{ChartFilePath}/Hard");
        Directory.CreateDirectory($"{ChartFilePath}/Ultra");
        Directory.CreateDirectory($"{ChartFilePath}/Special");
        Directory.CreateDirectory($"{MusicFilePath}");
        Directory.CreateDirectory($"{IllustrationFilePath}");
    }
}
