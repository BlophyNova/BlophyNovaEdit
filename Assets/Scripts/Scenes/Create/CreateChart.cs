using Data.ChartData;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.Profiling;
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

    string currentLevel="Red";
    string ChartFilePath=>$"{Application.streamingAssetsPath}/{currentChartFileIndex}/ChartFile";
    string MusicFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Music";
    string IllustrationFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Illustration";


    public int currentChartFileIndex = -1;

    private void CreatChart()
    {
        File.Copy(musicPathText.text, $"{MusicFilePath}/music.mp3");
        File.Copy(illustrationPathText.text, $"{IllustrationFilePath}/bg.png");
        ChartData chartData = new();
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
            Alert.EnableAlert(parentObject, "您填写的音乐文件不存在！");
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
            Alert.EnableAlert(parentObject, "您填写的曲绘文件不存在！");
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
                List<ChartFileIndex> chartFileIndices = new()
                {
                    new() { index = 0 }
                };
                currentChartFileIndex = chartFileIndices[0].index;

                chartFileIndices[currentChartFileIndex].musicName = musicNameText.text;
                chartFileIndices[currentChartFileIndex].musicPath = musicPathText.text;
                chartFileIndices[currentChartFileIndex].IllustrationPath = illustrationPathText.text;
                File.WriteAllText(indexJSONPath, JsonConvert.SerializeObject(chartFileIndices));
                CreateDirectory();
            }
            else
            {
                string rawData = File.ReadAllText(indexJSONPath);
            }

            CreatChart();
        }
    }
    private void Start()
    {
        thisButton.onClick.AddListener(() => OnClick());
    }
    void CreateDirectory()
    {
        Directory.CreateDirectory($"{ChartFilePath}/Green");
        Directory.CreateDirectory($"{ChartFilePath}/Yellow");
        Directory.CreateDirectory($"{ChartFilePath}/Red");
        Directory.CreateDirectory($"{MusicFilePath}");
        Directory.CreateDirectory($"{IllustrationFilePath}");
    }
}
