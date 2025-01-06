using Form.PropertyEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using UnityEngine;

namespace Form.EventEdit
{
    //这里放控件本身的方法
    public partial class EventEdit
    {
        private void LabelWindow_onWindowMoved()
        {
            eventLineRenderer.lineRendererTextureRect.anchoredPosition = labelWindow.labelWindowRect.anchoredPosition;
        }
        private void LabelWindow_onWindowGetFocus()
        {
            LabelWindowsManager.Instance.lineRendererParent.transform.SetAsLastSibling();
        }

        private void LabelWindow_onWindowLostFocus()
        {
        }
        private void LabelItem_onLabelLostFocus()
        {
            eventLineRenderer.gameObject.SetActive(false);
        }

        private void LabelItem_onLabelGetFocus()
        {
            eventLineRenderer.gameObject.SetActive(true);
        }
        private void EventEdit_onEventRefreshed(List<EventEditItem> eventEditItems)
        {
            eventClipboard.Clear();
            foreach (EventEditItem item in eventEditItems)
            {
                if (item.@event.IsSelected)
                {
                    eventClipboard.Add(item);
                }
            }
        }
        private void UpdateEventEditItemLineRendererRectSize()
        {
            eventLineRenderer.lineRendererTextureRect.sizeDelta = labelWindow.labelWindowRect.sizeDelta;
        }
        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateVerticalLineCount();
            UpdateNoteLocalPositionAndSize();
            UpdateEventEditItemLineRendererRectSize();
        }

        public void UpdateNoteLocalPositionAndSize()
        {
            for (int i = 0; i < eventEditItems.Count; i++)
            {
                foreach (EventVerticalLine item in eventVerticalLines)
                {
                    if (item.eventType != eventEditItems[i].eventType) continue;
                    float positionX = item.transform.localPosition.x;
                    eventEditItems[i].transform.localPosition = new Vector3(positionX,
                        YScale.Instance.GetPositionYWithBeats(eventEditItems[i].@event.startBeats.ThisStartBPM));
                    eventEditItems[i].thisEventEditItemRect.sizeDelta = new Vector2(
                        Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition),
                        eventEditItems[i].thisEventEditItemRect.sizeDelta.y);
                }
            }
        }
        public void UpdateVerticalLineCount()
        {
            int subdivision = GlobalData.Instance.chartEditData.eventVerticalSubdivision;
            Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
            Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
            for (int i = 1; i < subdivision; i++)
            {
                verticalLines[i - 1].localPosition =
                    (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) *
                    Vector2.right;
            }

            List<RectTransform> allVerticalLines = new(verticalLines);
            allVerticalLines.Insert(0, verticalLineLeft);
            allVerticalLines.Add(verticalLineRight);
            for (int i = 0; i < eventVerticalLines.Count; i++)
            {
                eventVerticalLines[i].transform.localPosition =
                    (allVerticalLines[i + 1].localPosition + allVerticalLines[i].localPosition) / 2;
            }
        }
    }
}