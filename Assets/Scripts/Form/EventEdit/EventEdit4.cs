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

namespace Form.EventEdit
{
    //这里放所有的刷新方法
    public partial class EventEdit
    {
        private void RefreshAll()
        {
            RefreshEditEvents(-1, false);
            RefreshPlayerEvents(false);
            onBoxRefreshed(currentBoxID);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isExeRefreshEvent">是否执行刷新事件</param>
        private void RefreshPlayerEvents(bool isExeRefreshEvent = true)
        {
            ChartTool.ConvertAllEditEvents2ChartDataEvents(GlobalData.Instance.chartEditData.boxes[currentBoxID],
                GlobalData.Instance.chartData.boxes[currentBoxID]);
            if (isExeRefreshEvent)
                onBoxRefreshed(currentBoxID);
        }
        /// <summary>
        /// 刷新某一个方框的所有事件
        /// </summary>
        /// <param name="boxID">方框ID</param>
        public void RefreshEditEvents(int boxID, bool isExeRefreshEvent = true)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            LogCenter.Log($"成功更改框号为{currentBoxID}");
            if (boxID >= 0)
            {
                EventCopy();
            }

            StartCoroutine(RefreshEditEvents(isExeRefreshEvent));
        }
        public IEnumerator RefreshEditEvents(bool isExeRefreshEvent = true)
        {
            yield return new WaitForEndOfFrame();
            foreach (EventEditItem item in eventEditItems)
            {
                Destroy(item.gameObject);
            }

            eventEditItems.Clear();
            if (isRef)
            {
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed, EventType.Speed);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX, EventType.CenterX);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY, EventType.CenterY);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX, EventType.MoveX);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY, EventType.MoveY);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX, EventType.ScaleX);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY, EventType.ScaleY);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate, EventType.Rotate);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha, EventType.Alpha);
                RefreshEditEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha, EventType.LineAlpha);
            }

            UpdateNoteLocalPositionAndSize();
            onEventRefreshed(eventEditItems);
            if (isExeRefreshEvent) onBoxRefreshed(currentBoxID);
        }
        /// <summary>
        /// 此方法的作用是，如果要换框，那就把当前剪切板的内容复制到其他剪切板中，实现跨框复制事件的功能
        /// </summary>
        public void EventCopy()
        {
            if (eventClipboard.Count > 0)
            {
                for (int i = 0; i < otherBoxEventsClipboard.Count; i++)
                {
                    Destroy(otherBoxEventsClipboard[i].gameObject);
                }

                otherBoxEventsClipboard.Clear();
            }

            foreach (EventEditItem item in eventClipboard)
            {
                EventEditItem eventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
                eventEditItem.gameObject.SetActive(false);
                eventEditItem.@event = item.@event;
                eventEditItem.eventType = item.eventType;
                otherBoxEventsClipboard.Add(eventEditItem);
                item.@event.IsSelected = false;
            }
        }

        private void RefreshEditEvent(List<Event> events, EventType eventType)
        {
            foreach (Event @event in events)
            {
                foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
                {
                    if (eventVerticalLine.eventType == eventType)
                    {
                        EventEditItem newEventEditItem =
                            Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);


                        float currentSecondsTime =
                            BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM);
                        float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                        newEventEditItem.transform.localPosition =
                            new Vector2(eventVerticalLine.transform.localPosition.x, positionY);

                        float endBeatsSecondsTime =
                            BPMManager.Instance.GetSecondsTimeByBeats(@event.endBeats.ThisStartBPM);
                        float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(endBeatsSecondsTime);

                        newEventEditItem.labelWindow = labelWindow;
                        newEventEditItem.thisEventEditItemRect.sizeDelta = new Vector2(
                            newEventEditItem.thisEventEditItemRect.sizeDelta.x, endBeatsPositionY - positionY);
                        newEventEditItem.@event = @event;
                        newEventEditItem.eventType = eventType;
                        newEventEditItem.SetSelectState(@event.IsSelected);
                        eventEditItems.Add(newEventEditItem);
                        newEventEditItem.Init();
                    }
                }
            }
        }

        private void AddEventAndRefresh(Data.ChartEdit.Event copyNewEvent, EventType eventType, int currentBoxID)
        {
            List<Data.ChartEdit.Event> events = eventType switch
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

            events.Add(copyNewEvent);
            Algorithm.BubbleSort(events, (a, b) => //排序
            {
                if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
                {
                    return 1;
                }

                if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
                {
                    return -1;
                }

                return 0;
            });
            RefreshEditEvents(-1);
        }
    }
}