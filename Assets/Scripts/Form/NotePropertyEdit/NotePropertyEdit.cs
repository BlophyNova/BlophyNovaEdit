using CustomSystem;
using Data.ChartData;
using Data.ChartEdit;
using Form.EventEdit;
using Form.LabelWindow;
using Log;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;
using Event = Data.ChartEdit.Event;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
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
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            return;
            void Redo()
            {
                note.HitBeats = targetValue;
            }
            void Undo()
            {
                note.HitBeats = sourceValue;
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

            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            return;
            void Redo()
            {
                note.holdBeats = targetValue;
            }
            void Undo()
            {
                note.holdBeats = sourceValue;
            }
        }

        private void EventStartBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;
            //EventValueChanged(match, @event.@event.startBeats);
            BPM sourceValue = new(@event.@event.startBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            return;
            void Redo()
            {
                @event.@event.startBeats = targetValue;
            }
            void Undo()
            {
                @event.@event.startBeats = sourceValue;
            }
        }

        private void EventEndBeatsChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success) return;
            //EventValueChanged(match, @event.@event.endBeats);
            BPM sourceValue = new(@event.@event.endBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            return;
            void Redo()
            {
                @event.@event.endBeats = targetValue;
            }
            void Undo()
            {
                @event.@event.endBeats = sourceValue;
            }
        }
        private void EaseChanged(int value)
        {
            int sourceValue = @event.@event.curveIndex;
            int targetValue = value;
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"事件Ease从{@event.@event.Curve.easeType}变更为{GlobalData.Instance.easeDatas[value].easeType}");
            return;
            //@event.@event.Curve = GlobalData.Instance.easeData[value];
            void Redo()
            {
                @event.@event.curveIndex = targetValue;
            }
            void Undo()
            {
                @event.@event.curveIndex = sourceValue;
            }
        }

        private void IsClockwiseChanged(bool value)
        {
            bool sourceValue = note.isClockwise;
            bool targetValue = value;
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"音符IsClockWise从{sourceValue}变更为{targetValue}");
            return;
            void Redo()
            {
                note.isClockwise = targetValue;
            }
            void Undo()
            {
                note.isClockwise = sourceValue;
            }
        }

        private void PositionXChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;

            float sourceValue = note.positionX;
            float targetValue = result;
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"音符PositionX从{note.positionX}变更为{value}");
            return;
            void Redo()
            {
                note.positionX = targetValue;
            }
            void Undo()
            {
                note.positionX = sourceValue;
            }
        }

        private void EndValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            Func<float, float> func = @event.eventType switch
            {
                Data.Enumerate.EventType.CenterX => value => Value16_9ToCenterXY(value, true),
                Data.Enumerate.EventType.CenterY => value => Value16_9ToCenterXY(value, false),
                Data.Enumerate.EventType.MoveX => value => Value16_9ToMoveXY(value, true),
                Data.Enumerate.EventType.MoveY => value => Value16_9ToMoveXY(value, false),
                Data.Enumerate.EventType.ScaleX => value => Value16_9ToScaleXY(value, true),
                Data.Enumerate.EventType.ScaleY => value => Value16_9ToScaleXY(value, false),
                Data.Enumerate.EventType.Alpha => value => Value0_255ToAlpha(value, true),
                Data.Enumerate.EventType.LineAlpha => value => Value0_255ToAlpha(value, true),
                _ => value => value
            };
            float sourceValue = @event.@event.endValue;
            float targetValue = func(result);
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"事件EndValue从{@event.@event.endValue}变更为{result}");
            return;
            void Redo()
            {
                @event.@event.endValue = targetValue;
            }
            void Undo()
            {
                @event.@event.endValue = sourceValue;
            }
        }

        private void StartValueChanged(string value)
        {
            if (!float.TryParse(value, out float result)) return;
            Func<float, float> func = @event.eventType switch 
            {
                Data.Enumerate.EventType.CenterX=>value =>Value16_9ToCenterXY(value,true),
                Data.Enumerate.EventType.CenterY=>value =>Value16_9ToCenterXY(value,false),
                Data.Enumerate.EventType.MoveX=>value =>Value16_9ToMoveXY(value,true),
                Data.Enumerate.EventType.MoveY=>value => Value16_9ToMoveXY(value,false),
                Data.Enumerate.EventType.ScaleX=>value =>Value16_9ToScaleXY(value,true),
                Data.Enumerate.EventType.ScaleY=>value =>Value16_9ToScaleXY(value,false),
                _=>value=>value
            };
            float sourceValue = @event.@event.startValue;
            float targetValue = func(result);
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"事件StartValue从{@event.@event.startValue}变更为{result}");
            return;
            void Redo()
            {
                @event.@event.startValue = targetValue;
            }
            void Undo()
            {
                @event.@event.startValue = sourceValue;
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
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            LogCenter.Log($"成功{value switch { true => "添加", false => "取消" }}{noteEffect}特效");
            return;
            void Undo()
            {
                note.effect = sourceValue;
            }
            void Redo()
            {
                note.effect = targetValue;
            }
        }

        private void NoteTypeChanged(int value)
        {
            int sourceValue = (int)note.noteType;
            int targetValue = value;
            LogCenter.Log($"音符类型从{note.noteType}变更为{(NoteType)value}");
            Steps.Instance.Add(Undo, Redo, RefreshChartPreviewAndChartEditCanvas);
            Redo();
            RefreshChartPreviewAndChartEditCanvas();
            return;

            //这里不能把两个方法的RefreshChartPreviewAndChartEditCanvas方法提出来，因为后面进行撤销和重做的操作是需要执行的
            void Undo()
            {
                note.noteType = (NoteType)sourceValue;
            }

            void Redo()
            {
                note.noteType = (NoteType)targetValue;
            }
        }
        private float CenterXYSnapTo16_9(float value,bool isX)
        {
            if (isX)
            {
                return 1600f * value - 800;
            }
            else
            {
                return 900f * value - 450;
            }
        }
        private float MoveXYSnapTo16_9(float value, bool isX) 
        {
            return value * 100f;
        }
        private float ScaleXYSnapTo16_9(float value, bool isX) 
        {
            return value * 200f;
        }
        private float AlphaSnapTo0_255(float value, bool isX)
        {
            return value * 255f;
        }
        float Value16_9ToCenterXY(float value, bool isX)
        {
            if (isX)
            {
                return (value + 800f) / 1600f;
            }
            else
            {
                return (value + 450f) / 900f;
            }
        }
        float Value16_9ToMoveXY(float value, bool isX)
        {
            return value / 100f;
        }
        float Value16_9ToScaleXY(float value, bool isX)
        {
            return value / 200f;
        }
        float Value0_255ToAlpha(float value, bool isX)
        {
            return value / 255f;
        }

    }
}