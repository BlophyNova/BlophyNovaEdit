using Data;
using Data.ChartData;
using Data.ChartEdit;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using System.Collections.Generic;
using System.IO;
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


        public int currentChartFileIndex = -1;
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

        private void CreatChart()
        {
            File.Copy(musicPathText.text, $"{MusicFilePath}/Music.mp3");
            File.Copy(illustrationPathText.text, $"{IllustrationFilePath}/Background.png");
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
            chartData.bpmList = new List<BPM>();
            chartData.bpmList.Add(new BPM { integer = 0, molecule = 0, denominator = 1, currentBPM = 60 });
            File.WriteAllText($"{ChartFilePath}/{Data.Enumerate.Hard.Easy}/Chart.json",
                JsonConvert.SerializeObject(chartData));
            File.WriteAllText($"{ChartFilePath}/{Data.Enumerate.Hard.Normal}/Chart.json",
                JsonConvert.SerializeObject(chartData));
            File.WriteAllText($"{ChartFilePath}/{Data.Enumerate.Hard.Hard}/Chart.json",
                JsonConvert.SerializeObject(chartData));
            File.WriteAllText($"{ChartFilePath}/{Data.Enumerate.Hard.Ultra}/Chart.json",
                JsonConvert.SerializeObject(chartData));
            File.WriteAllText($"{ChartFilePath}/{Data.Enumerate.Hard.Special}/Chart.json",
                JsonConvert.SerializeObject(chartData));
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
                string indexJSONPath = $"{Application.streamingAssetsPath}/index.json";
                if (!File.Exists(indexJSONPath))
                {
                    List<ChartFileIndex> chartFileIndices = new();
                    CreateNewChartIndex(indexJSONPath, chartFileIndices);
                }
                else
                {
                    string rawData = File.ReadAllText(indexJSONPath);
                    List<ChartFileIndex> chartFileIndices =
                        JsonConvert.DeserializeObject<List<ChartFileIndex>>(rawData);
                    CreateNewChartIndex(indexJSONPath, chartFileIndices);
                }

                CreatChart();

                parentObject.gameObject.SetActive(false);
                ChartList.Instance.RefreshList();
                return;
            }
            thisButton.interactable = true;
        }

        private void CreateNewChartIndex(string indexJSONPath, List<ChartFileIndex> chartFileIndices)
        {
            ChartFileIndex chartFileIndex = new();
            currentChartFileIndex = chartFileIndex.index = chartFileIndices.Count;
            chartFileIndices.Add(chartFileIndex);
            chartFileIndices[currentChartFileIndex].musicName = musicNameText.text;
            chartFileIndices[currentChartFileIndex].musicPath = musicPathText.text;
            chartFileIndices[currentChartFileIndex].IllustrationPath = illustrationPathText.text;
            MetaData metaData = new()
            {
                musicName = musicNameText.text,
                musicWriter = musicWriterText.text,
                artWriter = illustrationWriterText.text,
                chartWriter = chartWriterText.text,
                chartLevel = chartLevelText.text,
                description = descriptionText.text,
                chartHard = GlobalData.Instance.currentHard
            };
            chartFileIndices[currentChartFileIndex].metaData = metaData;
            File.WriteAllText(indexJSONPath, JsonConvert.SerializeObject(chartFileIndices, Formatting.Indented));
            CreateDirectory();
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