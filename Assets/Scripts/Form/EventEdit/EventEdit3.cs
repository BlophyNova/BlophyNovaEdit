using CustomSystem;
using Data.ChartEdit;
using Form.NoteEdit;
using Log;
using Scenes.DontDestroyOnLoad;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilityCode.ChartTool;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ChartTool.ChartTool;
using System;
using Manager;
using Form.PropertyEdit;
namespace Form.EventEdit
{
    //这里放用户编辑操作响应相关的事情
    public partial class EventEdit
    {
        private void SelectBoxDown()
        {
            selectBox.isPressing = true;
            selectBox.transform.SetAsLastSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        private void SelectBoxUp()
        {
            selectBox.isPressing = false;
            selectBox.transform.SetAsFirstSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        private void UndoNote()
        {
        }

        private void RedoNote()
        {
        }

        private void CopyEvent()
        {
            Debug.Log("复制事件");
            isCopy = true;
            AddEvent2Clipboard();
        }

        private void CutEvent()
        {
            Debug.Log("剪切事件");
            isCopy = false;
            AddEvent2Clipboard();
        }
        private void PasteEvent()
        {
            Debug.Log("粘贴事件");
            FindNearBeatLineAndEventVerticalLine(out BeatLine beatLine, out EventVerticalLine verticalLine);
            KeyValueList<Event, EventType> newEvents = null;
            KeyValueList<Event, EventType> deletedEvents = null;
            if (eventClipboard.Count > 0)
            {
                newEvents = CopyEvents(eventClipboard,currentBoxID,true);
                AlignEvents(newEvents, beatLine.thisBPM);
                deletedEvents = DeleteEvents(eventClipboard, currentBoxID,isCopy);
                AddEvents(newEvents,currentBoxID,true);
                AddEvents2UI(newEvents);
            }
            else
            {
                newEvents = CopyEvents(otherBoxEventsClipboard, currentBoxID, true); 
                AlignEvents(newEvents, beatLine.thisBPM);
                deletedEvents = DeleteEvents(otherBoxEventsClipboard, lastBoxID,isCopy);
                AddEvents(newEvents, currentBoxID, true);
                AddEvents2UI(newEvents);
            }


            LogCenter.Log($"成功{isCopy switch { true => "复制", false => "粘贴" }}{eventClipboard.Count}个音符");
            //RefreshAll();
            if (isCopy)
            {
                Steps.Instance.Add(CopyUndo, CopyRedo, RefreshAll);
            }
            else
            {
                Steps.Instance.Add(PasteUndo, PasteRedo, RefreshAll);
            }
            isCopy = true;
            return;
            void CopyUndo()
            {
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }
            void CopyRedo()
            {
                AddEvents(newEvents, currentBoxID,true);
            }
            void PasteUndo()
            {
                AddEvents(deletedEvents, currentBoxID, true);
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }
            void PasteRedo()
            {
                AddEvents(newEvents,currentBoxID,true);
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }

        



        private void MoveUp()
        {
            KeyValueList<Event, EventType> newEvents = null;
            KeyValueList<Event, EventType> deletedEvents = null;
            KeyValueList<Event, EventType> selectedEvents = GetSelectedEvents();
            newEvents = CopyEvents(selectedEvents, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedEvents[0].chartEditEvent.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new(nearBPM);
            if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            {
                bpm.AddOneBeat();
            }
            AlignEvents(newEvents, bpm);
            AddEvents(newEvents, currentBoxID, true);
            AddEvents2UI(newEvents);

            deletedEvents = DeleteEvents(selectedEvents, currentBoxID, false);
            Steps.Instance.Add(Undo, Redo, RefreshAll);
            return;
            void Undo()
            {
                AddEvents(deletedEvents, currentBoxID, true);
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }
            void Redo()
            {
                AddEvents(newEvents, currentBoxID, true);
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }

        private KeyValueList<Event, EventType> GetSelectedEvents()
        {
            IEnumerable<EventEditItem> selectedEventEditItems = selectBox.TransmitObjects().Cast<EventEditItem>();
            KeyValueList<Event, EventType> selectedEvents = new();
            foreach (EventEditItem item in selectedEventEditItems)
            {
                selectedEvents.Add(item.@event, item.eventType);
            }

            return selectedEvents;
        }

        private void MoveDown()
        {
            KeyValueList<Event, EventType> newEvents = null;
            KeyValueList<Event, EventType> deletedEvents = null;
            KeyValueList<Event, EventType> selectedEvents = GetSelectedEvents();
            newEvents = CopyEvents(selectedEvents, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedEvents[0].chartEditEvent.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new(nearBPM);
            if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            {
                bpm.SubtractionOneBeat();
            }
            AlignEvents(newEvents, bpm);
            AddEvents(newEvents, currentBoxID, true);
            AddEvents2UI(newEvents);

            deletedEvents = DeleteEvents(selectedEvents, currentBoxID, false);
            Steps.Instance.Add(Undo, Redo, RefreshAll);
            return;
            void Undo()
            {
                AddEvents(deletedEvents, currentBoxID, true);
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }
            void Redo()
            {
                AddEvents(newEvents, currentBoxID, true);
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }

        private void DeleteEventFromUI()
        {
            IEnumerable<EventEditItem> selectesBoxes = selectBox.TransmitObjects().Cast<EventEditItem>();
            KeyValueList<Event, EventType> selectedEvents = new();
            foreach (EventEditItem eventEditItem in selectesBoxes)
            {
                selectedEvents.Add(eventEditItem.@event,eventEditItem.eventType);
            }
            KeyValueList<Event, EventType> deletedEvents = DeleteEvents(selectedEvents, currentBoxID);
            //RefreshAll();
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                AddEvents(deletedEvents,currentBoxID, true);
            }

            void Redo()
            {
                DeleteEvents(eventClipboard, currentBoxID);
            }
        }

        private void AddEventFromUI()
        {
            Debug.Log($"{MousePositionInThisRectTransform}");
            if (!isFirstTime)
            {
                isFirstTime = true;
                FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine,
                    out EventVerticalLine nearEventVerticalLine);

                if (nearEventVerticalLine.eventType == EventType.LineAlpha)
                {
                    isFirstTime = false;
                    return;
                }

                EventEditItem newEventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
                //WindowSizeChanged();
                newEventEditItem.labelWindow = labelWindow;
                newEventEditItem.transform.localPosition = new(nearEventVerticalLine.transform.localPosition.x, nearBeatLine.transform.localPosition.y);
                newEventEditItem.easeLine.enabled = false;
                newEventEditItem.@event.chartEditEvent = newEventEditItem;
                newEventEditItem.@event.startBeats = new(nearBeatLine.thisBPM);
                newEventEditItem.eventType = nearEventVerticalLine.eventType;
                StartCoroutine(WaitForPressureAgain(newEventEditItem));
            }
            else if (isFirstTime)
            {
                //第二次
                isFirstTime = false;
                waitForPressureAgain = true;
            } /*报错*/
        }
        private List<EventEditItem> AddEvents2UI(KeyValueList<Event,EventType> keyValueList)
        {
            List<EventEditItem> eventEditItems = new();
            for (int i = 0; i < keyValueList.Count; i++)
            {
                foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
                {
                    if (eventVerticalLine.eventType == keyValueList.GetValue(i))
                    {
                        EventEditItem newEventEditItem = AddEvent2UI(keyValueList[i], keyValueList.GetValue(i), eventVerticalLine.transform.localPosition.x);
                        eventEditItems.Add(newEventEditItem);
                    }
                }
            }
            return eventEditItems;
        }
        private EventEditItem AddEvent2UI(Event @event, EventType eventType, float localPositionX)
        {
            EventEditItem newEventEditItem =
                Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);


            float currentSecondsTime =
                BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM);
            float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

            newEventEditItem.transform.localPosition = new(localPositionX, positionY);

            float endBeatsSecondsTime =
                BPMManager.Instance.GetSecondsTimeByBeats(@event.endBeats.ThisStartBPM);
            float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(endBeatsSecondsTime);

            newEventEditItem.labelWindow = labelWindow;
            newEventEditItem.thisEventEditItemRect.sizeDelta = new Vector2(
                Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition), endBeatsPositionY - positionY);
            newEventEditItem.@event = @event;
            newEventEditItem.@event.IsSelected = false;
            newEventEditItem.eventType = eventType;
            newEventEditItem.SetSelectState(@event.IsSelected);
            @event.chartEditEvent = newEventEditItem;
            newEventEditItem.Init();
            return newEventEditItem;
        }
        private void DestroyEvents(EventType eventType)
        {
            List<EventEditItem> tempEventEditItems = new();
            foreach (EventEditItem item in eventEditItems)
            {
                if (item.eventType == eventType)
                {
                    Destroy(item.gameObject);
                }
                else
                {
                    tempEventEditItems.Add(item);
                }
            }
            eventEditItems = tempEventEditItems;
        }

        private void AddEvents2UI(List<Event> events, EventType eventType)
        {
            foreach (Event @event in events)
            {
                foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
                {
                    if (eventVerticalLine.eventType == eventType)
                    {
                        EventEditItem newEventEditItem = AddEvent2UI(@event, eventType, eventVerticalLine.transform.localPosition.x);
                        eventEditItems.Add(newEventEditItem);
                        continue;
                    }
                }
            }
        }
    }
}