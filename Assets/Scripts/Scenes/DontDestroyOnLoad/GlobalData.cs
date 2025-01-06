using Data.ChartData;
using Data.EaseData;
using Data.Enumerate;
using Form.EventEdit;
using Form.LabelWindow;
using Newtonsoft.Json;
using Scenes.Edit;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UtilityCode.AssemblySystem;
using UtilityCode.ChartTool;
using UtilityCode.Singleton;

namespace Scenes.DontDestroyOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        public Hard currentHard;
        public ChartData chartData;
        public Data.ChartEdit.ChartData chartEditData;
        public AudioClip clip;

        [FormerlySerializedAs("currentCP")] public Sprite currentCp;

        [FormerlySerializedAs("currentCPH")] public Sprite currentCph;

        public bool isAutoplay = true;
        public int currentChartIndex = -1;

        public Alert alert;

        public List<LabelWindowContent> labelWindowContents = new();


        public LabelItem labelItemPrefab;
        public LabelWindow labelWindowPrefab;

        public EventEditItem eventEditItem;

        public TapEdit tapEditPrefab;
        public DragEdit dragEditPrefab;
        public HoldEdit holdEditPrefab;
        public FlickEdit flickEditPrefab;
        public PointEdit pointEditPrefab;
        public FullFlickEdit fullFlickEditPrefab;
        public VectrosityLineMask vectrosityLineMask;

        public List<EaseData> easeDatas;
        public List<EaseData> simpleEaseDatas;
        public bool isNewEditData;
        public List<Action> loopCallBacks = new();
        public int ScreenWidth => Camera.main.pixelWidth;
        public int ScreenHeight => Camera.main.pixelHeight;

        private IEnumerator Start()
        {
            Application.targetFrameRate = 9999;
            easeDatas = JsonConvert.DeserializeObject<List<EaseData>>(
                File.ReadAllText($"{Application.streamingAssetsPath}/Config/EaseDatas.json"));
            simpleEaseDatas =
                JsonConvert.DeserializeObject<List<EaseData>>(
                    File.ReadAllText($"{Application.streamingAssetsPath}/Config/SimpleEaseDatas.json"));
            if (isNewEditData)
            {
                ChartTool.CreateNewChart(chartEditData, easeDatas);
                //chartData.boxes = ChartTool.ConvertChartEdit2ChartData(chartEditData.boxes);
            }

            if (File.Exists($"{Application.streamingAssetsPath}/Config/Disclaimer.txt"))
            {
                if (bool.TryParse(File.ReadAllText($"{Application.streamingAssetsPath}/Config/Disclaimer.txt"), out bool result) && !result)
                {
                    ShowDisclaimer();
                }
            }
            else
            {
                ShowDisclaimer();
            }
            while (true)
            {
                yield return new WaitForSeconds(.1f);
                if (loopCallBacks.Count <= 0)
                {
                    continue;
                }

                Action action = loopCallBacks[0];
                loopCallBacks.RemoveAt(0);
                action();
            }
        }

        private static void ShowDisclaimer()
        {
            Alert.EnableAlert("使用本软件制作谱面之前，请明确获得相关素材的作者授权，本软件以及开发者不为因使用未授权的相关素材或其他形式产生的版权问题负责。继续使用本软件代表您同意，否则关闭本软件。");
            File.WriteAllText($"{Application.streamingAssetsPath}/Config/Disclaimer.txt", "True");
        }

        private void OnDestroy()
        {
            Destroy(gameObject);
        }

        public static void Refresh<T>(Action<T> action)
        {
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<T>(),
                interfaceMethod => action?.Invoke(interfaceMethod));
        }

        protected override void OnAwake()
        {
            //if(Instance!=null)Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator ReadResource()
        {
            yield return new WaitForEndOfFrame();
            UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip(
                $"file://{Application.streamingAssetsPath}/{currentChartIndex}/Music/Music.mp3", AudioType.MPEG);
            yield return unityWebRequest.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            unityWebRequest = UnityWebRequestTexture.GetTexture(
                $"file://{Application.streamingAssetsPath}/{currentChartIndex}/Illustration/Background.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCph = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
            //unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/{currentChartIndex}/Illustration/CP.png");
            //yield return unityWebRequest.SendWebRequest();
            //Texture2D cp = DownloadHandlerTexture.GetContent(unityWebRequest);
            //currentCp = Sprite.Create(cp, new Rect(0, 0, cp.width, cp.height), new Vector2(0.5f, 0.5f));
        }
    }
}