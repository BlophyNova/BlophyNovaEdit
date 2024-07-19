using System.Collections;
using Data.ChartData;
using Newtonsoft.Json;
using Scenes.PublicScripts;
using UnityEngine;
using UnityEngine.UI;
using GlobalData = Scenes.DontDestoryOnLoad.GlobalData;
namespace Scenes.SelectMusic
{
    public class ControlSpace : PublicControlSpace
    {
        public static ControlSpace instance;
        private void Awake() => instance = this;
        public string[] musics;
        public Image musicPrefab;
        private static new IEnumerator Send()
        {
            ResourceRequest rawChart = Resources.LoadAsync<TextAsset>($"MusicPack/{GlobalData.Instance.currentChapter}/{GlobalData.Instance.currentMusic}/ChartFile/{GlobalData.Instance.currentHard}/Chart");
            yield return rawChart;
            TextAsset rawChartTex = rawChart.asset as TextAsset;
            //GlobalData.Instance. = JsonConvert.DeserializeObject<ChartData>(chart);
            ChartData chart = JsonConvert.DeserializeObject<ChartData>(rawChartTex.text);
            GlobalData.Instance.chartData = chart;
            UIManager.Instance.SelectMusic(chart.metaData.musicName, chart.metaData.musicWriter, chart.metaData.chartWriter, chart.metaData.artWriter);

            ResourceRequest currentCp = Resources.LoadAsync<Sprite>($"MusicPack/{GlobalData.Instance.currentChapter}/{GlobalData.Instance.currentMusic}/Background/CP");
            yield return currentCp;
            GlobalData.Instance.currentCp = currentCp.asset as Sprite;

            ResourceRequest currentCph = Resources.LoadAsync<Sprite>($"MusicPack/{GlobalData.Instance.currentChapter}/{GlobalData.Instance.currentMusic}/Background/CPH");
            yield return currentCph;
            GlobalData.Instance.currentCph = currentCph.asset as Sprite;

            ResourceRequest clip = Resources.LoadAsync<AudioClip>($"MusicPack/{GlobalData.Instance.currentChapter}/{GlobalData.Instance.currentMusic}/Music/Music");
            yield return clip;
            GlobalData.Instance.clip = clip.asset as AudioClip;


        }
        private void UploadSyncMusicIndex()
        {
            GlobalData.Instance.currentMusicIndex = currentElementIndex;
            GlobalData.Instance.currentMusic = musics[currentElementIndex];
        }
        private void DownloadSyncMusicIndex()
        {
            currentElementIndex = GlobalData.Instance.currentMusicIndex;
            currentElement = allElementDistance[elementCount - 1 - currentElementIndex];
            GlobalData.Instance.currentMusic = musics[currentElementIndex];
        }
        protected override void OnStart()
        {
            musics = GlobalData.Instance.chapters[GlobalData.Instance.currentChapterIndex].musicPath;
            int currentChapterMusicCount = GlobalData.Instance.chapters[GlobalData.Instance.currentChapterIndex].musicPath.Length;
            elementCount = currentChapterMusicCount;
            currentElement = 1;
            InitAllElementDistance();
            for (int i = 0; i < currentChapterMusicCount; i++)
            {
                Instantiate(musicPrefab, transform).sprite = Resources.Load<Sprite>($"MusicPack/{GlobalData.Instance.currentChapter}/{GlobalData.Instance.chapters[GlobalData.Instance.currentChapterIndex].musicPath[i]}/Background/CPH");
            }
            DownloadSyncMusicIndex();
            StartCoroutine(Send());
            StartCoroutine(ReturnLastTimeCancelMusic());

        }
        private IEnumerator ReturnLastTimeCancelMusic()
        {
            yield return new WaitForEndOfFrame();
            verticalBar.value = allElementDistance[elementCount - 1 - currentElementIndex];
        }

        private Vector2 startPoint;
        private Vector2 endPoint;
        protected override void LargeImageUpdate()
        {
            if( Input.touchCount <= 0 )
                return;
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPoint = touch.position;
                    break;
                case TouchPhase.Ended:
                {
                    endPoint = touch.position;
                    float deltaY = (endPoint - startPoint).y;
                    if (deltaY > 300 && currentElementIndex + 1 < elementCount)
                    {
                        currentElement = allElementDistance[elementCount - 1 - ++currentElementIndex];
                    }
                    else if (deltaY < -300 && currentElementIndex - 1 >= 0)
                    {
                        currentElement = allElementDistance[elementCount - 1 - --currentElementIndex];
                    }
                    UploadSyncMusicIndex();
                    StartCoroutine(Send());
                    StartCoroutine(Lerp());
                    break;
                }
            }
        }
    }
}
