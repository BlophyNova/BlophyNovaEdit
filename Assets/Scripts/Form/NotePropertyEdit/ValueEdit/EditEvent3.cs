using CustomSystem;
using Data.ChartEdit;
using Data.Interface;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Event = Data.ChartEdit.Event;

namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditEvent
    {
        private void Start()
        {
            notePropertyEdit.labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged;
            LabelWindow_onWindowSizeChanged();
            startTime.onEndEdit.AddListener(value =>
            {
                Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
                if (!match.Success) return;
                //EventValueChanged(match, note.HitBeats);
                BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        events[i].startBeats = new(originEvents[i].startBeats);
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Event @event in events)
                    {
                        @event.startBeats = new(targetValue);
                    }
                }
            });
            endTime.onEndEdit.AddListener(value => 
            {
                Match match = Regex.Match(value, @"(\d+):(\d+)/(\d+)");
                if (!match.Success) return;
                //EventValueChanged(match, note.HitBeats);
                BPM targetValue = new(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                return;
                void Undo()
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        events[i].endBeats = new(originEvents[i].endBeats);
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Event @event in events)
                    {
                        @event.endBeats = new(targetValue);
                    }
                }
            });
            startValue.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value,out float result)) return;
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
                    }
                }
                void Redo()
                {
                    foreach (Data.ChartEdit.Event @event in events)
                    {
                        result = @event.eventType switch
                        {
                            Data.Enumerate.EventType.CenterX => Value16_9ToCenterXY(result, true),
                            Data.Enumerate.EventType.CenterY => Value16_9ToCenterXY(result, false),
                            Data.Enumerate.EventType.MoveX => Value16_9ToMoveXY(result, true),
                            Data.Enumerate.EventType.MoveY => Value16_9ToMoveXY(result, false),
                            Data.Enumerate.EventType.ScaleX =>  Value16_9ToScaleXY(result, true),
                            Data.Enumerate.EventType.ScaleY =>  Value16_9ToScaleXY(result, false),
                            Data.Enumerate.EventType.Alpha =>  Value0_255ToAlpha(result, true),
                            Data.Enumerate.EventType.LineAlpha =>  Value0_255ToAlpha(result, true),
                            _ => result
                        };
                        @event.startValue = result;
                    }
                }
            });
            endValue.onEndEdit.AddListener(value =>
            {
                if (!float.TryParse(value, out float result)) return;
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
                    foreach (Data.ChartEdit.Event @event in events)
                    {
                        result = @event.eventType switch
                        {
                            Data.Enumerate.EventType.CenterX => Value16_9ToCenterXY(result, true),
                            Data.Enumerate.EventType.CenterY => Value16_9ToCenterXY(result, false),
                            Data.Enumerate.EventType.MoveX => Value16_9ToMoveXY(result, true),
                            Data.Enumerate.EventType.MoveY => Value16_9ToMoveXY(result, false),
                            Data.Enumerate.EventType.ScaleX => Value16_9ToScaleXY(result, true),
                            Data.Enumerate.EventType.ScaleY => Value16_9ToScaleXY(result, false),
                            Data.Enumerate.EventType.Alpha => Value0_255ToAlpha(result, true),
                            Data.Enumerate.EventType.LineAlpha => Value0_255ToAlpha(result, true),
                            _ => result
                        };
                        @event.endValue = result;
                    }
                }
            });
            easeEdit.onValueChanged += value =>
            {
                if(value<0)return;
                Steps.Instance.Add(Undo, Redo, Finally);
                Redo();
                Finally();
                void Undo()
                {    for (int i = 0; i < events.Count; i++)
                     {
                        events[i].curveIndex = originEvents[i].curveIndex;
                     }
                }

                void Redo()
                {
                    foreach (Event @event in events)
                    {
                        @event.curveIndex = value;
                    }
                }
            };
        }

        private void LabelWindow_onWindowSizeChanged()
        {
            foreach (RectTransform content in contentList)
            {
                content.sizeDelta = new(viewport.rect.width, 50);
            }
        }

        void Finally()
        {
            GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1));
            GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1));
        }
    }
}