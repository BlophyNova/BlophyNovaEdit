using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Data.ChartData;
using Hook;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scenes.Select
{
    public class Hard : PublicButton
    {
        public Data.Enumerate.Hard hard;
        public List<Hard> otherButton = new();
        public Image image;
        public TMP_Text chartInfomation;
        public Color selectedColor;
        public Color unselectedColor;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.currentHard = hard;
                image.color = selectedColor;
                foreach (Hard hard in otherButton)
                {
                    hard.image.color = unselectedColor;
                }
                
                string chartJsonPath = $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/ChartFile/{GlobalData.Instance.currentHard}/MetaData.json";
                string rawData = File.ReadAllText(new Uri(chartJsonPath).LocalPath, Encoding.UTF8);
                GlobalData.Instance.metaData = JsonConvert.DeserializeObject<MetaData>(rawData);
                MetaData metaData = GlobalData.Instance.metaData;
                
                string chartVersion = metaData.chartVersion == 0 ? "未知" : $"{metaData.chartVersion}";
                chartInfomation.text =
                    $"谱面版本:{chartVersion}\n" +
                    $"曲名:{metaData.musicName}\n" +
                    $"曲师:{metaData.musicWriter}\n" +
                    $"谱师:{metaData.chartWriter}\n" +
                    $"画师:{metaData.artWriter}\n" +
                    $"定数:{metaData.chartLevel}\n" +
                    $"描述:{metaData.description}";
            });
        }
    }
}