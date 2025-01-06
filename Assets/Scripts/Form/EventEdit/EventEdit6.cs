using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomSystem;
using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Form.NoteEdit;
using Form.PropertyEdit;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Algorithm;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;

namespace Form.EventEdit
{
    //这里放处理数据的方法
    public partial class EventEdit
    {
        private void DeleteEvent(EventEditItem eventEditItem,int boxID)
        {
            List<Event> events = FindEditEventListByEventType(eventEditItem.eventType, boxID); 
            events.Remove(eventEditItem.@event);
            onEventDeleted(eventEditItem);
        }
        private void DeleteEvent(Event @event,EventType eventType)
        {
            List<Event> events = FindEditEventListByEventType(eventType, currentBoxID);
            events.Remove(@event);
        }
        private void AddEvent(EventEditItem eventEditItem)
        {
            AddEvent(eventEditItem.@event, eventEditItem.eventType);
        }

        private void AddEvent(Data.ChartEdit.Event @event, EventType eventType, bool isPaste = false)
        {
            List<Event> events = FindEditEventListByEventType(eventType, currentBoxID);
            if (!isPaste)
            {
                @event.startValue = @event.endValue = events[^1].endValue;
                //@event.Curve = GlobalData.Instance.easeData[0];
                @event.curveIndex = 0;
            }

            events.Add(@event);
            Algorithm.BubbleSort(events, (a, b) => //排序
            {
                if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM) return 1;
                if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM) return -1;
                return 0;
            });
            List<Data.ChartData.Event> chartDataEvents = FindPlayerEventListByEventType(eventType, currentBoxID);
            if (chartDataEvents == null)
            {
                SpeedEvent(eventType);
                return;
            }

            ChartTool.InsertEditEvent2PlayerEvent(chartDataEvents, @event);

            SyncScaleY(@event, eventType, isPaste);
            //Debug.LogError("错误记忆");
        }

        private void SyncScaleY(Event @event, EventType eventType, bool isPaste)
        {
            if (eventType == EventType.ScaleX && !isPaste) //同步scaleY
            {
                List<Event> scaleYEvents =
                    GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY;
                scaleYEvents.Add(new Event(@event));
                Algorithm.BubbleSort(scaleYEvents, (a, b) => //排序
                {
                    if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM) return 1;
                    if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM) return -1;
                    return 0;
                });
                ChartTool.InsertEditEvent2PlayerEvent(
                    GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
                    new Event(@event));
                RefreshEditEvents(-1);
            }
        }

        private List<Data.ChartData.Event> FindPlayerEventListByEventType(EventType eventType, int boxID)
        {
            return eventType switch
            {
                EventType.CenterX => GlobalData.Instance.chartData.boxes[boxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartData.boxes[boxID].boxEvents.centerY,
                EventType.MoveX => GlobalData.Instance.chartData.boxes[boxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartData.boxes[boxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartData.boxes[boxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartData.boxes[boxID].boxEvents.scaleY,
                EventType.Rotate => GlobalData.Instance.chartData.boxes[boxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartData.boxes[boxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartData.boxes[boxID].boxEvents.lineAlpha,
                _ => null
            };
        }

        private List<Event> FindEditEventListByEventType(EventType eventType,int boxID)
        {
            return eventType switch
            {
                EventType.Speed => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.speed,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.centerY,
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.scaleY,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[boxID].boxEvents.lineAlpha,
                _ => null
            };
        }

        private void AddEvent2EventClipboard()
        {
            eventClipboard.Clear();
            foreach (EventEditItem selectedEventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                eventClipboard.Add(selectedEventEditItem);
            }
            LogCenter.Log($@"已将{eventClipboard.Count}个事件发送至剪切板！");
        }
        private void SpeedEvent(EventType eventType)
        {
            #region 以下代码为speed事件处理相关专属代码，没啥bug的情况下一个字都别改

            if (eventType != EventType.Speed)
            {
                return;
            }

            List<Data.ChartEdit.Event> filledVoid =
                GameUtility.FillVoid(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed);
            for (int i = 0; i < GlobalData.Instance.chartData.boxes[currentBoxID].lines.Count; i++)
            {
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed = new List<Data.ChartData.Event>();


                ChartTool.ForeachBoxEvents(filledVoid,
                    GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed);
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career = new AnimationCurve
                { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career.keys = GameUtility
                    .CalculatedSpeedCurve(GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed.ToArray())
                    .ToArray();


                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far = new AnimationCurve
                { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far.keys =
                    GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
            }

            #endregion
        }

        private KeyValueList<Event,EventType> InstNewEvents(List<EventEditItem> eventClipboard, BPM beatLineBpm)
        {
            KeyValueList<Event, EventType> newEvents = new();
            BPM firstEventStartBeats = eventClipboard[0].@event.startBeats;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                EventEditItem @event = eventClipboard[i];
                Event copyNewEvent = new(@event.@event)
                {
                    startBeats = new BPM(beatLineBpm) + (new BPM(@event.@event.startBeats) - new BPM(firstEventStartBeats)),
                    endBeats = new BPM(beatLineBpm) + (new BPM(@event.@event.endBeats) - new BPM(firstEventStartBeats))
                };
                if (isCopy)
                {
                    copyNewEvent.IsSelected = false;
                }
                newEvents.Add(copyNewEvent,@event.eventType);
                //AddEventAndRefresh(copyNewEvent, currentBoxID);
                AddEvent(copyNewEvent, @event.eventType, true);
            }
            return newEvents;
        }
        private void InstNewEvents(KeyValueList<Event, EventType> eventClipboard, BPM beatLineBpm)
        {
            BPM firstEventStartBeats = eventClipboard[0].startBeats;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                Event copyNewEvent = new(@event)
                {
                    startBeats = new BPM(beatLineBpm) + (new BPM(@event.startBeats) - new BPM(firstEventStartBeats)),
                    endBeats = new BPM(beatLineBpm) + (new BPM(@event.endBeats) - new BPM(firstEventStartBeats))
                };
                if (isCopy)
                {
                    copyNewEvent.IsSelected = false;
                }
                //AddEventAndRefresh(copyNewEvent, currentBoxID);
                AddEvent(copyNewEvent, eventClipboard.GetValue(i), true);
            }
        }
    }
}