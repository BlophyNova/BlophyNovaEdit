using Data;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Scenes.Select
{
    public class ChartItem : PublicButton
    {
        public TMP_Text musicName;
        public Image illustrationPreview;
        public TMP_Text chartInfomation;
        public ChartFileIndex thisChartFileIndex;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.currentChartIndex = thisChartFileIndex.index;
                string illustrationPath = $"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/Illustration";
                illustrationPath = $"{Directory.GetFiles(illustrationPath)[0]}";
                StartCoroutine(GetIllustration(illustrationPath));
                illustrationPreview.color = Color.white;
                illustrationPreview.type = Image.Type.Simple;
                illustrationPreview.preserveAspect = true;
                chartInfomation.text =
                    $"曲名:{thisChartFileIndex.metaData.musicName}\n" +
                    $"曲师:{thisChartFileIndex.metaData.musicWriter}\n" +
                    $"谱师:{thisChartFileIndex.metaData.chartWriter}\n" +
                    $"画师:{thisChartFileIndex.metaData.artWriter}\n" +
                    $"定数:{thisChartFileIndex.metaData.chartLevel}\n" +
                    $"描述:{thisChartFileIndex.metaData.description}";
            });
        }

        private IEnumerator GetIllustration(string path)
        {
            UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            illustrationPreview.sprite =
                Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
        }
    }
}