using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CustomSystem;
using CustomSystem.Steps;
using Data.ChartEdit;
using Data.Interface;
using NCalc;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
using Event = Data.ChartEdit.Event;
using static UtilityCode.ValueConvert.ValueConvert;
using EventType = Data.Enumerate.EventType;

namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditEvent
    {
        private void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged;
            LabelWindow_onWindowSizeChanged();
            startTime.onEndEdit.AddListener(StartTimeChanged);
            endTime.onEndEdit.AddListener(EndTimeChanged);
            startValue.onEndEdit.AddListener(StartValueChanged);
            endValue.onEndEdit.AddListener(EndValueChanged);
            easeEdit.onValueChanged += EaseChanged;
            easeEdit.onCustomValueChanged += CustomEaseChanged;
            syncEvent.onValueChanged.AddListener(IsSyncEventChanged);
        }

        private void IsSyncEventChanged(bool value)
        {
            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].isSyncEvent = originEvents[i].isSyncEvent;
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    @event.isSyncEvent = value;
                    @event.endValue = @event.startValue;
                }
            }
        }

        private void CustomEaseChanged(int value)
        {
            if (value < 0)
            {
                return;
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            if (GlobalData.Instance.chartEditData.customCurves[value - 1].isDeleted)
            {
                Alert.EnableAlert("这个缓动被删除啦，不要再选择啦～");
                return;
            }

            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].curveIndex = originEvents[i].curveIndex;
                    events[i].isCustomCurve = originEvents[i].isCustomCurve;
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    @event.isCustomCurve = true;
                    @event.curveIndex = value - 1;
                }
            }
        }

        private void EaseChanged(int value)
        {
            if (value < 0)
            {
                return;
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].curveIndex = originEvents[i].curveIndex;
                    events[i].isCustomCurve = originEvents[i].isCustomCurve;
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    @event.isCustomCurve = false;
                    @event.curveIndex = value;
                }
            }
        }

        private void EndValueChanged(string value)
        {
            if (!float.TryParse(value, out float result))
            {
                Expression expression = new(value);
                try
                {
                    result = float.Parse($"{expression.Evaluate()}");
                }
                catch
                {
                    return;
                }
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            //EventValueChanged(match, note.HitBeats);
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].endValue = originEvents[i].endValue;
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    result = @event.eventType switch
                    {
                        EventType.CenterX => Value16_9ToCenterXY(result, true),
                        EventType.CenterY => Value16_9ToCenterXY(result, false),
                        EventType.MoveX => Value16_9ToMoveXY(result, true),
                        EventType.MoveY => Value16_9ToMoveXY(result, false),
                        EventType.ScaleX => Value16_9ToScaleXY(result, true),
                        EventType.ScaleY => Value16_9ToScaleXY(result, false),
                        EventType.Alpha => Value0_255ToAlpha(result, true),
                        EventType.LineAlpha => Value0_255ToAlpha(result, true),
                        _ => result
                    };
                    @event.endValue = result;
                }
            }
        }

        private void StartValueChanged(string value)
        {
            if (!float.TryParse(value, out float result))
            {
                Expression expression = new(value);
                try
                {
                    result = float.Parse($"{expression.Evaluate()}");
                }
                catch
                {
                    return;
                }
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            //EventValueChanged(match, note.HitBeats);
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].startValue = originEvents[i].startValue;
                    if (events[i].isSyncEvent)
                    {
                        events[i].endValue = originEvents[i].endValue;
                    }
                }
            }

            void Redo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    result = events[i].eventType switch
                    {
                        EventType.CenterX => Value16_9ToCenterXY(result, true),
                        EventType.CenterY => Value16_9ToCenterXY(result, false),
                        EventType.MoveX => Value16_9ToMoveXY(result, true),
                        EventType.MoveY => Value16_9ToMoveXY(result, false),
                        EventType.ScaleX => Value16_9ToScaleXY(result, true),
                        EventType.ScaleY => Value16_9ToScaleXY(result, false),
                        EventType.Alpha => Value0_255ToAlpha(result, true),
                        EventType.LineAlpha => Value0_255ToAlpha(result, true),
                        _ => result
                    };
                    events[i].startValue = result;
                    if (events[i].isSyncEvent)
                    {
                        events[i].endValue = result;
                    }
                }
            }
        }

        private void EndTimeChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success)
            {
                return;
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            //EventValueChanged(match, note.HitBeats);
            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].endBeats = new BPM(originEvents[i].endBeats);
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    @event.endBeats = new BPM(targetValue);
                }
            }
        }

        private void StartTimeChanged(string value)
        {
            Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
            if (!match.Success)
            {
                return;
            }

            List<Event> originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new Event(@event));
            }

            BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value));
            Steps.Instance.Add(Undo, Redo, Finally);
            Redo();
            Finally();
            return;

            void Undo()
            {
                for (int i = 0; i < events.Count; i++)
                {
                    events[i].startBeats = new BPM(originEvents[i].startBeats);
                }
            }

            void Redo()
            {
                foreach (Event @event in events)
                {
                    @event.startBeats = new BPM(targetValue);
                }
            }
        }

        private void LabelWindow_onWindowSizeChanged()
        {
            foreach (RectTransform content in contentList)
            {
                content.sizeDelta = new Vector2(viewport.rect.width, 50);
            }
        }

        public void Finally()
        {
            GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1),
                new List<Type> { typeof(EventEdit.EventEdit) });
            GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), null);
        }
    }
}