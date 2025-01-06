using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;

namespace Form.EventEdit
{
    public class EventEditItem : PublicButton, ISelectBoxItem
    {
        public LabelWindow.LabelWindow labelWindow;
        public RectTransform thisEventEditItemRect;
        public RectTransform isSelectedRect;
        public RectTransform easeLineRect;
        public LineRenderer easeLine;
        public Event @event;
        public EventType eventType;
        private EventEdit thisEventEdit;

        public EventEdit ThisEventEdit
        {
            get
            {
                if (thisEventEdit == null)
                {
                    foreach (LabelItem item in labelWindow.labels)
                    {
                        if (item.labelWindowContent.labelWindowContentType == LabelWindowContentType.EventEdit)
                        {
                            thisEventEdit = (EventEdit)item.labelWindowContent;
                        }
                    }
                }

                return thisEventEdit;
            }
        }

        private void Start()
        {
            thisButton.onClick.AddListener(() => { ThisEventEdit.selectBox.SetSingleNote(this); });
            ThisEventEdit.labelItem.onLabelGetFocus += LabelWindow_onLabelGetFocus;
            ThisEventEdit.labelItem.onLabelLostFocus += LabelWindow_onLabelLostFocus;
            labelWindow.onWindowSizeChanged += LabelWindow_onWindowSizeChanged;
        }

        private void OnDestroy()
        {
            if (labelWindow == null)
            {
                return;
            }

            ThisEventEdit.labelItem.onLabelGetFocus -= LabelWindow_onLabelGetFocus;
            ThisEventEdit.labelItem.onLabelLostFocus -= LabelWindow_onLabelLostFocus;
            labelWindow.onWindowSizeChanged -= LabelWindow_onWindowSizeChanged;
        }

        public bool IsNoteEdit => false;

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
        }

        private void LabelWindow_onWindowSizeChanged()
        {
            StartCoroutine(DrawLineOnEEI());
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

        public EventEditItem Init()
        {
            //SetSelectState(false);

            //在eei上画线
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;
            List<Event> events = eventType switch
            {
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .scaleY,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .centerY,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .rotate,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents
                    .lineAlpha,
                EventType.Speed => GlobalData.Instance.chartEditData.boxes[ThisEventEdit.currentBoxID].boxEvents.speed,
                _ => throw new Exception("怎么会···没找到事件类型")
            };
            foreach (Event item in events)
            {
                if (item.startValue < minValue)
                {
                    minValue = item.startValue;
                }

                if (item.startValue > maxValue)
                {
                    maxValue = item.startValue;
                }

                if (item.endValue < minValue)
                {
                    minValue = item.endValue;
                }

                if (item.endValue > maxValue)
                {
                    maxValue = item.endValue;
                }
            }

            StartCoroutine(DrawLineOnEEI());
            return this;
        }

        public IEnumerator DrawLineOnEEI()
        {
            easeLine.enabled = false;
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
                currentPosition.x =
                    @event.Curve.thisCurve.Evaluate(i / (float)pointCount) * (corners[2].x - corners[0].x) +
                    corners[0].x;
                currentPosition.z = -.1f;
                points.Add(currentPosition);
            }

            easeLine.SetPositions(points.ToArray());
            easeLine.enabled = true;
        }
    }
}