using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Data.ChartData;
using Data.ChartEdit;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.ChartTool;
using ChartData = Data.ChartEdit.ChartData;
using File = System.IO.File;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

namespace Scenes.Select
{
    public class CreateChart : PublicButton
    {
        //public GameObject alertContent;
        public TMP_InputField musicNameText;
        public TMP_InputField musicPathText;
        public TMP_InputField illustrationPathText;
        public TMP_InputField musicWriterText;
        public TMP_InputField illustrationWriterText;
        public TMP_InputField chartWriterText;
        public TMP_InputField chartLevelText;
        public TMP_InputField descriptionText;
        public Image image;
        public Transform parentObject;
        public Color illustrationPreviewNone;


        public string currentChartFileIndex = string.Empty;
        private string ChartFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/ChartFile";
        private string MusicFilePath => $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Music";

        private string IllustrationFilePath =>
            $"{Application.streamingAssetsPath}/{currentChartFileIndex}/Illustration";

        private void Start()
        {
            thisButton.onClick.AddListener(() => OnClick());
        }

        private void OnEnable()
        {
            musicNameText.text = string.Empty;
            musicPathText.text = string.Empty;
            illustrationPathText.text = string.Empty;
            musicWriterText.text = string.Empty;
            illustrationWriterText.text = string.Empty;
            chartWriterText.text = string.Empty;
            chartLevelText.text = string.Empty;
            descriptionText.text = string.Empty;
            image.sprite = null;
            image.color = illustrationPreviewNone;
            thisButton.interactable = true;
        }

        private void CreatCharts()
        {
            File.Copy(musicPathText.text, $"{MusicFilePath}/Music{Path.GetExtension(musicPathText.text)}");
            File.Copy(illustrationPathText.text,
                $"{IllustrationFilePath}/Background{Path.GetExtension(illustrationPathText.text)}");
            ChartData chartData = new();
            ChartTool.CreateNewChart(chartData, GlobalData.Instance.easeDatas);
            chartData.yScale = 6;
            chartData.beatSubdivision = 4;
            chartData.verticalSubdivision = 10;
            chartData.eventVerticalSubdivision = 10;
            chartData.playSpeed = 1;
            chartData.offset = 0;
            chartData.loopPlayBack = true;
            chartData.musicLength = -1;
            chartData.bpmList = new List<BPM>
            {
                new() { integer = 0, molecule = 0, denominator = 1, currentBPM = 60 }
            };
            chartData.customCurves = new List<CustomCurve>();

            CreatChart(Data.Enumerate.Hard.Easy, chartData);
            CreatChart(Data.Enumerate.Hard.Normal, chartData);
            CreatChart(Data.Enumerate.Hard.Hard, chartData);
            CreatChart(Data.Enumerate.Hard.Ultra, chartData);
            CreatChart(Data.Enumerate.Hard.Special, chartData);
        }

        private void CreatChart(Data.Enumerate.Hard hard, ChartData chartData)
        {
            File.WriteAllText($"{ChartFilePath}/{hard}/Chart.json",
                JsonConvert.SerializeObject(chartData),Encoding.UTF8);
            MetaData metaData = new()
            {
                musicName = musicNameText.text,
                hard = hard,
                musicWriter = musicWriterText.text,
                artWriter = illustrationWriterText.text,
                chartWriter = chartWriterText.text,
                chartLevel = chartLevelText.text,
                description = descriptionText.text,
                chartVersion = 0
            };
            File.WriteAllText($"{ChartFilePath}/{hard}/MetaData.json",
                JsonConvert.SerializeObject(metaData),Encoding.UTF8);
        }

        private bool VerifyLocalMusicExistence()
        {
            if (File.Exists(musicPathText.text))
            {
                return true;
            }

            Alert.EnableAlert("您填写的音乐文件不存在！");
            return false;
        }

        private bool VerifyLocalIllustrationExistence()
        {
            if (File.Exists(illustrationPathText.text))
            {
                return true;
            }

            Alert.EnableAlert("您填写的曲绘文件不存在！");
            return false;
        }

        public void OnClick()
        {
            thisButton.interactable = false;
            if (VerifyLocalMusicExistence() &
                VerifyLocalIllustrationExistence())
            {
                currentChartFileIndex =
                    $"{DateTime.Now.Year}{DateTime.Now.Month:D2}{DateTime.Now.Day:D2}{DateTime.Now.Hour:D2}{DateTime.Now.Minute:D2}{DateTime.Now.Second:D2}";

                CreateDirectory();
                CreatCharts();

                parentObject.gameObject.SetActive(false);
                ChartList.Instance.RefreshList();
                return;
            }

            thisButton.interactable = true;
        }

        private void CreateDirectory()
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
}