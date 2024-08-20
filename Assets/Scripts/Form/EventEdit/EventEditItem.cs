using Data.ChartEdit;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEditItem : PublicButton
{
    public LabelWindow labelWindow;
    public RectTransform rectTransform;
    public EaseRenderer easeRenderer;
    public RectTransform isSelectedRect;
    public EventEdit ThisEventEdit => (EventEdit)labelWindow.currentLabelWindow;
    public Data.ChartEdit.Event @event;
    public Data.Enumerate.EventType eventType;
    private void Start()
    {
        thisButton.onClick.AddListener(() =>
        {
            if (labelWindow.associateLabelWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
            {
                NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelWindow;
                notePropertyEdit.@event?.isSelectedRect.gameObject.SetActive(false);
                notePropertyEdit.note?.isSelectedRect.gameObject.SetActive(false);
                notePropertyEdit.note = null;
                notePropertyEdit.SelectedNote(this);
                isSelectedRect.gameObject.SetActive(true);
            }
        });
    }
    public EventEditItem Init()
    {
        isSelectedRect.gameObject.SetActive(false);

        float minValue=float.MaxValue; 
        float maxValue=float.MinValue;
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
        List<Vector2> points = new();
        int pointCount = (int)((@event.endBeats.ThisStartBPM - @event.startBeats.ThisStartBPM) * 100);
        for (int i = 0; i <= pointCount; i++)
        {
            float time=i/(float)pointCount;
            float maxAndMinValueDelta = maxValue - minValue;
            float endAndStartValueDelta=@event.endValue - @event.startValue;
            float value = endAndStartValueDelta>=0?  @event.curve.thisCurve.Evaluate(time):1-@event.curve.thisCurve.Evaluate(time);
            if (Mathf.Abs(maxAndMinValueDelta) - Mathf.Abs(endAndStartValueDelta) > .001f)
            {
                value =Mathf.Abs(endAndStartValueDelta) / Mathf.Abs(maxAndMinValueDelta) * value + Mathf.Abs(endAndStartValueDelta) / Mathf.Abs(maxAndMinValueDelta)/2;
            }
            points.Add(new(value, time));
        }
        easeRenderer.points = points;
        return this;
    }
}
