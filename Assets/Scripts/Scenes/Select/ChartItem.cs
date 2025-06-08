using System.Collections;
using System.IO;
using Data.ChartData;
using Hook;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Scenes.Select
{
    public class ChartItem : PublicButton
    {
        public MetaData metaData;
        public TMP_Text musicName;
        public Image illustrationPreview;
        public TMP_Text chartInfomation;
        public string currentChartIndex;

        private void Start()
        {
            thisButton.onClick.AddListener(() =>
            {
                GlobalData.Instance.currentChartIndex = currentChartIndex;
                string illustrationPath =
                    $"{Applicationm.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/Illustration";
                illustrationPath = $"{Directory.GetFiles(illustrationPath)[0]}";
                StartCoroutine(GetIllustration(illustrationPath));
                illustrationPreview.color = Color.white;
                illustrationPreview.type = Image.Type.Simple;
                illustrationPreview.preserveAspect = true;
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