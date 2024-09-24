using Scenes.DontDestroyOnLoad;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using EventType = Data.Enumerate.EventType;

public partial class EventEdit
{
    public delegate void OnEventDeleted(EventEditItem eventEditItem);
    public event OnEventDeleted onEventDeleted = eventEditItem => { };

    public delegate void OnEventRefreshed(List<EventEditItem> eventEditItems);
    public event OnEventRefreshed onEventRefreshed=eventEditItems => { };
    void SelectBoxDown()
    {
        selectBox.isPressing = true;
        selectBox.transform.SetAsLastSibling();
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
    void SelectBoxUp()
    {
        selectBox.isPressing = false;
        selectBox.transform.SetAsFirstSibling();
        Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
    }
    private void AddNewEvent2EventList(EventEditItem eventEditItem)
    {
        List<Data.ChartEdit.Event> events = eventEditItem.eventType switch
        {
            EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
            EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
            EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
            _ => null
        };

        eventEditItem.@event.startValue = eventEditItem.@event.endValue = events[^1].endValue;
        eventEditItem.@event.curve = GlobalData.Instance.easeData[0];
        events.Add(eventEditItem.@event);
        Algorithm.BubbleSort(events, (a, b) =>//排序
        {
            if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
            {
                return 1;
            }
            else if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
            {
                return -1;
            }
            return 0;
        });
        List<Data.ChartData.Event> chartDataEvents = eventEditItem.eventType switch
        {
            EventType.CenterX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerX,
            EventType.CenterY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerY,
            EventType.MoveX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveX,
            EventType.MoveY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveY,
            EventType.ScaleX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleX,
            EventType.ScaleY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
            EventType.Rotate => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.rotate,
            EventType.Alpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.alpha,
            EventType.LineAlpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.lineAlpha,
            _ => null
        };
        ChartTool.RefreshChartEventByChartEditEvent(chartDataEvents, eventEditItem.@event);

        if (eventEditItem.eventType == EventType.ScaleX)//同步scaleY
        {
            List<Data.ChartEdit.Event> scaleYEvents = GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY;
            scaleYEvents.Add(new(eventEditItem.@event));
            Algorithm.BubbleSort(scaleYEvents, (a, b) =>//排序
            {
                if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
                {
                    return 1;
                }
                else if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
                {
                    return -1;
                }
                return 0;
            });
            ChartTool.RefreshChartEventByChartEditEvent(GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY, new(eventEditItem.@event));
            RefreshNotes(-1);
        }
        #region 以下代码为speed事件处理相关专属代码，没啥bug的情况下一个字都别改
        if (eventEditItem.eventType != EventType.Speed) goto skipSpeed;
        List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed);
        for (int i = 0; i < GlobalData.Instance.chartData.boxes[currentBoxID].lines.Count; i++)
        {
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed = new();


            ChartTool.ForeachBoxEvents(filledVoid, GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed);
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career.keys = GameUtility.CalculatedSpeedCurve(GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed.ToArray()).ToArray();


            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far = new() { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far.keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
        }
        skipSpeed:
        eventEditItem.Init();
        #endregion

        //Debug.LogError("错误记忆");
    }
}
