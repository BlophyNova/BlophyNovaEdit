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
            }
            else
            {
                newEvents = CopyEvents(otherBoxEventsClipboard, currentBoxID, true); 
                AlignEvents(newEvents, beatLine.thisBPM);
                deletedEvents = DeleteEvents(otherBoxEventsClipboard, lastBoxID,isCopy);
            }


            LogCenter.Log($"成功{isCopy switch { true => "复制", false => "粘贴" }}{eventClipboard.Count}个音符");
            RefreshAll();
            if (isCopy)
            {
                Steps.Instance.Add(CopyUndo, CopyRedo, RefreshAll);
            }
            else
            {
                Steps.Instance.Add(PasteUndo, PasteRedo, RefreshAll);
            }
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
            newEvents = CopyEvents(eventClipboard, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            bpm.AddOneBeat();
            AlignEvents(newEvents, bpm);
            deletedEvents = DeleteEvents(eventClipboard, currentBoxID,false);

            Steps.Instance.Add(Undo, Redo, RefreshAll);
            RefreshAll();
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

        private void MoveDown()
        {
            KeyValueList<Event, EventType> newEvents = null;
            KeyValueList<Event, EventType> deletedEvents = null;
            newEvents = CopyEvents(eventClipboard, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            bpm.SubtractionOneBeat();
            AlignEvents(newEvents, bpm);
            deletedEvents = DeleteEvents(eventClipboard, currentBoxID, false);

            Steps.Instance.Add(Undo, Redo, RefreshAll);
            RefreshAll();
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
            KeyValueList<Event, EventType> deletedEvents = DeleteEvents(eventClipboard,currentBoxID);
            RefreshAll();
            Steps.Instance.Add(Undo, Redo, RefreshAll);
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
                //WindowSizeChanged();
                newEventEditItem.labelWindow = labelWindow;
                newEventEditItem.transform.localPosition = new(nearEventVerticalLine.transform.localPosition.x, nearBeatLine.transform.localPosition.y);
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
    }
}