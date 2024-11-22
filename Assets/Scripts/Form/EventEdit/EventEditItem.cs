using Data.ChartEdit;
using Form.NotePropertyEdit;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEditItem : PublicButton, ISelectBoxItem
{
    public LabelWindow labelWindow;
    public RectTransform thisEventEditItemRect;
    public RectTransform isSelectedRect;
    public RectTransform easeLineRect;
    public LineRenderer easeLine;
    public EventEdit ThisEventEdit => (EventEdit)labelWindow.currentLabelItem.labelWindowContent;
    public bool IsNoteEdit => false;
    public Data.ChartEdit.Event @event;
    public Data.Enumerate.EventType eventType;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            ThisEventEdit.selectBox.SetSingleNote(this);
        });
        labelWindow.currentLabelItem.onLabelGetFocus += LabelWindow_onLabelGetFocus;
        labelWindow.currentLabelItem.onLabelLostFocus += LabelWindow_onLabelLostFocus;
        labelWindow.onWindowSizeChanged += () => StartCoroutine(DrawLineOnEEI());
    }

    private void LabelWindow_onLabelGetFocus()
    {
        for (int i = 0; i < easeLine.positionCount; i++)
        {
            Vector3 currentIndexPosition = easeLine.GetPosition(i);
            currentIndexPosition.z = -.1f;
            easeLine.SetPosition(i, currentIndexPosition);
        }
    }

    private void LabelWindow_onLabelLostFocus()
    {
        for (int i = 0; i < easeLine.positionCount; i++)
        {
            Vector3 currentIndexPosition = easeLine.GetPosition(i);
            currentIndexPosition.z = .1f;
            easeLine.SetPosition(i, currentIndexPosition);
        }
    }
    private void OnDestroy()
    {
        labelWindow.currentLabelItem.onLabelGetFocus -= LabelWindow_onLabelGetFocus;
        labelWindow.currentLabelItem.onLabelLostFocus -= LabelWindow_onLabelLostFocus;
        Debug.Log($@"呜呜，我是EEI，喔被销毁了，我的相关信息如下：startBeats:{@event.startBeats};eventType:{eventType};");
    }
    public EventEditItem Init()
    {
        //SetSelectState(false);

        //在eei上画线
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        List<Data.ChartEdit.Event> events = eventType switch
        {
            Data.Enumerate.EventType.MoveX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.moveX,
            Data.Enumerate.EventType.MoveY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.moveY,
            Data.Enumerate.EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.scaleX,
            Data.Enumerate.EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.scaleY,
            Data.Enumerate.EventType.CenterX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.centerX,
            Data.Enumerate.EventType.CenterY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.centerY,
            Data.Enumerate.EventType.Rotate => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.rotate,
            Data.Enumerate.EventType.Alpha => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.alpha,
            Data.Enumerate.EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.lineAlpha,
            Data.Enumerate.EventType.Speed => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.speed,
            _ => throw new Exception("怎么会···没找到事件类型")
        };
        foreach (Data.ChartEdit.Event item in events)
        {
            if (item.startValue < minValue) minValue = item.startValue;
            if (item.startValue > maxValue) maxValue = item.startValue;
            if (item.endValue < minValue) minValue = item.endValue;
            if (item.endValue > maxValue) maxValue = item.endValue;
        }

        StartCoroutine(DrawLineOnEEI());
        return this;
    }

    private IEnumerator DrawLineOnEEI()
    {
        yield return new WaitForEndOfFrame();
        List<Vector3> points = new();
        int pointCount = (int)((@event.endBeats.ThisStartBPM - @event.startBeats.ThisStartBPM) * 100);
        easeLine.positionCount = pointCount;
        //easeLine.startWidth = easeLine.endWidth = -.2f;
        Vector3[] corners = new Vector3[4];
        easeLineRect.GetLocalCorners(corners);
        for (int i = 0; i < pointCount; i++)
        {
            //positions[i].
            Vector3 currentPosition = (corners[2] - corners[0]) * (i / (float)pointCount) + corners[0];
            //currentPosition.y = @event.curve.thisCurve.Evaluate(i / (float)pointCount) * (corners[2].y - corners[0].y) + corners[0].y;
            currentPosition.x = @event.curve.thisCurve.Evaluate(i / (float)pointCount) * (corners[2].x - corners[0].x) + corners[0].x;
            currentPosition.z = -.1f;
            points.Add(currentPosition);
        }
        easeLine.SetPositions(points.ToArray());
    }

    public Vector3[] GetCorners()
    {
        Vector3[] corners = new Vector3[4];
        thisEventEditItemRect.GetWorldCorners(corners);
        return corners;
    }

    public void SetSelectState(bool active)
    {
        @event.IsSelected = active;
        isSelectedRect.gameObject.SetActive(active);
        LogCenter.Log($@"{ThisEventEdit.currentBoxID}号框的{eventType}事件的{@event.startBeats.integer}:{@event.startBeats.molecule}/{@event.startBeats.denominator}的选择状态被改为：{isSelectedRect.gameObject.activeSelf}");
    }

}
