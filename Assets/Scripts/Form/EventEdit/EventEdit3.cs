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
                    BPM nearBpm = new(FindNearBeatLine((Vector2)transform.InverseTransformPoint(transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM);
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
            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType !=
                LabelWindowContentType.NotePropertyEdit) return;
            NotePropertyEdit.NotePropertyEdit notePropertyEdit =
                (NotePropertyEdit.NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelItem
                    .labelWindowContent;
            List<Event> events = notePropertyEdit.@event.eventType switch
            {
                EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
                _ => throw new Exception("耳朵耷拉下来，呜呜呜，没找到事件类型")
            };
            if (events.FindIndex(item => item.Equals(notePropertyEdit.@event.@event)) == 0)
            {
                LogCenter.Log($"用户尝试删除{notePropertyEdit.@event.eventType}的第一个事件");
                Alert.EnableAlert("这是第一个事件，不支持删除了啦~");
                return;
            }

            LogCenter.Log(
                $"{notePropertyEdit.@event.eventType}的{notePropertyEdit.@event.@event.startBeats.integer}:{notePropertyEdit.@event.@event.startBeats.molecule}/{notePropertyEdit.@event.@event.startBeats.denominator}事件被删除");
            events.Remove(notePropertyEdit.@event.@event);
            onEventDeleted(notePropertyEdit.@event);
            notePropertyEdit.RefreshEvents();
            Steps.Instance.Add(Undo, Redo,default);
            return;
            void Undo()
            {
                Event @event = notePropertyEdit.@event.@event;
                EventType eventType = notePropertyEdit.@event.eventType;
                //eventEditItems.Add(eventEditItem);
                AddEvent(@event, eventType);
                RefreshEditEvents(-1);
            }

            void Redo()
            {
                DeleteEvent(notePropertyEdit.@event);
                RefreshAll();
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