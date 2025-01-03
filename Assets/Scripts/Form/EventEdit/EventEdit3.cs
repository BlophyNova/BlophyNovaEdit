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

namespace Form.EventEdit
{
    //������û��༭������Ӧ��ص�����
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
            Debug.Log("�����¼�");
            isCopy = true;
            AddEvent2EventClipboard();
        }

        private void PasteEvent()
        {
            Debug.Log("ճ���¼�");
            FindNearBeatLineAndEventVerticalLine(out BeatLine beatLine, out EventVerticalLine verticalLine);
            KeyValueList<Event, EventType> newEvents = null;
            if (eventClipboard.Count > 0)
            {
                newEvents = InstNewEvents(eventClipboard, beatLine);
            }
            else
            {
                newEvents = InstNewEvents(otherBoxEventsClipboard, beatLine);
            }

            KeyValueList<Event, EventType> deletedEvents = DeleteSourceEvent();

            LogCenter.Log($"�ɹ�{isCopy switch { true => "����", false => "ճ��" }}{eventClipboard.Count}������");
            RefreshAll();
            Steps.Instance.Add(CopyUndo, CopyRedo,RefreshAll);
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
                InstNewEvents(newEvents, beatLine);
                //DeleteSourceEvent();
            }
            void PasteUndo()
            {

            }
            void PasteRedo()
            {

            }
        }

        private KeyValueList<Event, EventType> DeleteSourceEvent()
        {
            KeyValueList<Event, EventType> deletedEvents = new();
            if (!isCopy)
            {
                foreach (EventEditItem eventEditItem in eventClipboard)
                {
                    DeleteEvent(eventEditItem);
                    deletedEvents.Add(eventEditItem.@event,eventEditItem.eventType);
                    //Debug.LogError("����������");
                }
            }
            return deletedEvents;
        }

        private void CutEvent()
        {
            Debug.Log("�����¼�");
            isCopy = false;
            AddEvent2EventClipboard();
        }
        private void MoveUp()
        {
            foreach (EventEditItem eventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                eventEditItem.@event.startBeats.AddOneBeat();
                eventEditItem.@event.endBeats.AddOneBeat();
            }

            LogCenter.Log($"�ɹ���{selectBox.TransmitObjects().Count}���¼������ƶ�һ��");

            RefreshAll();
        }

        private void MoveDown()
        {
            foreach (EventEditItem eventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                eventEditItem.@event.startBeats.SubtractionOneBeat();
                eventEditItem.@event.endBeats.SubtractionOneBeat();
            }

            LogCenter.Log($"�ɹ���{selectBox.TransmitObjects().Count}���¼������ƶ�һ��");

            RefreshAll();
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
                _ => throw new Exception("�������������������أ�û�ҵ��¼�����")
            };
            if (events.FindIndex(item => item.Equals(notePropertyEdit.@event.@event)) == 0)
            {
                LogCenter.Log($"�û�����ɾ��{notePropertyEdit.@event.eventType}�ĵ�һ���¼�");
                Alert.EnableAlert("���ǵ�һ���¼�����֧��ɾ������~");
                return;
            }

            LogCenter.Log(
                $"{notePropertyEdit.@event.eventType}��{notePropertyEdit.@event.@event.startBeats.integer}:{notePropertyEdit.@event.@event.startBeats.molecule}/{notePropertyEdit.@event.@event.startBeats.denominator}�¼���ɾ��");
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
                //�ڶ���
                isFirstTime = false;
                waitForPressureAgain = true;
            } /*����*/
        }
    }
}