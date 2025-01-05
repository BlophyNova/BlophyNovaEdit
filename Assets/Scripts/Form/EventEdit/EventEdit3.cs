using Form.NoteEdit;
using Form.PropertyEdit;
using Log;
using Manager;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;
using Scenes.DontDestroyOnLoad;
using Data.Enumerate;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using Scenes.PublicScripts;
using CustomSystem;
using Data.ChartEdit;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;
using UtilityCode.ChartTool;

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
            AddEvent2EventClipboard();
        }

        private void PasteEvent()
        {
            Debug.Log("粘贴事件");
            FindNearBeatLineAndEventVerticalLine(out BeatLine beatLine, out EventVerticalLine verticalLine);
            KeyValueList<Event, EventType> newEvents = null;
            if (eventClipboard.Count > 0)
            {
                newEvents = InstNewEvents(eventClipboard, beatLine.thisBPM);
            }
            else
            {
                newEvents = InstNewEvents(otherBoxEventsClipboard, beatLine.thisBPM);
            }

            KeyValueList<Event, EventType> deletedEvents = DeleteSourceEvent(eventClipboard);

            LogCenter.Log($"成功{isCopy switch { true => "复制", false => "粘贴" }}{eventClipboard.Count}个音符");
            RefreshAll();
            if (isCopy)
            {
                Steps.Instance.Add(CopyUndo, CopyRedo,RefreshAll);
            }
            else
            {
                Steps.Instance.Add(PasteUndo, PasteRedo,RefreshAll);
            }
            onEventRefreshed(eventEditItems);
            return;
            void CopyUndo()
            {
                for (int i = 0; i < newEvents.Count; i++)
                {
                    DeleteEvent(newEvents.GetKey(i), newEvents.GetValue(i));
                }
                //if (!isCopy)
                //{
                //    InstNewEvents(deletedEvents,beatLine);
                //}
            }
            void CopyRedo()
            {
                InstNewEvents(newEvents, beatLine.thisBPM);
                //DeleteSourceEvent();
            }
            void PasteUndo()
            {
                for (int i = 0; i < newEvents.Count; i++)
                {
                    DeleteEvent(newEvents.GetKey(i), newEvents.GetValue(i));
                }

                InstNewEvents(deletedEvents,deletedEvents[0].startBeats);
            }
            void PasteRedo()
            {
                for (int i = 0; i < deletedEvents.Count; i++)
                {
                    DeleteEvent(deletedEvents.GetKey(i), deletedEvents.GetValue(i));
                }
                InstNewEvents(newEvents,newEvents[0].startBeats);
            }
        }

        private KeyValueList<Event, EventType> DeleteSourceEvent(List<EventEditItem> eventEditItems)
        {
            KeyValueList<Event, EventType> deletedEvents = new();
            if (isCopy)
            {
                return deletedEvents;
            }

            foreach (EventEditItem eventEditItem in eventEditItems)
            {
                DeleteEvent(eventEditItem);
                deletedEvents.Add(eventEditItem.@event,eventEditItem.eventType);
                //Debug.LogError("这里有问题");
            }
            return deletedEvents;
        }

        private void CutEvent()
        {
            Debug.Log("剪切事件");
            isCopy = false;
            AddEvent2EventClipboard();
        }
        private void MoveUp()
        {
            List<EventEditItem> selectedBox = selectBox.TransmitObjects().Cast<EventEditItem>().ToList();
            foreach (EventEditItem eventEditItem in selectedBox)
            {
                BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);

                if (eventEditItem.@event.startBeats.denominator != GlobalData.Instance.chartEditData.beatSubdivision)
                {
                    BPM nearBpm = new(FindNearBeatLine((Vector2)transform.InverseTransformPoint((Vector2)transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM);
                    eventEditItem.@event.startBeats = nearBpm;
                }
                eventEditItem.@event.startBeats.AddOneBeat();
                eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
            }

            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个事件向上移动一格");
            Steps.Instance.Add(Undo,Redo,RefreshAll);
            RefreshAll();
            return;

            void Undo()
            {
                foreach (EventEditItem eventEditItem in selectedBox)
                {
                    BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);
                    eventEditItem.@event.startBeats.SubtractionOneBeat();
                    eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
                }
            }

            void Redo()
            {
                foreach (EventEditItem eventEditItem in selectedBox)
                {
                    BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);
                    eventEditItem.@event.startBeats.AddOneBeat();
                    eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
                }
            }
        }

        private void MoveDown()
        {
            List<EventEditItem> selectedBox = selectBox.TransmitObjects().Cast<EventEditItem>().ToList();
            foreach (EventEditItem eventEditItem in selectedBox)
            {
                BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);
                if (eventEditItem.@event.startBeats.denominator != GlobalData.Instance.chartEditData.beatSubdivision)
                {
                    BPM nearBpm = new(FindNearBeatLine((Vector2)transform.InverseTransformPoint(transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM);
                    eventEditItem.@event.startBeats = nearBpm;
                }
                eventEditItem.@event.startBeats.SubtractionOneBeat();
                eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
            }

            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个事件向下移动一格");
            Steps.Instance.Add(Undo, Redo, RefreshAll);
            RefreshAll();
            return;

            void Undo()
            {
                foreach (EventEditItem eventEditItem in selectedBox)
                {
                    BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);
                    eventEditItem.@event.startBeats.AddOneBeat();
                    eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
                }
            }

            void Redo()
            {
                foreach (EventEditItem eventEditItem in selectedBox)
                {
                    BPM delta =new BPM(eventEditItem.@event.endBeats) - new BPM(eventEditItem.@event.startBeats);
                    eventEditItem.@event.startBeats.SubtractionOneBeat();
                    eventEditItem.@event.endBeats = new BPM(eventEditItem.@event.startBeats) + delta;
                }
            }
        }

        private void DeleteEventWithUI()
        {
            KeyValueList<Event,EventType> deletedEvents = new();
            foreach (EventEditItem eventEditItem in eventClipboard)
            {
                List<Event> events= FindEditEventListByEventType(eventEditItem.eventType);
                if (events.Count - 1 <= 0) continue;
                events.Remove(eventEditItem.@event);
                deletedEvents.Add(eventEditItem.@event,eventEditItem.eventType);
                onEventDeleted(eventEditItem);
            }
            RefreshAll();
            Steps.Instance.Add(Undo, Redo, RefreshAll);
            return;
            void Undo()
            {
                for (int i = 0; i < deletedEvents.Count; i++) 
                {
                    AddEvent(deletedEvents[i], deletedEvents.GetValue(i));
                }
            }

            void Redo()
            {
                for (int i = 0; i < deletedEvents.Count; i++)
                {
                    List<Event> events = FindEditEventListByEventType(deletedEvents.GetValue(i));
                    events.Remove(deletedEvents[i]);
                }
            }
        }

        private void AddEvent()
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
                WindowSizeChanged();
                newEventEditItem.labelWindow = labelWindow;
                newEventEditItem.transform.localPosition = new Vector2(nearEventVerticalLine.transform.localPosition.x,
                    nearBeatLine.transform.localPosition.y);
                newEventEditItem.@event.startBeats = new BPM(nearBeatLine.thisBPM);
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
    }
}