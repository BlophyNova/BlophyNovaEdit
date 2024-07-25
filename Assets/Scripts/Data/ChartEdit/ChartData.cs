using Newtonsoft.Json;
using Scenes.DontDestoryOnLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data.ChartEdit
{
    [Serializable]
    public class ChartData
    {
        public float yScale;
        public int beatSubdivision;//节拍线细分(单位：份)
        public int verticalSubdivision;//垂直线细分(单位：份)
        public int eventVerticalSubdivision;
        public List<BPM> bpmList = new(); 
        public Box[] boxes;
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
    }
    [Serializable]
    public class Box
    {
        public BoxEvents boxEvents;
        public Line[] lines;
    }
    [Serializable]
    public class Line
    {
        public Note[] onlineNotes;
        public int onlineNotesLength = -1;
        public int OnlineNotesLength
        {
            get
            {
                if (onlineNotesLength < 0) onlineNotesLength = onlineNotes.Length;
                return onlineNotesLength;
            }
        }
        public Note[] offlineNotes;
        public int offlineNotesLength = -1;
        public int OfflineNotesLength
        {
            get
            {
                if (offlineNotesLength < 0) offlineNotesLength = offlineNotes.Length;
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
        [JsonIgnore] public BPM EndTime => hitBeats + HoldBeats;
        [JsonIgnore] public float hitFloorPosition;//打击地板上距离
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
        public Event[] moveX;
        private int lengthMoveX = -1;
        public int LengthMoveX
        {
            get
            {
                if (lengthMoveX < 0) lengthMoveX = moveX.Length;
                return lengthMoveX;
            }
        }
        public Event[] moveY;
        private int lengthMoveY = -1;
        public int LengthMoveY
        {
            get
            {
                if (lengthMoveY < 0) lengthMoveY = moveY.Length;
                return lengthMoveY;
            }
        }
        public Event[] rotate;
        private int lengthRotate = -1;
        public int LengthRotate
        {
            get
            {
                if (lengthRotate < 0) lengthRotate = rotate.Length;
                return lengthRotate;
            }
        }
        public Event[] alpha;
        private int lengthAlpha = -1;
        public int LengthAlpha
        {
            get
            {
                if (lengthAlpha < 0) lengthAlpha = alpha.Length;
                return lengthAlpha;
            }
        }
        public Event[] scaleX;
        private int lengthScaleX = -1;
        public int LengthScaleX
        {
            get
            {
                if (lengthScaleX < 0) lengthScaleX = scaleX.Length;
                return lengthScaleX;
            }
        }
        public Event[] scaleY;
        private int lengthScaleY = -1;
        public int LengthScaleY
        {
            get
            {
                if (lengthScaleY < 0) lengthScaleY = scaleY.Length;
                return lengthScaleY;
            }
        }
        public Event[] centerX;
        private int lengthCenterX = -1;
        public int LengthCenterX
        {
            get
            {
                if (lengthCenterX < 0) lengthCenterX = centerX.Length;
                return lengthCenterX;
            }
        }
        public Event[] centerY;
        private int lengthCenterY = -1;
        public int LengthCenterY
        {
            get
            {
                if (lengthCenterY < 0) lengthCenterY = centerY.Length;
                return lengthCenterY;
            }
        }
        public Event[] lineAlpha;
        private int lengthLineAlpha = -1;
        public int LengthLineAlpha
        {
            get
            {
                if (lengthLineAlpha < 0) lengthLineAlpha = lineAlpha.Length;
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
        public AnimationCurve curve;
    }
}