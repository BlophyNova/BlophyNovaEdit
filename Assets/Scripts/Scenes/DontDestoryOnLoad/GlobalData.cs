using System;
using System.Collections;
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
        public Chapter[] chapters;
        public string currentChapter;
        public int currentChapterIndex;
        public string currentMusic;
        public int currentMusicIndex;
        public string currentHard;
        public ChartData chartData;
        public AudioClip clip;
        [FormerlySerializedAs("currentCP")]
        public Sprite currentCp;
        [FormerlySerializedAs("currentCPH")]
        public Sprite currentCph;
        public Grade score;
        public bool isAutoplay = true;
        public float offset;
        public int ScreenWidth => Camera.main.pixelWidth;
        public int ScreenHeight => Camera.main.pixelHeight;
        [FormerlySerializedAs("WhereToEnterSettings")]
        public string whereToEnterSettings;

        public Alert alert;
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
    public class Grade
    {
        public int tapPerfect;
        public int holdPerfect;
        public int dragPerfect;
        public int flickPerfect;
        public int fullFlickPerfect;
        public int pointPerfect;

        public int tapGood;
        public int holdGood;
        public int pointGood;
        [FormerlySerializedAs("tapEarly_good")]
        public int tapEarlyGood;
        [FormerlySerializedAs("holdEarly_good")]
        public int holdEarlyGood;
        [FormerlySerializedAs("pointEarly_good")]
        public int pointEarlyGood;
        [FormerlySerializedAs("tapLate_good")]
        public int tapLateGood;
        [FormerlySerializedAs("holdLate_good")]
        public int holdLateGood;
        [FormerlySerializedAs("pointLate_good")]
        public int pointLateGood;

        public int tapBad;
        [FormerlySerializedAs("tapEarly_bad")]
        public int tapEarlyBad;
        [FormerlySerializedAs("tapLate_bad")]
        public int tapLateBad;
        public int pointBad;
        [FormerlySerializedAs("pointEarly_bad")]
        public int pointEarlyBad;
        [FormerlySerializedAs("pointLate_bad")]
        public int pointLateBad;

        public int tapMiss;
        public int holdMiss;
        public int dragMiss;
        public int flickMiss;
        public int fullFlickMiss;
        public int pointMiss;


        public int Perfect => tapPerfect + holdPerfect + dragPerfect + flickPerfect + fullFlickPerfect + pointPerfect;

        public int Good => tapGood + holdGood + pointGood;
        public int EarlyGood => tapEarlyGood + holdEarlyGood + pointEarlyGood;
        public int LateGood => tapLateGood + holdLateGood + pointLateGood;

        public int Bad => tapBad + pointBad;
        public int EarlyBad => tapEarlyBad + pointEarlyBad;
        public int LateBad => tapLateBad + pointLateBad;

        public int Miss => tapMiss + holdMiss + dragMiss + flickMiss + fullFlickMiss + pointMiss;
        public int combo;
        public int Combo
        {
            get => combo;
            set
            {
                combo = value;
                maxCombo = maxCombo >= combo ? maxCombo : combo;
            }
        }
        public int maxCombo;

        public int tapCount;
        public int holdCount;
        public int dragCount;
        public int flickCount;
        public int fullFlickCount;
        public int pointCount;
        public int JudgedTapCount => tapPerfect + tapGood + tapBad + tapMiss;
        public int JudgedHoldCount => holdPerfect + holdGood + holdMiss;
        public int JudgedDragCount => dragPerfect + dragMiss;
        public int JudgedFlickCount => flickPerfect + flickMiss;
        public int JudgedFullFlickCount => fullFlickPerfect + fullFlickMiss;
        public int JudgedPointCount => pointPerfect + pointGood + pointMiss;
        
        public int NoteCount => tapCount + holdCount + dragCount + flickCount + fullFlickCount + pointCount;
        public float Accuracy => (Perfect + Good * ValueManager.Instance.goodJudgePercent) / NoteCount;

        public float delta = -1;
        public float Delta
        {
            get
            {
                if (delta < 0)
                {
                    delta = 350000f /
                        (ValueManager.Instance.tapWeight * tapCount +
                        ValueManager.Instance.holdWeight * holdCount +
                        ValueManager.Instance.dragWeight * dragCount +
                        ValueManager.Instance.flickWeight * flickCount +
                        ValueManager.Instance.fullFlickWeight * fullFlickCount +
                        ValueManager.Instance.pointWeight * pointCount);
                }
                return delta;
            }
        }
        public float score;
        public float Score
        {
            get
            {
                float result = 500000f * Accuracy +
                    150000f * maxCombo / NoteCount +
                    Delta * (ValueManager.Instance.tapWeight * tapPerfect) +
                    Delta * (ValueManager.Instance.holdWeight * holdPerfect) +
                    Delta * (ValueManager.Instance.dragWeight * dragPerfect) +
                    Delta * (ValueManager.Instance.flickWeight * flickPerfect) +
                    Delta * (ValueManager.Instance.fullFlickWeight * fullFlickPerfect) +
                    Delta * (ValueManager.Instance.pointWeight * pointPerfect);
                return result;
            }
        }
        public void Reset()
        {
            tapPerfect = 0;
            holdPerfect = 0;
            dragPerfect = 0;
            flickPerfect = 0;
            fullFlickPerfect = 0;
            pointPerfect = 0;
            tapGood = 0;
            holdGood = 0;
            pointGood = 0;
            tapEarlyGood = 0;
            holdEarlyGood = 0;
            pointEarlyGood = 0;
            tapLateGood = 0;
            holdLateGood = 0;
            pointLateGood = 0;
            tapBad = 0;
            tapEarlyBad = 0;
            tapLateBad = 0;
            pointBad = 0;
            pointEarlyBad = 0;
            pointLateBad = 0;
            tapMiss = 0;
            holdMiss = 0;
            dragMiss = 0;
            flickMiss = 0;
            fullFlickMiss = 0;
            pointMiss = 0;
            combo = 0;
            maxCombo = 0;
            tapCount = 0;
            holdCount = 0;
            dragCount = 0;
            flickCount = 0;
            fullFlickCount = 0;
            pointCount = 0;
            delta = -1;
            score = 0;
        }
        /// <summary>
        /// 加分
        /// </summary>
        /// <param name="noteType">音符类型</param>
        /// <param name="noteJudge">判定等级</param>
        /// <param name="isEarly">是否过早</param>
        public void AddScore(NoteType noteType, NoteJudge noteJudge, bool isEarly)
        {
            switch (noteJudge)
            {
                case NoteJudge.Perfect://完美
                    AddScorePerfect(noteType);
                    break;
                case NoteJudge.Early://开 心 早 了
                    AddScoreGood(noteType, isEarly);
                    break;
                case NoteJudge.Late://后 知 后 觉
                    AddScoreGood(noteType, isEarly);
                    break;
                case NoteJudge.Bad://坏
                    AddScoreBad(noteType, isEarly);
                    break;
                case NoteJudge.Miss://小姐
                    AddScoreMiss(noteType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(noteJudge), noteJudge, null);
            }
        }
        /// <summary>
        /// 加Miss分
        /// </summary>
        /// <param name="noteType"></param>
        private void AddScoreMiss(NoteType noteType)
        {
            switch (noteType)
            {
                case NoteType.Tap:
                    tapMiss++;
                    Combo = 0;
                    break;
                case NoteType.Hold:
                    holdMiss++;
                    Combo = 0;
                    break;
                case NoteType.Drag:
                    dragMiss++;
                    Combo = 0;
                    break;
                case NoteType.Flick:
                    flickMiss++;
                    Combo = 0;
                    break;
                case NoteType.Point:
                    pointMiss++;
                    Combo = 0;
                    break;
                case NoteType.FullFlickPink:
                    fullFlickMiss++;
                    Combo = 0;
                    break;
                case NoteType.FullFlickBlue:
                    fullFlickMiss++;
                    Combo = 0;
                    break;
                default:
                    Debug.LogError("如果你看到这条消息,请截图并在群里@MojaveHao/Niubility748/HuaWaterED进行反馈\n" +
                        "加分出错:Miss但未找到音符类型");
                    break;
            }
        }
        /// <summary>
        /// 加Bad分
        /// </summary>
        /// <param name="noteType"></param>
        /// <param name="isEarly"></param>
        private void AddScoreBad(NoteType noteType, bool isEarly)
        {
            switch (noteType)
            {
                case NoteType.Tap:
                    tapBad++;
                    switch (isEarly)
                    {
                        case true:
                            tapEarlyBad++;
                            break;
                        case false:
                            tapLateBad++;
                            break;
                    }
                    Combo = 0;
                    break;
                case NoteType.Point:
                    pointBad++;
                    switch (isEarly)
                    {
                        case true:
                            pointEarlyBad++;
                            break;
                        case false:
                            pointLateBad++;
                            break;
                    }
                    break;
                default:
                    Debug.LogError("如果你看到这条消息,请截图并在群里@MojaveHao/Niubility748/HuaWaterED进行反馈\n" +
                        "加分出错:Bad但未找到音符类型");
                    break;
            }
        }
        /// <summary>
        /// 加Good分
        /// </summary>
        /// <param name="noteType"></param>
        /// <param name="isEarly"></param>
        private void AddScoreGood(NoteType noteType, bool isEarly)
        {
            switch (noteType)
            {
                case NoteType.Tap:
                    tapGood++;
                    switch (isEarly)
                    {
                        case true:
                            tapEarlyGood++;
                            break;
                        case false:
                            tapLateGood++;
                            break;
                    }
                    Combo++;
                    break;
                case NoteType.Hold:
                    holdGood++;
                    switch (isEarly)
                    {
                        case true:
                            holdEarlyGood++;
                            break;
                        case false:
                            holdLateGood++;
                            break;
                    }
                    Combo++;
                    break;
                case NoteType.Point:
                    pointGood++;
                    switch (isEarly)
                    {
                        case true:
                            pointEarlyGood++;
                            break;
                        case false:
                            pointLateGood++;
                            break;
                    }
                    Combo++;
                    break;
                default:
                    Debug.LogError("如果你看到这条消息,请截图并在群里@MojaveHao/Niubility748/HuaWaterED进行反馈\n" +
                        "加分出错:Good但未找到音符类型");
                    break;
            }
        }
        /// <summary>
        /// 加Perfect分
        /// </summary>
        /// <param name="noteType"></param>
        private void AddScorePerfect(NoteType noteType)
        {
            switch (noteType)
            {
                case NoteType.Tap:
                    tapPerfect++;
                    Combo++;
                    break;
                case NoteType.Hold:
                    holdPerfect++;
                    Combo++;
                    break;
                case NoteType.Drag:
                    dragPerfect++;
                    Combo++;
                    break;
                case NoteType.Flick:
                    flickPerfect++;
                    Combo++;
                    break;
                case NoteType.Point:
                    pointPerfect++;
                    Combo++;
                    break;
                case NoteType.FullFlickPink:
                    fullFlickPerfect++;
                    Combo++;
                    break;
                case NoteType.FullFlickBlue:
                    fullFlickPerfect++;
                    Combo++;
                    break;
                default:
                    Debug.LogError("如果你看到这条消息,请截图并在群里@MojaveHao/Niubility748/HuaWaterED进行反馈\n" +
                        "加分出错:Perfect但未找到音符类型");
                    break;
            }
        }
    }
    [Serializable]
    public class Chapter
    {
        public string chapterName;
        public string[] musicPath;
    }
}