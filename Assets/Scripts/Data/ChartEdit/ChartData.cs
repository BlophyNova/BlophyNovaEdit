using Data.ChartData;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

namespace Data.ChartEdit
{
    [Serializable]
    public class ChartData
    {
        public float yScale;
        public int beatSubdivision;//节拍线细分(单位：份)
        public int verticalSubdivision;//垂直线细分(单位：份)
        public int eventVerticalSubdivision;
        public float playSpeed;
        public float offset;
        public bool loopPlayBack;
        public List<BPM> bpmList;
        public List<Box> boxes;
    }
    [Serializable]
    public class BPM
    {
        public int integer = 0;
        /// <summary>
        /// 分子
        /// </summary>
        public int molecule = 0;
        /// <summary>
        /// 分母
        /// </summary>
        public int denominator = 1;
        /// <summary>
        /// 当前BPM的开始或者说上一个BPM的结束拍
        /// </summary>
        public float ThisStartBPM => integer + molecule / (float)denominator;
        public float currentBPM;
        public BPM() { }

        public BPM(int integer,int molecule,int denominator) 
        { 
            this.integer = integer;
            this.molecule = molecule;
            this.denominator = denominator;
        }
        public BPM(BPM bpm)
        {
            this.molecule = bpm.molecule;
            this.denominator = bpm.denominator;
            this.integer = bpm.integer;
            this.currentBPM = bpm.currentBPM;
        }
        public void AddOneBeat()
        {
            denominator = GlobalData.Instance.chartEditData.beatSubdivision;
            if (molecule < denominator - 1) molecule++;
            else if (molecule + 1 >= denominator)
            {
                molecule = 0;
                integer++;
            }
        }
        public void SubtractionOneBeat()
        {
            denominator = GlobalData.Instance.chartEditData.beatSubdivision;
            if (molecule >0) molecule--;
            else if (molecule - 1 <=0)
            {
                molecule = denominator-1;
                integer--;
            }
        }
        public static BPM operator+(BPM a,BPM b)
        {
            do
            {
                a.AddOneBeat();
                b.SubtractionOneBeat();
            } while (b.ThisStartBPM>0);
            return a;
        }
        public static BPM operator -(BPM a, BPM b)
        {
            do
            {
                a.SubtractionOneBeat();
                b.SubtractionOneBeat();
            } while (b.ThisStartBPM > 0);
            return a;
        }

        public static BPM Zero => new();
        public static BPM One => new(1, 0, 1);
    }
    [Serializable]
    public class Box
    {
        public BoxEvents boxEvents;
        public List<Line> lines=new();
    }
    [Serializable]
    public class Line
    {
        public List<Note> onlineNotes;
        public int onlineNotesLength = -1;
        public int OnlineNotesLength
        {
            get
            {
                if (onlineNotesLength < 0) onlineNotesLength = onlineNotes.Count;
                return onlineNotesLength;
            }
        }
        public List<Note> offlineNotes;
        public int offlineNotesLength = -1;
        public int OfflineNotesLength
        {
            get
            {
                if (offlineNotesLength < 0) offlineNotesLength = offlineNotes.Count;
                return offlineNotesLength;
            }
        }
    }
    [Serializable]
    public class Note
    {
        public NoteType noteType;
        public BPM hitBeats;//打击时间
        public BPM holdBeats;
        [JsonIgnore]
        public BPM HoldBeats
        {
            get => holdBeats;
            set => holdBeats = value;
        }
        public NoteEffect effect;
        public float positionX;
        public bool isClockwise;//是逆时针
        public bool hasOther;//还有别的Note和他在统一时间被打击，简称多押标识（（
        [JsonIgnore] public BPM EndBeats => hitBeats + HoldBeats;
        [JsonIgnore] public float hitFloorPosition;//打击地板上距离
    }
    [Serializable]
    public class BoxEvents
    {

        public List<Event> speed;
        private int lengthSpeed = -1;
        public int LengthSpeed
        {
            get
            {
                if (lengthSpeed < 0) lengthSpeed = speed.Count;
                return lengthSpeed;
            }
        }
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
        public BPM startBeats;
        public BPM endBeats;
        public float startValue;
        public float endValue;
        public EaseData curve;
        public Event() { }
        public Event(Event @event)
        {
            endValue = @event.endValue;
            startValue = @event.startValue;
            curve = @event.curve;
            startBeats= @event.startBeats;
            endBeats= @event.endBeats;
        }
    }
}