using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Data.ChartData;
using Data.ChartEdit;
using Data.Enumerate;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using UtilityCode.Singleton;
using xeetsh.ReadOnlyInspectorAttributeKit;
namespace Scenes.DontDestroyOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        public Hard currentHard;
        public Data.ChartData.ChartData chartData;
        public Data.ChartEdit.ChartData chartEditData;
        public AudioClip clip;
        [FormerlySerializedAs("currentCP")]
        public Sprite currentCp;
        [FormerlySerializedAs("currentCPH")]
        public Sprite currentCph;
        public bool isAutoplay = true;
        public int currentChartIndex = -1;
        public int ScreenWidth => Camera.main.pixelWidth;
        public int ScreenHeight => Camera.main.pixelHeight;

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

        public List<EaseData> easeData;
        public static void Refresh<T>(Action<T> action)
        {
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<T>(), (interfaceMethod) => action?.Invoke(interfaceMethod));
        }

        protected override void OnAwake()
        {
            //if(Instance!=null)Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            Destroy(gameObject);
        }

        public IEnumerator ReadResource()
        {
            yield return new WaitForEndOfFrame();
            UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.streamingAssetsPath}/{currentChartIndex}/Music/Music.mp3", AudioType.MPEG);
            yield return unityWebRequest.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/{currentChartIndex}/Illustration/Background.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCph = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
            //unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/{currentChartIndex}/Illustration/CP.png");
            //yield return unityWebRequest.SendWebRequest();
            //Texture2D cp = DownloadHandlerTexture.GetContent(unityWebRequest);
            //currentCp = Sprite.Create(cp, new Rect(0, 0, cp.width, cp.height), new Vector2(0.5f, 0.5f));
        }
        public List<Action> loopCallBacks = new();
        public bool isNewEditData;
        private IEnumerator Start()
        {
            Application.targetFrameRate = 9999;
            easeData = JsonConvert.DeserializeObject<List<EaseData>>(File.ReadAllText($"{Application.streamingAssetsPath}/Config/EaseData.json"));
            if (isNewEditData)
            {
                ChartTool.CreateNewChart(chartEditData, easeData);
                //chartData.boxes = ChartTool.ConvertChartEdit2ChartData(chartEditData.boxes);
            }
            while (true)
            {
                yield return new WaitForSeconds(.1f);
                if (loopCallBacks.Count <= 0) continue;
                Action action = loopCallBacks[0];
                loopCallBacks.RemoveAt(0);
                action();
            }
        }
    }
}