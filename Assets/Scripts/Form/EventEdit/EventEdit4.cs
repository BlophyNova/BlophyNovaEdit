using Form.PropertyEdit;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.ChartTool;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ChartTool.ChartTool;
using System.Linq;
using Unity.VisualScripting;
using Data.Interface;
namespace Form.EventEdit
{
    //这里放所有的刷新方法
    public partial class EventEdit
    {
        #region 一键刷新当前框的所有事件
        void RefreshEvents(int boxID)
        {

            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            List<EventEditItem> allSelectedEvents = new();
            allSelectedEvents.AddRange(RefreshEvents(EventType.Speed, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.CenterX, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.CenterY, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.MoveX, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.MoveY, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.ScaleX, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.ScaleY, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.Rotate, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.Alpha, currentBoxID));
            allSelectedEvents.AddRange(RefreshEvents(EventType.LineAlpha, currentBoxID));

            List<ISelectBoxItem> selectedEvents = new();
            foreach (var item in allSelectedEvents)
            {
                selectedEvents.Add(item);
            }

            selectBox.SetMutliNote(selectedEvents);
        }
        void RefreshPlayer(int boxID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            ConvertAllEvents(GlobalData.Instance.chartEditData.boxes[currentBoxID], GlobalData.Instance.chartData.boxes[currentBoxID]);
        }
        #endregion
        /// <summary>
        /// 刷新某一个方框的所有事件
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="boxID"></param>
        /// <returns>isSelect为True的Events</returns>
        List<EventEditItem> RefreshEvents(EventType eventType, int boxID)
        {
            LogCenter.Log($"成功更改框号为{currentBoxID}");

            if (boxID != currentBoxID && currentBoxID >= 0)
            {
                List<Event> setSelect2False = FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[lastBoxID], eventType);
                foreach (Event item in setSelect2False)
                {
                    item.IsSelected = false;
                }
            }
            DestroyEvents(eventType);

            List<Event> events = FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[currentBoxID], eventType);
            List<Event> targetEvents = new();
            List<EventEditItem> selectedEvents = new();
            foreach (Event @event in events) 
            {
                @event.eventType = eventType;
                targetEvents.Add(@event);
            }
            List<EventEditItem> newEvents = AddEvents2UI(targetEvents);
            eventEditItems.AddRange(newEvents);

            foreach (EventEditItem item in newEvents)
            {
                if (item.@event.IsSelected)
                {
                    selectedEvents.Add(item);
                }
            }
            UpdateNoteLocalPositionAndSize();

            onEventsRefreshed(targetEvents);
            return selectedEvents;
        }

    }
}