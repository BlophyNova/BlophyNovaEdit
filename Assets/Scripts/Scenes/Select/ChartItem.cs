using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChartItem : PublicButton
{
    public TMP_Text musicName;
    public ChartFileIndex thisChartFileIndex;
    public Image illustrationPreview;
    public TMP_Text chartInfomation;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            GlobalData.Instance.currentChartIndex = thisChartFileIndex.index;
            StartCoroutine(GetIllustration($"{Application.streamingAssetsPath}/{GlobalData.Instance.currentChartIndex}/Illustration/Background.png"));
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
    IEnumerator GetIllustration(string path)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}");
        yield return unityWebRequest.SendWebRequest();
        Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
        illustrationPreview.sprite = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
    }
}
