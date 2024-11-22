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
        public float musicLength;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="integer"></param>
        /// <param name="molecule">分子</param>
        /// <param name="denominator">分母</param>

        public BPM(int integer, int molecule, int denominator)
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
            if (molecule > 0) molecule--;
            else if (molecule - 1 <= 0)
            {
                molecule = denominator - 1;
                integer--;
            }
        }
        public static BPM operator +(BPM a, BPM b)
        {
            while (b.ThisStartBPM > 0)
            {
                a.AddOneBeat();
                b.SubtractionOneBeat();
            }
            return a;
        }
        public static BPM operator -(BPM a, BPM b)
        {
            while (b.ThisStartBPM > 0)
            {
                a.SubtractionOneBeat();
                b.SubtractionOneBeat();
            }
            return a;
        }
        public static BPM operator *(BPM a, int b)
        {
            BPM res = new(a);
            res.integer *= b;
            res.molecule *= b;
            while (a.molecule >= a.denominator)
            {
                res.integer++;
                res.molecule -= a.denominator;
            }
            return res;
        }
        public static BPM Zero => new();
        public static BPM One => new(1, 0, 1);
        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj is BPM)
            {
                var my_obj = obj as BPM;
                if (Mathf.Abs(integer - my_obj.integer) > .001f) return false;
                if (Mathf.Abs(molecule - my_obj.molecule) > .001f) return false;
                if (Mathf.Abs(denominator - my_obj.denominator) > .001f) return false;
                res = true;
            }
            return res;
        }
    }
    [Serializable]
    public class Box
    {
        public BoxEvents boxEvents;
        public List<Line> lines = new();
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
        BPM hitBeats;//打击时间
        public BPM HitBeats
        {
            get => hitBeats;
            set
            {
                hitBeats = value;
                Debug.Log($@"有人在碰我敏感肌呜呜呜···");
            }
        }
        public BPM holdBeats;
        //public BPM endBeats;
        public NoteEffect effect;
        public float positionX;
        public bool isClockwise;//是逆时针
        public bool hasOther;//还有别的Note和他在统一时间被打击，简称多押标识（（
        public bool isSelected;
        [JsonIgnore]
        public BPM EndBeats
        {
            get
            {
                BPM cloneHitBeats = new(hitBeats);
                BPM cloneHoldBeats = new(holdBeats);
                return cloneHitBeats + cloneHoldBeats;
            }
        }

        [JsonIgnore] public float hitFloorPosition;//打击地板上距离
        public Note() { }
        public Note(Note note)
        {
            noteType = note.noteType;
            hitBeats = new(note.hitBeats);
            holdBeats = new(note.holdBeats);
            effect = note.effect;
            positionX = note.positionX;
            isClockwise = note.isClockwise;
            hasOther = note.hasOther;
            isSelected = note.isSelected;
        }

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
        public bool IsSelected
        {
            get
            {
                Debug.Log(@$"startBeats:{startBeats.integer}:{startBeats.molecule}/{startBeats.denominator};IsSelected：{isSelected}");
                return isSelected;
            }
            set
            {
                isSelected = value;
                Debug.Log(@$"startBeats:{startBeats.integer}:{startBeats.molecule}/{startBeats.denominator};IsSelected：{isSelected}");
            }
        }
        [SerializeField] bool isSelected;
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
            startBeats = new(@event.startBeats);
            endBeats = new(@event.endBeats);
            IsSelected = @event.IsSelected;
        }

        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj is Event)
            {
                var my_obj = obj as Event;
                if (Mathf.Abs(startBeats.ThisStartBPM - my_obj.startBeats.ThisStartBPM) > .001f) return false;
                if (Mathf.Abs(endBeats.ThisStartBPM - my_obj.endBeats.ThisStartBPM) > .001f) return false;
                if (Mathf.Abs(startValue - my_obj.startValue) > .001f) return false;
                if (Mathf.Abs(endValue - my_obj.endValue) > .001f) return false;
                res = true;
            }
            return res;
        }
    }
}