using Data.ChartEdit;
using Log;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ChartTool.ChartTool;
using System;
namespace Form.EventEdit
{
    //这里放处理数据的方法,不负责刷新以及为ChartData加入数据
    public partial class EventEdit
    {
        ChartData ChartEditData => GlobalData.Instance.chartEditData;
        Data.ChartData.ChartData ChartData => GlobalData.Instance.chartData;
        #region 增删改查
        void AddEvent(Event @event,EventType eventType,int boxID, bool isPaste = false)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], eventType); 
            if (!isPaste)
            {
                @event.startValue = @event.endValue = events[^1].endValue;
                //@event.Curve = GlobalData.Instance.easeData[0];
                @event.curveIndex = 0;
            }
            int index = Algorithm.BinarySearch(events, m => m.startBeats.ThisStartBPM < @event.startBeats.ThisStartBPM, false);
            events.Insert(index, @event);
        }
        void DeleteEvent(Event @event, EventType eventType,int boxID)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], eventType);
            events.Remove(@event);
        }
        int FindEventIndex(Event @event, EventType eventType, int boxID)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], eventType);
            int findIndex = events.FindIndex(m => m == @event);
            return findIndex;
        }
        void AddEvent2Clipboard()
        {
            eventClipboard.Clear();
            foreach (EventEditItem selectedEventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                eventClipboard.Add(selectedEventEditItem.@event,selectedEventEditItem.eventType);
            }
            LogCenter.Log($@"已将{eventClipboard.Count}个事件发送至剪切板！");
        }
        private void SyncScaleY(Event @event, EventType eventType,int boxID, bool isPaste)
        {
            if (eventType == EventType.ScaleX && !isPaste) //同步scaleY
            {
                AddEvent(new(@event), EventType.ScaleY, boxID, isPaste);
            }
        }
        #endregion
        private KeyValueList<Event, EventType> CopyEvents(KeyValueList<Event, EventType> eventClipboard,int boxID,bool isPaste)
        {
            KeyValueList<Event, EventType> newEvents = null;
            for (int i = 0; i < eventClipboard.Count; i++) 
            {
                Event @event =new(eventClipboard[i]);
                AddEvent(@event, eventClipboard.GetValue(i),boxID,isPaste);
                newEvents.Add(@event,eventClipboard.GetValue(i));
            }
            return newEvents;
        }
        private KeyValueList<Event, EventType> AddEvents(KeyValueList<Event, EventType> eventClipboard, int boxID, bool isPaste=false)
        {
            KeyValueList<Event, EventType> newEvents = null;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                AddEvent(@event, eventClipboard.GetValue(i), boxID, isPaste);
                newEvents.Add(@event, eventClipboard.GetValue(i));
            }
            return newEvents;
        }
        private KeyValueList<Event, EventType> DeleteEvents(KeyValueList<Event, EventType> eventClipboard, int boxID,bool isCopy=false)
        {
            KeyValueList<Event, EventType> deletedEvents = new();
            if(isCopy)return deletedEvents;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                DeleteEvent(eventClipboard[i],eventClipboard.GetValue(i),boxID);
                deletedEvents.Add(eventClipboard[i],eventClipboard.GetValue(i));
            }
            return deletedEvents;
        }
        private void AlignEvents(KeyValueList<Event, EventType> eventClipboard,BPM bpm)
        {
            BPM firstEventStartBeats = eventClipboard[0].startBeats;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                eventClipboard[i].startBeats = new BPM(bpm) + (new BPM(@event.startBeats) - new BPM(firstEventStartBeats));
                eventClipboard[i].endBeats = new BPM(bpm) + (new BPM(@event.endBeats) - new BPM(firstEventStartBeats));
            }
        }
    }
}