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
            RefreshEvents(EventType.Speed, boxID);
            RefreshEvents(EventType.CenterX, boxID);
            RefreshEvents(EventType.CenterY, boxID);
            RefreshEvents(EventType.MoveX, boxID);
            RefreshEvents(EventType.MoveY, boxID);
            RefreshEvents(EventType.ScaleX, boxID);
            RefreshEvents(EventType.ScaleY, boxID);
            RefreshEvents(EventType.Rotate, boxID);
            RefreshEvents(EventType.Alpha, boxID);
            RefreshEvents(EventType.LineAlpha, boxID);
        }
        void RefreshPlayer(int boxID)
        {
            ConvertAllEvents(GlobalData.Instance.chartEditData.boxes[boxID], GlobalData.Instance.chartData.boxes[boxID]);
        }
        #endregion
        #region 一键刷新当前框某一个特定事件的所有事件
        void RefreshAll(EventType eventType, int boxID)
        {
            RefreshEvents(eventType, boxID);
            RefreshPlayer(eventType,boxID);
        }
        void RefreshPlayer(EventType eventType,int boxID)
        {
            ConvertEvents(GlobalData.Instance.chartEditData.boxes[boxID], GlobalData.Instance.chartData.boxes[boxID],eventType, boxID);
        }
        #endregion
        /// <summary>
        /// 刷新某一个方框的所有事件
        /// </summary>
        /// <param name="boxID">方框ID</param>
        void RefreshEvents(EventType eventType, int boxID)
        {
            LogCenter.Log($"成功更改框号为{currentBoxID}");

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Debug.Log($"这里！！！赶紧重构完音符属性编辑控件后，把这里删了！！");
            }
            List<Event> setSelect2False = FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[lastBoxID], eventType);
            foreach (Event item in setSelect2False)
            {
                item.IsSelected = false;
            }




            DestroyEvents(eventType);

            List<Event> events = FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[currentBoxID], eventType);
            List<Event> keyValueList = new();
            foreach (Event @event in events) 
            {
                @event.eventType = eventType;
                keyValueList.Add(@event);
            }
            eventEditItems.AddRange(AddEvents2UI(keyValueList));
            UpdateNoteLocalPositionAndSize();

            onEventsRefreshed(keyValueList);
        }

    }
}