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
namespace Form.EventEdit
{
    //这里放所有的刷新方法
    public partial class EventEdit
    {
        #region 一键刷新当前框的所有事件
        private void RefreshAll()
        {
            RefreshEvents(-1);
            RefreshPlayer(currentBoxID);
        }
        public void RefreshEvents(int boxID) 
        {
            RefreshEvents(EventType.Speed,boxID);
            RefreshEvents(EventType.CenterX,boxID);
            RefreshEvents(EventType.CenterY,boxID);
            RefreshEvents(EventType.MoveX,boxID);
            RefreshEvents(EventType.MoveY,boxID);
            RefreshEvents(EventType.ScaleX,boxID);
            RefreshEvents(EventType.ScaleY,boxID);
            RefreshEvents(EventType.Rotate,boxID);
            RefreshEvents(EventType.Alpha,boxID);
            RefreshEvents(EventType.LineAlpha,boxID);
        }
        private void RefreshPlayer(int boxID)
        {
            ConvertAllEvents(GlobalData.Instance.chartEditData.boxes[boxID], GlobalData.Instance.chartData.boxes[boxID]);
        }
        #endregion
        #region 一键刷新当前框某一个特定事件的所有事件
        private void RefreshAll(EventType eventType, int boxID)
        {
            RefreshEvents(eventType, boxID);
            RefreshPlayer(eventType,boxID);
        }
        private void RefreshPlayer(EventType eventType,int boxID)
        {
            ConvertEvents(GlobalData.Instance.chartEditData.boxes[boxID], GlobalData.Instance.chartData.boxes[boxID],eventType, boxID);
        }
        #endregion
        /// <summary>
        /// 刷新某一个方框的所有事件
        /// </summary>
        /// <param name="boxID">方框ID</param>
        public void RefreshEvents(EventType eventType, int boxID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            LogCenter.Log($"成功更改框号为{currentBoxID}");
            if (boxID >= 0)
            {
                EventCopy();
            }

            DestroyEvents(eventType);

            List<Event> events = FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[currentBoxID], eventType);
            KeyValueList<Event, EventType> keyValueList = new();
            foreach (Event @event in events) 
            {
                keyValueList.Add(@event, eventType);
            }
            eventEditItems.AddRange(AddEvents2UI(keyValueList));
            UpdateNoteLocalPositionAndSize();

            onEventsRefreshed(keyValueList);
        }



        /// <summary>
        /// 此方法的作用是，如果要换框，那就把当前剪切板的内容复制到其他剪切板中，实现跨框复制事件的功能
        /// </summary>
        public void EventCopy()
        {
            if (eventClipboard.Count <= 0)
            {
                return;
            }
            otherBoxEventsClipboard.Clear();
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                otherBoxEventsClipboard.Add(eventClipboard.GetKey(i), eventClipboard.GetValue(i));
            }
        }
    }
}