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
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using UtilityCode.Singleton;
namespace Scenes.DontDestroyOnLoad
{
    public class GlobalData : MonoBehaviourSingleton<GlobalData>
    {
        public string currentHard;
        public Data.ChartData.ChartData chartData;
        public Data.ChartEdit.ChartData chartEditData;
        public AudioClip clip;
        [FormerlySerializedAs("currentCP")]
        public Sprite currentCp;
        [FormerlySerializedAs("currentCPH")]
        public Sprite currentCph;
        public bool isAutoplay = true;
        public float offset;
        public float playSpeed=1;
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

        public List<EaseData> easeData;
        public static void Refresh<T>(Action<T> action)
        {
            AssemblySystem.Exe(AssemblySystem.FindAllInterfaceByTypes<T>(), (interfaceMethod) => action?.Invoke(interfaceMethod));
        }
        public void AddNoteEdit2ChartData(Data.ChartEdit.Note noteEdit,int boxID,int lineID)
        {
            int index_noteEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[lineID].onlineNotes, m => m.hitBeats.ThisStartBPM < noteEdit.hitBeats.ThisStartBPM, false);
            chartEditData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteEdits, noteEdit);

            Data.ChartData.Note note = new(noteEdit);
            int index_noteChartData = Algorithm.BinarySearch(chartData.boxes[boxID].lines[lineID].onlineNotes, m => m.hitTime < note.hitTime, false);
            if (noteEdit.noteType != NoteType.Point) chartData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteChartData, note);
            else
                chartData.boxes[boxID].lines[4].onlineNotes.Insert(Algorithm.BinarySearch(chartData.boxes[boxID].lines[4].onlineNotes, m => m.hitTime < note.hitTime, false), note);
            //else
            //{
            //    int index_pointEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[4].onlineNotes, m => m.hitBeats.ThisStartBPM < noteEdit.hitBeats.ThisStartBPM, false);
            //    chartData.boxes[boxID].lines[4].onlineNotes.Insert(index_pointEdits, note);
            //}
        }
        public void RefreshChartEventByChartEditEvent(List<Data.ChartData.Event> chartDataEvent,Data.ChartEdit.Event chartEditDataEvent)
        {
            int index_noteEdits = Algorithm.BinarySearch(chartDataEvent, m => m.startTime <BPMManager.Instance.GetSecondsTimeWithBeats(chartEditDataEvent.startBeats.ThisStartBPM), false);
            Data.ChartData.Event @event = new(chartEditDataEvent);
            chartDataEvent.Insert(index_noteEdits, @event);
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
            easeData = JsonConvert.DeserializeObject<List<EaseData>>(File.ReadAllText($"{Application.streamingAssetsPath}/Config/EaseData.json")); 
            CreateNewChart();
            chartData.boxes= ConvertChartEdit2ChartData(chartEditData.boxes);
        }
        public void CreateNewChart()
        {
            chartEditData.boxes = new();
            Data.ChartEdit.Box chartEditBox = CreateNewBox();
            chartEditData.boxes.Add(chartEditBox);
        }

        public Data.ChartEdit.Box CreateNewBox()
        {
            Data.ChartEdit.Box chartEditBox = new();
            chartEditBox.lines = new() { new(), new(), new(), new(), new() };
            chartEditBox.boxEvents = new();
            chartEditBox.boxEvents.scaleX = new();
            chartEditBox.boxEvents.scaleY = new();
            chartEditBox.boxEvents.moveX = new();
            chartEditBox.boxEvents.moveY = new();
            chartEditBox.boxEvents.centerX = new();
            chartEditBox.boxEvents.centerY = new();
            chartEditBox.boxEvents.alpha = new();
            chartEditBox.boxEvents.lineAlpha = new();
            chartEditBox.boxEvents.rotate = new();
            chartEditBox.boxEvents.speed = new();
            chartEditBox.boxEvents.scaleX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
            chartEditBox.boxEvents.scaleY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
            chartEditBox.boxEvents.moveX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.moveY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.centerX.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = .5f, endValue = .5f, curve = easeData[0] });
            chartEditBox.boxEvents.centerY.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = .5f, endValue = .5f, curve = easeData[0] });
            chartEditBox.boxEvents.alpha.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 1, endValue = 1, curve = easeData[0] });
            chartEditBox.boxEvents.lineAlpha.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.rotate.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 0, endValue = 0, curve = easeData[0] });
            chartEditBox.boxEvents.speed.Add(new() { startBeats = BPM.Zero, endBeats = BPM.One, startValue = 3, endValue = 3, curve = easeData[0] });
            for (int i = 0; i < chartEditBox.lines.Count; i++)
            {
                chartEditBox.lines[i].offlineNotes = new();
                chartEditBox.lines[i].onlineNotes = new();
            }

            return chartEditBox;
        }

        public List<Data.ChartData.Box> ConvertChartEdit2ChartData(List<Data.ChartEdit.Box> boxes)
        {
            List<Data.ChartData.Box> result = new();
            foreach (Data.ChartEdit.Box box in boxes)
            {
                Data.ChartData.Box chartDataBox = new();
                chartDataBox.lines = new() { new(), new(), new(), new(), new() };
                chartDataBox.boxEvents = new();
                chartDataBox.boxEvents.scaleX = new();
                chartDataBox.boxEvents.scaleY = new();
                chartDataBox.boxEvents.moveX = new();
                chartDataBox.boxEvents.moveY = new();
                chartDataBox.boxEvents.centerX = new();
                chartDataBox.boxEvents.centerY = new();
                chartDataBox.boxEvents.alpha = new();
                chartDataBox.boxEvents.lineAlpha = new();
                chartDataBox.boxEvents.rotate = new();
                ForeachBoxEvents(box.boxEvents.scaleX, chartDataBox.boxEvents.scaleX);
                ForeachBoxEvents(box.boxEvents.scaleY, chartDataBox.boxEvents.scaleY);
                ForeachBoxEvents(box.boxEvents.moveX, chartDataBox.boxEvents.moveX);
                ForeachBoxEvents(box.boxEvents.moveY, chartDataBox.boxEvents.moveY);
                ForeachBoxEvents(box.boxEvents.centerX, chartDataBox.boxEvents.centerX);
                ForeachBoxEvents(box.boxEvents.centerY, chartDataBox.boxEvents.centerY);
                ForeachBoxEvents(box.boxEvents.alpha, chartDataBox.boxEvents.alpha);
                ForeachBoxEvents(box.boxEvents.lineAlpha, chartDataBox.boxEvents.lineAlpha);
                ForeachBoxEvents(box.boxEvents.rotate, chartDataBox.boxEvents.rotate);
                for (int i = 0; i < chartDataBox.lines.Count; i++)
                {
                    chartDataBox.lines[i].offlineNotes = new();
                    foreach (Data.ChartEdit.Note item in box.lines[i].offlineNotes)
                    {
                        Data.ChartData.Note newChartDataNote = new(item);
                        chartDataBox.lines[i].offlineNotes.Add(newChartDataNote);
                    }
                    chartDataBox.lines[i].onlineNotes = new();
                    foreach (Data.ChartEdit.Note item in box.lines[i].onlineNotes)
                    {
                        Data.ChartData.Note newChartDataNote = new(item);
                        chartDataBox.lines[i].onlineNotes.Add(newChartDataNote);
                    }
                    List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(box.boxEvents.speed);
                    chartDataBox.lines[i].speed = new();
                    ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
                    chartDataBox.lines[i].career = new() { postWrapMode=WrapMode.ClampForever,preWrapMode=WrapMode.ClampForever};
                    chartDataBox.lines[i].career.keys = GameUtility.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray();
                    chartDataBox.lines[i].far = new() { postWrapMode=WrapMode.ClampForever,preWrapMode=WrapMode.ClampForever};
                    chartDataBox.lines[i].far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
                }
                result.Add(chartDataBox);
            }
            return result;
        }

        public static void ForeachBoxEvents(List<Data.ChartEdit.Event> editBoxEvent, List<Data.ChartData.Event> chartDataBoxEvent)
        {
            foreach (Data.ChartEdit.Event item in editBoxEvent)
            {
                chartDataBoxEvent.Add(new() { startTime = item.startBeats.ThisStartBPM, endTime = item.endBeats.ThisStartBPM, startValue = item.startValue, endValue = item.endValue, curve = item.curve.thisCurve });
            }
        }
        
    }
}