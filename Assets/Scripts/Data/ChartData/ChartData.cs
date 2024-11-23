using System;
using System.Collections.Generic;
using Data.Enumerate;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
namespace Data.ChartData
{

    [Serializable]
    public class ChartData
    {
        public MetaData metaData;
        public List<Box> boxes;
        public GlobalData globalData;
        public List<Text> texts;
    }

    [Serializable]
    public class MetaData
    {
        public string musicName = "";
        public string musicWriter = "";
        [FormerlySerializedAs("musicBPMText")]
        public string musicBpmText = "";
        public string chartWriter = "";
        public string artWriter = "";
        public Hard chartHard;
        public string chartLevel = "";
        public string description = "";
    }
    [Serializable]
    public class GlobalData
    {
        public float offset;
        public float musicLength;
        public int tapCount;
        public int holdCount;
        public int dragCount;
        public int flickCount;
        public int fullFlickCount;
        public int pointCount;
    }
    [Serializable]
    public class Text
    {
        public float startTime;
        public float endTime;
        public float size;
        public string text;
        public Event moveX;
        public Event moveY;

        //先放这里，画个大饼
        public List<EventString> thisEvent;
        public List<Event> positionX;
        public List<Event> positionY;
        public List<Event> spaceBetween;
        public List<Event> textSize;
        public List<Event> r;
        public List<Event> g;
        public List<Event> b;
        public List<Event> rotate;
        public List<Event> alpha;
    }
    #region 下面都是依赖
    [Serializable]
    public class Box
    {
        public BoxEvents boxEvents;
        public List<Line> lines;
    }
    [Serializable]
    public class Line
    {
        public List<Note> onlineNotes;
        //public int onlineNotesLength = -1;
        public int OnlineNotesLength => onlineNotes.Count;
        public List<Note> offlineNotes;
        //public int offlineNotesLength = -1;
        public int OfflineNotesLength => offlineNotes.Count;
        public List<Event> speed;
        public AnimationCurve far;//画布偏移绝对位置，距离
        public AnimationCurve career;//速度
    }
    [Serializable]
    public class Note
    {
        public NoteType noteType;
        public float hitTime;//打击时间
        public float holdTime;
        [JsonIgnore]
        public float HoldTime
        {
            get => holdTime == 0 ? JudgeManager.Bad : holdTime;
            set => holdTime = value;
        }
        public NoteEffect effect;
        public float positionX;
        public bool isClockwise;//是逆时针
        public bool hasOther;//还有别的Note和他在统一时间被打击，简称多押标识（（
        [JsonIgnore] public float EndTime => hitTime + HoldTime;
        [JsonIgnore] public float hitFloorPosition;//打击地板上距离
        public Note(ChartEdit.Note noteEdit)
        {
            noteType = noteEdit.noteType;
            hitTime = BPMManager.Instance.GetSecondsTimeWithBeats(noteEdit.HitBeats.ThisStartBPM);
            holdTime = BPMManager.Instance.GetSecondsTimeWithBeats(noteEdit.EndBeats.ThisStartBPM) - BPMManager.Instance.GetSecondsTimeWithBeats(noteEdit.HitBeats.ThisStartBPM);
            effect = noteEdit.effect;
            positionX = noteEdit.positionX;
            isClockwise = noteEdit.isClockwise;
            hasOther = noteEdit.hasOther;
        }
        public Note() { }
    }
    [Serializable]
    public enum NoteType
    {
        Tap = 0,
        Hold = 1,
        Drag = 2,
        Flick = 3,
        Point = 4,
        FullFlickPink = 5,
        FullFlickBlue = 6
    }
    [Flags]
    [Serializable]
    public enum NoteEffect
    {
        Ripple = 1,
        FullLine = 2,
        CommonEffect = 4
    }
    [Serializable]
    public class BoxEvents
    {
        public List<Event> moveX;
        private int lengthMoveX = -1;
        public int LengthMoveX
        {
            get
            {
                if (lengthMoveX < 0) lengthMoveX = moveX.Count;
                return lengthMoveX;
            }
        }
        public List<Event> moveY;
        private int lengthMoveY = -1;
        public int LengthMoveY
        {
            get
            {
                if (lengthMoveY < 0) lengthMoveY = moveY.Count;
                return lengthMoveY;
            }
        }
        public List<Event> rotate;
        private int lengthRotate = -1;
        public int LengthRotate
        {
            get
            {
                if (lengthRotate < 0) lengthRotate = rotate.Count;
                return lengthRotate;
            }
        }
        public List<Event> alpha;
        private int lengthAlpha = -1;
        public int LengthAlpha
        {
            get
            {
                if (lengthAlpha < 0) lengthAlpha = alpha.Count;
                return lengthAlpha;
            }
        }
        public List<Event> scaleX;
        private int lengthScaleX = -1;
        public int LengthScaleX
        {
            get
            {
                if (lengthScaleX < 0) lengthScaleX = scaleX.Count;
                return lengthScaleX;
            }
        }
        public List<Event> scaleY;
        private int lengthScaleY = -1;
        public int LengthScaleY
        {
            get
            {
                if (lengthScaleY < 0) lengthScaleY = scaleY.Count;
                return lengthScaleY;
            }
        }
        public List<Event> centerX;
        private int lengthCenterX = -1;
        public int LengthCenterX
        {
            get
            {
                if (lengthCenterX < 0) lengthCenterX = centerX.Count;
                return lengthCenterX;
            }
        }
        public List<Event> centerY;
        private int lengthCenterY = -1;
        public int LengthCenterY
        {
            get
            {
                if (lengthCenterY < 0) lengthCenterY = centerY.Count;
                return lengthCenterY;
            }
        }
        public List<Event> lineAlpha;
        private int lengthLineAlpha = -1;
        public int LengthLineAlpha
        {
            get
            {
                if (lengthLineAlpha < 0) lengthLineAlpha = lineAlpha.Count;
                return lengthLineAlpha;
            }
        }
    }
    [Serializable]
    public class Event
    {
        public Event(Data.ChartEdit.Event @event)
        {
            startTime = BPMManager.Instance.GetSecondsTimeWithBeats(@event.startBeats.ThisStartBPM);
            endTime = BPMManager.Instance.GetSecondsTimeWithBeats(@event.endBeats.ThisStartBPM);
            startValue = @event.startValue;
            endValue = @event.endValue;
            curve = @event.Curve.thisCurve;
        }
        public Event() { }
        public float startTime;
        public float endTime;
        public float startValue;
        public float endValue;
        public AnimationCurve curve;
    }
    [Serializable]
    public class EventString
    {
        public float startTime;
        public float endTime;
        public string startValue;
        public string endValue;
    }
    #endregion
}