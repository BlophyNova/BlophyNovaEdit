using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UtilityCode.Algorithm;
using static UnityEngine.Camera;

public class EventEdit : LabelWindowContent,IInputEventCallback,IRefresh
{
    public int currentBoxID;


    public BasicLine basicLine;
    public RectTransform eventEditRect;
    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public List<RectTransform> verticalLines = new();
    public List<EventVerticalLine> eventVerticalLines = new();
    public List<EventEditItem> eventEditItems = new();
    public bool isFirstTime = false;
    public bool waitForPressureAgain = false;
    private void Start()
    {
        UpdateVerticalLineCount();
    }
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        UpdateVerticalLineCount();
    }
    public void UpdateVerticalLineCount()
    {
        int subdivision = GlobalData.Instance.chartEditData.eventVerticalSubdivision;
        Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
        Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
        for (int i = 1; i < subdivision; i++)
        {
            verticalLines[i-1].localPosition = (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) * Vector2.right;
        }
        List<RectTransform> allVerticalLines = new(verticalLines);
        allVerticalLines.Insert(0,verticalLineLeft);
        allVerticalLines.Add(verticalLineRight);
        for (int i = 0; i < eventVerticalLines.Count; i++)
        {
            eventVerticalLines[i].transform.localPosition= (allVerticalLines[i + 1].localPosition + allVerticalLines[i].localPosition)/2;
        }
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        Debug.Log($"{MousePositionInThisRectTransform}");
        if (!isFirstTime)
        {
            isFirstTime = true;
            BeatLine nearBeatLine=new();
            float nearBeatLineDis= float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log($@"{eventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis= Vector2.Distance(MousePositionInThisRectTransform, (Vector2)eventEditRect.InverseTransformPoint(item.transform.position)+ labelWindow.labelWindowRect.sizeDelta /2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }
            EventVerticalLine nearEventVerticalLine = new();
            float nearEventVerticalLineDis = float.MaxValue;
            foreach (EventVerticalLine item in eventVerticalLines)
            {
                float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearEventVerticalLineDis)
                {
                    nearEventVerticalLineDis = dis;
                    nearEventVerticalLine = item;
                }
            }
            EventEditItem newEventEditItem =Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
            newEventEditItem.transform.localPosition = new Vector2(nearEventVerticalLine.transform.localPosition.x,nearBeatLine.transform.localPosition.y);
            newEventEditItem.@event.startBeats = new(nearBeatLine.thisBPM);
            newEventEditItem.eventType = nearEventVerticalLine.eventType;
            StartCoroutine(WaitForPressureAgain(newEventEditItem));
        }
        else if (isFirstTime)
        {
            //第二次
            isFirstTime = false;
            waitForPressureAgain = true;
        }
        else {/*报错*/}
    }
    public IEnumerator WaitForPressureAgain(EventEditItem eventEditItem)
    {
        while (true)
        {
            if (waitForPressureAgain) break;
            BeatLine nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log($@"{eventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }
            eventEditItem.rectTransform.sizeDelta = new(eventEditItem.rectTransform.sizeDelta.x, nearBeatLine.transform.localPosition.y - eventEditItem.transform.localPosition.y);
            eventEditItem.@event.endBeats = new(nearBeatLine.thisBPM);
            yield return new WaitForEndOfFrame();
        }
        waitForPressureAgain = false;

        if (eventEditItem.@event.endBeats.ThisStartBPM - eventEditItem.@event.startBeats.ThisStartBPM <= .0001f)
        {
            Debug.LogError("哒咩哒咩，长度为0的Hold！");
            Destroy(eventEditItem.gameObject);
        }
        else
        {
            eventEditItems.Add(eventEditItem);
            //添加事件到对应的地方
            List<Data.ChartEdit.Event> events = eventEditItem.eventType switch
            {
                Data.Enumerate.EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
                Data.Enumerate.EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
                Data.Enumerate.EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
                Data.Enumerate.EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
                Data.Enumerate.EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
                Data.Enumerate.EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
                Data.Enumerate.EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
                Data.Enumerate.EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
                Data.Enumerate.EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
                Data.Enumerate.EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
                _ =>null
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
                Data.Enumerate.EventType.Speed => GlobalData.Instance.chartData.boxes[currentBoxID].lines[0].speed,
                Data.Enumerate.EventType.CenterX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerX,
                Data.Enumerate.EventType.CenterY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerY,
                Data.Enumerate.EventType.MoveX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveX,
                Data.Enumerate.EventType.MoveY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveY,
                Data.Enumerate.EventType.ScaleX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleX,
                Data.Enumerate.EventType.ScaleY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
                Data.Enumerate.EventType.Rotate => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.rotate,
                Data.Enumerate.EventType.Alpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.alpha,
                Data.Enumerate.EventType.LineAlpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.lineAlpha,
                _ => null
            };
            GlobalData.Instance.RefreshChartEventByChartEditEvent(chartDataEvents, eventEditItem.@event);
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[4].speed =
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[3].speed =
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[2].speed =
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[1].speed =
            GlobalData.Instance.chartData.boxes[currentBoxID].lines[0].speed;
            Debug.LogError("错误记忆");
        }
    }
    public void Refresh()
    {
        UpdateVerticalLineCount();
    }
    public void RefreshNotes(int boxID)
    {
        currentBoxID= boxID;
    }
}
