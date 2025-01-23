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
using Scenes.PublicScripts;
namespace Form.EventEdit
{
    //这里放处理数据的方法,不负责刷新
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
            AddEvent2ChartData(@event, eventType, boxID);
        }
        void DeleteEvent(Event @event, EventType eventType,int boxID)
        {
            List<Event> events = FindChartEditEventList(ChartEditData.boxes[boxID], eventType);
            if (events.Count <= 1||@event.disallowDelete)
            {
                Alert.EnableAlert("这个事件不允许删除了啦（小声嘀咕");
                return;
            }
            events.Remove(@event);
            eventEditItems.Remove(@event.chartEditEvent);
            if(@event.chartEditEvent != null) 
            Destroy(@event.chartEditEvent.gameObject);
            DeleteEvent2ChartData(@event, eventType, boxID);
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
            KeyValueList<Event, EventType> newEvents = new();
            for (int i = 0; i < eventClipboard.Count; i++) 
            {
                Event @event =new(eventClipboard[i]);
                //AddEvent(@event, eventClipboard.GetValue(i),boxID,isPaste);
                newEvents.Add(@event,eventClipboard.GetValue(i));
            }
            return newEvents;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventClipboard"></param>
        /// <param name="boxID"></param>
        /// <param name="isPaste">为True的话，跳过重置数值和曲线</param>
        /// <returns></returns>
        private KeyValueList<Event, EventType> AddEvents(KeyValueList<Event, EventType> eventClipboard, int boxID, bool isPaste=false)
        {
            KeyValueList<Event, EventType> newEvents = new();
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                Event @event = eventClipboard[i];
                AddEvent(@event, eventClipboard.GetValue(i), boxID, isPaste);
                newEvents.Add(@event, eventClipboard.GetValue(i));
            }
            onEventsAdded(newEvents);
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
            onEventsDeleted(deletedEvents);
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
        private void BatchEvents(KeyValueList<Event,EventType>events,Action<Event> action)
        {
            for (int i = 0; i < events.Count; i++) 
            {
                action(events[i]);
            }
        }
    }
}