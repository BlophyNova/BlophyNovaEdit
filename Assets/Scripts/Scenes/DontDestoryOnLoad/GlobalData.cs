using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Data.ChartData;
using Data.Enumerate;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UtilityCode.Singleton;
namespace Scenes.DontDestoryOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        public string currentHard;
        public ChartData chartData;
        public Data.ChartEdit.ChartData chartEditData;
        public AudioClip clip;
        [FormerlySerializedAs("currentCP")]
        public Sprite currentCp;
        [FormerlySerializedAs("currentCPH")]
        public Sprite currentCph;
        public bool isAutoplay = true;
        public float offset;
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

        public static void Refresh()
        {
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<IRefresh>(), (interfaceMethod) => interfaceMethod.Refresh());
        }
        protected override void OnAwake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public IEnumerator ReadResource()
        {
            UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.streamingAssetsPath}/-1/Music/Music.mp3", AudioType.MPEG);
            yield return unityWebRequest.SendWebRequest();
            clip = DownloadHandlerAudioClip.GetContent(unityWebRequest);
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/-1/Illustration/CPH.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cph = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCph = Sprite.Create(cph, new Rect(0, 0, cph.width, cph.height), new Vector2(0.5f, 0.5f));
            unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{Application.streamingAssetsPath}/-1/Illustration/CP.png");
            yield return unityWebRequest.SendWebRequest();
            Texture2D cp = DownloadHandlerTexture.GetContent(unityWebRequest);
            currentCp = Sprite.Create(cp, new Rect(0, 0, cp.width, cp.height), new Vector2(0.5f, 0.5f));
        }
        private void Start()
        {
            Application.targetFrameRate = 9999;
        }
    }
    
    [Serializable]
    public class Chapter
    {
        public string chapterName;
        public string[] musicPath;
    }
}