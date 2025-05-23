﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data.ChartEdit;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using UnityEngine;
using UtilityCode.Algorithm;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ChartTool.ChartTool;

namespace Form.EventEdit
{
    //这里放处理数据的方法,不负责刷新
    public partial class EventEdit
    {
        private ChartData ChartEditData => GlobalData.Instance.chartEditData;
        private Data.ChartData.ChartData ChartData => GlobalData.Instance.chartData;

        private List<Event> CopyEvents(List<Event> eventClipboard, int boxID, bool isPaste)
        {
            List<Event> newEvents = new();
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = new(eventClipboard[i]);
                //AddEvent(@event, eventClipboard.GetValue(i),boxID,isPaste);
                newEvents.Add(@event);
            }

            return newEvents;
        }

        /// <summary>
        /// </summary>
        /// <param name="eventClipboard"></param>
        /// <param name="boxID"></param>
        /// <param name="isPaste">为True的话，跳过重置数值和曲线</param>
        /// <returns></returns>
        private List<Event> AddEvents(List<Event> eventClipboard, int boxID, bool isPaste = false)
        {
            List<Event> newEvents = new();
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                AddEvent(@event, boxID, isPaste);
                newEvents.Add(@event);
            }

            onEventsAdded(newEvents);
            return newEvents;
        }

        private List<Event> DeleteEvents(List<Event> eventClipboard, int boxID, bool isCopy = false)
        {
            List<Event> deletedEvents = new();
            if (isCopy)
            {
                return deletedEvents;
            }

            for (int i = 0; i < eventClipboard.Count; i++)
            {
                DeleteEvent(eventClipboard[i], boxID);
                deletedEvents.Add(eventClipboard[i]);
            }

            onEventsDeleted(deletedEvents);
            return deletedEvents;
        }

        private void AlignEvents(List<Event> eventClipboard, BPM bpm)
        {
            BPM firstEventStartBeats = eventClipboard[0].startBeats;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                eventClipboard[i].startBeats =
                    new BPM(bpm) + (new BPM(@event.startBeats) - new BPM(firstEventStartBeats));
                eventClipboard[i].endBeats = new BPM(bpm) + (new BPM(@event.endBeats) - new BPM(firstEventStartBeats));
            }
        }

        private void BatchEvents(List<Event> events, Action<Event> action)
        {
            for (int i = 0; i < events.Count; i++)
            {
                action(events[i]);
            }
        }

        #region 增删改查

        private void AddEvent(Event @event, int boxID, bool isPaste = false)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], @event.eventType);

            int index = Algorithm.BinarySearch(events, m => m.startBeats.ThisStartBPM < @event.startBeats.ThisStartBPM,
                false);

            events.Insert(index, @event);

            if (!isPaste)
            {
                @event.startValue = @event.endValue = events[index - 1].endValue;
                //@event.Curve = GlobalData.Instance.easeData[0];
                @event.curveIndex = 0;
                @event.chartEditEvent.Init();
            }

            AddEvent2ChartData(@event, boxID);
        }

        private void DeleteEvent(Event @event, int boxID)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], @event.eventType);
            if (events.Count <= 1 || @event.disallowDelete)
            {
                Alert.EnableAlert("这个事件不允许删除了啦（小声嘀咕");
                return;
            }

            events.Remove(@event);
            eventEditItems.Remove(@event.chartEditEvent);
            if (@event.chartEditEvent != null)
            {
                Destroy(@event.chartEditEvent.gameObject);
            }

            DeleteEvent2ChartData(@event, boxID);
        }

        private int FindEventIndex(Event @event, EventType eventType, int boxID)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], eventType);
            int findIndex = events.FindIndex(m => m == @event);
            return findIndex;
        }

        private void AddEvent2Clipboard()
        {
            List<Event> events = new();
            foreach (EventEditItem selectedEventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                if (selectedEventEditItem.@event.disallowCopy)
                {
                    continue;
                }

                selectedEventEditItem.@event.disallowDelete = false;
                selectedEventEditItem.@event.disallowMove = false;
                events.Add(selectedEventEditItem.@event);
            }

            GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(events);
        }

        private void SyncScaleY(Event @event, EventType eventType, int boxID, bool isPaste)
        {
            if (eventType == EventType.ScaleX && !isPaste) //同步scaleY
            {
                Event syncSclaeY = new(@event)
                {
                    eventType = EventType.ScaleY
                };
                AddEvent(syncSclaeY, boxID, isPaste);
            }
        }

        #endregion
    }
}