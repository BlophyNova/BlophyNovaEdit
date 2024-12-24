using CustomSystem;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Controller;
using Data.ChartData;
using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.EventEdit;
using Form.LabelWindow;
using Log;
using Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Line = Data.ChartData.Line;
using Note = Data.ChartEdit.Note;

namespace Form.NotePropertyEdit
{
    public partial class NotePropertyEdit : LabelWindowContent
    {
        public delegate void OnEventValueChanged();

        public delegate void OnNoteValueChanged();

        public Event eventMemory;

        public EventEditItem @event;
        public Note note;

        public TMP_Dropdown noteType; //note
        public Toggle commonEffect; //note
        public Toggle ripple; //note
        public TMP_InputField startTime; //both
        public TMP_InputField endTime; //event、hold only
        public TMP_InputField startValue; //event
        public TMP_InputField endValue; //event
        public TMP_InputField postionX; //note
        public Toggle isClockwise; //note
        public TMP_Dropdown ease; //event

        private void Start()
        {
            noteType.onValueChanged.AddListener(NoteTypeChanged);
            commonEffect.onValueChanged.AddListener(CommonEffectChanged);
            ripple.onValueChanged.AddListener(RippleChanged);
            startValue.onEndEdit.AddListener(StartValueChanged);
            endValue.onEndEdit.AddListener(EndValueChanged);
            postionX.onEndEdit.AddListener(PositionXChanged);
            isClockwise.onValueChanged.AddListener(IsClockwiseChanged);
            ease.onValueChanged.AddListener(EaseChanged);
        }

        public event OnNoteValueChanged onNoteValueChanged = () => { };
        public event OnEventValueChanged onEventValueChanged = () => { };

        private void NoteHitBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;
            //EventValueChanged(match, note.HitBeats);
            BPM sourceValue = new(note.HitBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo);
            Redo();
            return;
            void Redo()
            {
                note.HitBeats = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                note.HitBeats = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void NoteEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;

            BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value));
            BPM sourceValue = new(note.holdBeats);
            BPM targetValue = endBeats - new BPM(note.HitBeats);

            Steps.Instance.Add(Undo, Redo);
            Redo();
            return;
            void Redo()
            {
                note.holdBeats = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                note.holdBeats = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EventStartBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;
            //EventValueChanged(match, @event.@event.startBeats);
            BPM sourceValue = new(@event.@event.startBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo);
            Redo();
            return;
            void Redo()
            {
                @event.@event.startBeats = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                @event.@event.startBeats = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EventEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;
            //EventValueChanged(match, @event.@event.endBeats);
            BPM sourceValue = new(@event.@event.endBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo);
            Redo();
            return;
            void Redo()
            {
                @event.@event.endBeats = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                @event.@event.endBeats = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        //private void EventValueChanged(Match match,BPM bpm)
        //{
        //    BPM sourceValue = new(bpm);
        //    BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
        //    Steps.Instance.Add(Undo, Redo);
        //    Redo();
        //    //LogCenter.Log($"事件EndBeats从{bpm.integer}:{bpm.molecule}/{bpm.denominator}变更为{match.Groups[1].Value}:{match.Groups[2].Value}/{match.Groups[3].Value}");
        //    //BPM endBeats = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
        //    //    int.Parse(match.Groups[3].Value));
        //    return;
        //    void Redo()
        //    {
        //        bpm = targetValue;
        //        RefreshChartPreviewAndChartEditCanvas();
        //    }
        //    void Undo()
        //    {
        //        bpm = sourceValue;
        //        RefreshChartPreviewAndChartEditCanvas();
        //    }
        //}

        private void EaseChanged(int value)
        {
            int sourceValue = @event.@event.curveIndex;
            int targetValue = value;
            Steps.Instance.Add(Undo, Redo);
            Redo();
            LogCenter.Log($"事件Ease从{@event.@event.Curve.easeType}变更为{GlobalData.Instance.easeDatas[value].easeType}");
            return;
            //@event.@event.Curve = GlobalData.Instance.easeData[value];
            void Redo()
            {
                @event.@event.curveIndex = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                @event.@event.curveIndex = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void IsClockwiseChanged(bool value)
        {
            bool sourceValue = note.isClockwise;
            bool targetValue = value;
            Steps.Instance.Add(Undo, Redo);
            Redo();
            LogCenter.Log($"音符IsClockWise从{sourceValue}变更为{targetValue}");
            return;
            void Redo()
            {
                note.isClockwise = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                note.isClockwise = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void PositionXChanged(string value)
        {
            if (!float.TryParse(value, out float result))return;

            float sourceValue = note.positionX;
            float targetValue = result;
            Steps.Instance.Add(Undo, Redo);
            Redo();
            LogCenter.Log($"音符PositionX从{note.positionX}变更为{value}");
            return;
            void Redo()
            {
                note.positionX = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                note.positionX = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void EndValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;

            float sourceValue = @event.@event.endValue;
            float targetValue = result;
            Steps.Instance.Add(Undo,Redo);
            Redo();
            LogCenter.Log($"事件EndValue从{@event.@event.endValue}变更为{result}");
            return;
            void Redo()
            {
                @event.@event.endValue = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                @event.@event.endValue = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void StartValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;

            float sourceValue= @event.@event.startValue;
            float targetValue=result;
            Steps.Instance.Add(Undo, Redo);
            Redo();
            LogCenter.Log($"事件StartValue从{@event.@event.startValue}变更为{result}");
            return;
            void Redo()
            {
                @event.@event.startValue = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Undo()
            {
                @event.@event.startValue = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void RippleChanged(bool value)
        {
            AddOrSubtractionNoteEffect(value, NoteEffect.Ripple);
        }

        private void CommonEffectChanged(bool value)
        {
            AddOrSubtractionNoteEffect(value, NoteEffect.CommonEffect);
        }

        private void AddOrSubtractionNoteEffect(bool value, NoteEffect noteEffect)
        {
            NoteEffect sourceValue = note.effect;
            NoteEffect targetValue = value switch
            {
                true => note.effect | noteEffect,
                false => note.effect ^ noteEffect
            };
            Steps.Instance.Add(Undo, Redo);
            Redo();
            LogCenter.Log($"成功{value switch { true => "添加", false => "取消" }}{noteEffect}特效");
            return;
            void Undo()
            {
                note.effect = sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
            void Redo()
            {
                note.effect = targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        private void NoteTypeChanged(int value)
        {
            int sourceValue = (int)note.noteType;
            int targetValue = value;
            LogCenter.Log($"音符类型从{note.noteType}变更为{(NoteType)value}");
            Steps.Instance.Add(Undo, Redo);
            Redo();
            return;

            //这里不能把两个方法的RefreshChartPreviewAndChartEditCanvas方法提出来，因为后面进行撤销和重做的操作是需要执行的
            void Undo()
            {
                note.noteType = (NoteType)sourceValue;
                RefreshChartPreviewAndChartEditCanvas();
            }

            void Redo()
            {
                note.noteType = (NoteType)targetValue;
                RefreshChartPreviewAndChartEditCanvas();
            }
        }

        public void SelectedNote(Scenes.Edit.NoteEdit note)
        {
            SelectedNote(note.thisNoteData);
        }
    }
}