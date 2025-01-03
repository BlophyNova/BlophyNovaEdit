using System;
using System.Collections.Generic;
using Data.ChartData;
using Newtonsoft.Json;
using UnityEngine;
using UtilityCode.Algorithm;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

namespace Data.ChartEdit
{
    [Serializable]
    public class ChartData
    {
        public float yScale;
        public int beatSubdivision; //节拍线细分(单位：份)
        public int verticalSubdivision; //垂直线细分(单位：份)
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
        public int integer;

        /// <summary>
        ///     分子
        /// </summary>
        public int molecule;

        /// <summary>
        ///     分母
        /// </summary>
        public int denominator = 1;

        public float currentBPM;

        public BPM()
        {
        }

        /// <summary>
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
            molecule = bpm.molecule;
            denominator = bpm.denominator;
            integer = bpm.integer;
            currentBPM = bpm.currentBPM;
        }

        /// <summary>
        ///     当前BPM的开始或者说上一个BPM的结束拍
        /// </summary>
        public float ThisStartBPM => integer + molecule / (float)denominator;

        public static BPM Zero => new();
        public static BPM One => new(1, 0, 1);

        public void AddOneBeat(int beatSubdivision=-1)
        {
            denominator =GlobalData.Instance.chartEditData.beatSubdivision;
            if (molecule < denominator - 1)
            {
                molecule++;
            }
            else if (molecule + 1 >= denominator)
            {
                molecule = 0;
                integer++;
            }
        }

        public void SubtractionOneBeat(int beatSubdivision=-1)
        {
            denominator = beatSubdivision<=0? GlobalData.Instance.chartEditData.beatSubdivision:beatSubdivision;
            if (molecule > 0)
            {
                molecule--;
            }
            else if (molecule - 1 <= 0)
            {
                molecule = denominator - 1;
                integer--;
            }
        }

        public static BPM operator +(BPM a, BPM b)
        {
            BPM _a = new(a);
            BPM _b = new(b);
            int _c = Algorithm.GetLeastCommonMutiple(_a.denominator, _b.denominator);
            _a.denominator = _c;
            _b.denominator = _c;
            _a.molecule *= _c / _a.denominator;
            _b.molecule *= _c / _b.denominator;
            while (_b.ThisStartBPM > 0)
            {
                _a.AddOneBeat();
                _b.SubtractionOneBeat();
            }

            return _a;
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

        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj is BPM)
            {
                BPM my_obj = obj as BPM;
                if (Mathf.Abs(integer - my_obj.integer) > .001f)
                {
                    return false;
                }

                if (Mathf.Abs(molecule - my_obj.molecule) > .001f)
                {
                    return false;
                }

                if (Mathf.Abs(denominator - my_obj.denominator) > .001f)
                {
                    return false;
                }

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
        public List<Note> offlineNotes;
        public int offlineNotesLength = -1;

        public int OnlineNotesLength
        {
            get
            {
                if (onlineNotesLength < 0)
                {
                    onlineNotesLength = onlineNotes.Count;
                }

                return onlineNotesLength;
            }
        }

        public int OfflineNotesLength
        {
            get
            {
                if (offlineNotesLength < 0)
                {
                    offlineNotesLength = offlineNotes.Count;
                }

                return offlineNotesLength;
            }
        }
    }

    [Serializable]
    public class Note
    {
        public NoteType noteType;

        public BPM holdBeats;

        //public BPM endBeats;
        public NoteEffect effect;
        public float positionX;
        public bool isClockwise; //是逆时针
        public bool hasOther; //还有别的Note和他在统一时间被打击，简称多押标识（（
        public bool isSelected;

        [JsonIgnore] public float hitFloorPosition; //打击地板上距离
        private BPM hitBeats; //打击时间

        public Note()
        {
        }

        public Note(Note note)
        {
            noteType = note.noteType;
            hitBeats = new BPM(note.hitBeats);
            holdBeats = new BPM(note.holdBeats);
            effect = note.effect;
            positionX = note.positionX;
            isClockwise = note.isClockwise;
            hasOther = note.hasOther;
            isSelected = note.isSelected;
        }

        public BPM HitBeats
        {
            get => hitBeats;
            set=>hitBeats = value;
        }

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
    }

    [Serializable]
    public class BoxEvents
    {
        public List<Event> speed;
        public List<Event> moveX;
        public List<Event> moveY;
        public List<Event> rotate;
        public List<Event> alpha;
        public List<Event> scaleX;
        public List<Event> scaleY;
        public List<Event> centerX;
        public List<Event> centerY;
        public List<Event> lineAlpha;
        private int lengthAlpha = -1;
        private int lengthCenterX = -1;
        private int lengthCenterY = -1;
        private int lengthLineAlpha = -1;
        private int lengthMoveX = -1;
        private int lengthMoveY = -1;
        private int lengthRotate = -1;
        private int lengthScaleX = -1;
        private int lengthScaleY = -1;
        private int lengthSpeed = -1;

        public int LengthSpeed
        {
            get
            {
                if (lengthSpeed < 0)
                {
                    lengthSpeed = speed.Count;
                }

                return lengthSpeed;
            }
        }

        public int LengthMoveX
        {
            get
            {
                if (lengthMoveX < 0)
                {
                    lengthMoveX = moveX.Count;
                }

                return lengthMoveX;
            }
        }

        public int LengthMoveY
        {
            get
            {
                if (lengthMoveY < 0)
                {
                    lengthMoveY = moveY.Count;
                }

                return lengthMoveY;
            }
        }

        public int LengthRotate
        {
            get
            {
                if (lengthRotate < 0)
                {
                    lengthRotate = rotate.Count;
                }

                return lengthRotate;
            }
        }

        public int LengthAlpha
        {
            get
            {
                if (lengthAlpha < 0)
                {
                    lengthAlpha = alpha.Count;
                }

                return lengthAlpha;
            }
        }

        public int LengthScaleX
        {
            get
            {
                if (lengthScaleX < 0)
                {
                    lengthScaleX = scaleX.Count;
                }

                return lengthScaleX;
            }
        }

        public int LengthScaleY
        {
            get
            {
                if (lengthScaleY < 0)
                {
                    lengthScaleY = scaleY.Count;
                }

                return lengthScaleY;
            }
        }

        public int LengthCenterX
        {
            get
            {
                if (lengthCenterX < 0)
                {
                    lengthCenterX = centerX.Count;
                }

                return lengthCenterX;
            }
        }

        public int LengthCenterY
        {
            get
            {
                if (lengthCenterY < 0)
                {
                    lengthCenterY = centerY.Count;
                }

                return lengthCenterY;
            }
        }

        public int LengthLineAlpha
        {
            get
            {
                if (lengthLineAlpha < 0)
                {
                    lengthLineAlpha = lineAlpha.Count;
                }

                return lengthLineAlpha;
            }
        }
    }

    [Serializable]
    public class Event
    {
        [SerializeField] private bool isSelected;
        public BPM startBeats;
        public BPM endBeats;
        public float startValue;
        public float endValue;
        public int curveIndex;

        public Event()
        {
        }

        public Event(Event @event)
        {
            endValue = @event.endValue;
            startValue = @event.startValue;
            //Curve = @event.Curve;
            curveIndex = @event.curveIndex;
            startBeats = new BPM(@event.startBeats);
            endBeats = new BPM(@event.endBeats);
            IsSelected = @event.IsSelected;
        }

        public bool IsSelected
        {
            get=>isSelected;
            set=>isSelected = value;
        }

        //public EaseData curve;
        [JsonIgnore] public EaseData.EaseData Curve => GlobalData.Instance.easeDatas[curveIndex];

        public override bool Equals(object obj)
        {
            bool res = false;
            if (obj is Event)
            {
                Event my_obj = obj as Event;
                if (Mathf.Abs(startBeats.ThisStartBPM - my_obj.startBeats.ThisStartBPM) > .001f)
                {
                    return false;
                }

                if (Mathf.Abs(endBeats.ThisStartBPM - my_obj.endBeats.ThisStartBPM) > .001f)
                {
                    return false;
                }

                if (Mathf.Abs(startValue - my_obj.startValue) > .001f)
                {
                    return false;
                }

                if (Mathf.Abs(endValue - my_obj.endValue) > .001f)
                {
                    return false;
                }

                res = true;
            }

            return res;
        }
    }
}