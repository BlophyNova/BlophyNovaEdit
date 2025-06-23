using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using Data.ChartData;
using Data.EaseData;
using Data.Enumerate;
using Data.GeneralSettings;
using Form.EventEdit;
using Form.LabelWindow;
using Hook;
using Manager;
using Newtonsoft.Json;
using Scenes.Edit;
using Scenes.PublicScripts;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UtilityCode.AssemblySystem;
using UtilityCode.ChartTool;
using UtilityCode.Singleton;
using ChartData = Data.ChartData.ChartData;

namespace Scenes.DontDestroyOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        public delegate void OnStartEdit();

        public Hard currentHard;
        public ChartData chartData;
        public MetaData metaData;
        public Data.ChartEdit.ChartData chartEditData;
        public AudioClip clip;
        public GeneralData generalData;

        [FormerlySerializedAs("currentCP")] public Sprite currentCp;

        [FormerlySerializedAs("currentCPH")] public Sprite currentCph;

        public bool isAutoplay = true;
        public string currentChartIndex = string.Empty;

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
        public bool isNewEditData;

        public bool saveChartData;
        public bool isInited;
        public List<Action> loopCallBacks = new();
        public int ScreenWidth => Camera.main.pixelWidth;
        public int ScreenHeight => Camera.main.pixelHeight;

        private void Start()
        {
            //File.WriteAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/GeneralData.json").LocalPath,JsonConvert.SerializeObject(GlobalData.Instance.generalData),Encoding.UTF8);
            Init();
        }

        private void Update()
        {
            if (saveChartData)
            {
                saveChartData = false;
                string chartDataContent = JsonConvert.SerializeObject(chartData);
                File.WriteAllText(new Uri($"{Applicationm.streamingAssetsPath}/Chart.json").LocalPath, chartDataContent,
                    Encoding.UTF8);
            }
        }

        private void OnDestroy()
        {
            Destroy(gameObject);
        }

        private async void Init()
        {
            await Applicationm.Init(Applicationm.streamingAssetsPath);
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 9999;
            easeDatas = JsonConvert.DeserializeObject<List<EaseData>>(
                File.ReadAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/EaseDatas.json").LocalPath,
                    Encoding.UTF8));
            if (isNewEditData)
            {
                ChartTool.CreateNewChart(chartEditData, easeDatas);
                BPMManager.UpdateInfo(chartEditData.bpmList);
                //chartData.boxes = ChartTool.ConvertChartEdit2ChartData(chartEditData.boxes);
            }
            else
            {
                if (SceneManager.GetActiveScene().name == "Edit")
                {
                    chartEditData = JsonConvert.DeserializeObject<Data.ChartEdit.ChartData>(
                        File.ReadAllText(
                            new Uri(
                                    $"{Applicationm.streamingAssetsPath}/{currentChartIndex}/ChartFile/{currentHard}/Chart.json")
                                .LocalPath, Encoding.UTF8));
                    BPMManager.UpdateInfo(chartEditData.bpmList);
                }
            }

            if (SceneManager.GetActiveScene().name == "Edit")
            {
                generalData = JsonConvert.DeserializeObject<GeneralData>(File.ReadAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/GeneralData.json").LocalPath,Encoding.UTF8));
                generalData.Init();
            }
            Disclaimer();
            isInited = true;
            while (true)
            {
                await UniTask.WaitForSeconds(.1f);
                if (loopCallBacks.Count <= 0)
                {
                    continue;
                }

                Action action = loopCallBacks[0];
                loopCallBacks.RemoveAt(0);
                action();
            }
        }

        public event OnStartEdit onStartEdit = () => { };

        public void StartEdit()
        {
            onStartEdit();
        }

        private static void Disclaimer()
        {
            if (Application.isEditor)
            {
                return;
            }

            if (File.Exists($"{Applicationm.streamingAssetsPath}/Config/Disclaimer.txt"))
            {
                if (bool.TryParse(
                        File.ReadAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/Disclaimer.txt").LocalPath,
                            Encoding.UTF8),
                        out bool result) && !result)
                {
                    ShowDisclaimer();
                }
            }
            else
            {
                ShowDisclaimer();
            }
        }

        private static void ShowDisclaimer()
        {
            Alert.EnableAlert("使用本软件制作谱面之前，请明确获得相关素材的作者授权，本软件以及开发者不为因使用未授权的相关素材或其他形式产生的版权问题负责。继续使用本软件代表您同意，否则关闭本软件。");
            File.WriteAllText(new Uri($"{Applicationm.streamingAssetsPath}/Config/Disclaimer.txt").LocalPath, "True",
                Encoding.UTF8);
        }
        /// <summary>
        /// 刷新方法
        /// </summary>
        /// <param name="action">刷新内容，一般是执行Refresh()方法</param>
        /// <param name="types">需要执行的方法类，默认白名单，就是执行出现的类，如果为null就是执行所有的类</param>
        /// <param name="isBlackList">是否为黑名单模式，默认为False，当此值为True时，执行列表中没有出现的类</param>
        /// <typeparam name="T"></typeparam>
        public static void Refresh<T>(Action<T> action, List<Type> types, bool isBlackList = false)
        {
            List<T> foundTypes = AssemblySystem.FindAllInterfaceByTypes<T>();
            AssemblySystem.Exe(foundTypes, interfaceMethod => action?.Invoke(interfaceMethod), types, isBlackList);
        }

        protected override void OnAwake()
        {
            //if(Instance!=null)Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        public IEnumerator ReadResource()
        {
            yield return new WaitForEndOfFrame();
            string musicPath = $"{Applicationm.streamingAssetsPath}/{currentChartIndex}/Music";
            musicPath = Directory.GetFiles(musicPath)[0];
            AudioType audioType = Path.GetExtension(musicPath).ToLower() switch
            {
                ".mp3" => AudioType.MPEG,
                ".ogg" => AudioType.OGGVORBIS,
                ".wav" => AudioType.WAV,
                _ => throw new Exception("呜呜呜，瓦没见过这个音频格式喵···")
            };

            UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{musicPath}", audioType);
            yield return unityWebRequest.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);


            string illustrationPath = $"{Applicationm.streamingAssetsPath}/{currentChartIndex}/Illustration";
            illustrationPath = Directory.GetFiles(illustrationPath)[0];
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{illustrationPath}");
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