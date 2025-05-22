using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomSystem;
using Cysharp.Threading.Tasks;
using Data.ChartEdit;
using Data.Interface;
using Form.NoteEdit;
using Form.PropertyEdit;
using Manager;
using Newtonsoft.Json;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;

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
            DeleteEventFromUI();
        }

        private void PasteEvent()
        {
            Debug.Log("粘贴事件");

            FindNearBeatLineAndEventVerticalLine(out BeatLine beatLine, out EventVerticalLine verticalLine);
            string rawData = GUIUtility.systemCopyBuffer;
            try
            {
                List<Event> newEvents = JsonConvert.DeserializeObject<List<Event>>(rawData);
                Steps.Instance.Add(PasteUndo, PasteRedo, default);
                PasteRedo();
                isCopy = true;
                return;

                void PasteUndo()
                {
                    DeleteEvents(newEvents, currentBoxID);
                }

                void PasteRedo()
                {
                    AlignEvents(newEvents, beatLine.thisBPM);
                    BatchEvents(newEvents, @event => @event.IsSelected = false);
                    BatchEvents(newEvents, @event => @event.id = TimeUtility.GetCurrentTime());
                    List<Event> instNewEvents = AddEvents(newEvents, currentBoxID, true);
                    eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                }
            }
            catch (JsonException je)
            {
            }
        }


        private void Move(bool isMoving) 
        {
            this.isMoving = isMoving;
            MoveAsync(isMoving);
        }
        async void MoveAsync(bool isMoving)
        {
            if (!isMoving||selectBox.selectedBoxItems.Count==0)
            {
                return;
            }
            FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine,
                out _);
            BPM nearBeatLineBpm = new(nearBeatLine.thisBPM);
            while (this.isMoving && FocusIsMe && selectBox.selectedBoxItems.Count != 0)
            {
                await UniTask.NextFrame();//啊哈，没错，引入了UniTask导致的，真方便（
                FindNearBeatLineAndEventVerticalLine(out nearBeatLine,
                    out _);
                try
                {
                    nearBeatLineBpm = new(nearBeatLine.thisBPM);
                }
                catch
                {
                    // ignored
                }

                //这，这对吗？还是不要频繁刷新比较好，想想别的方法吧
                BPM firstBpm = ((EventEditItem)selectBox.selectedBoxItems[0]).@event.startBeats;
                foreach (EventEditItem eventEditItem in selectBox.selectedBoxItems.Cast<EventEditItem>())
                {
                    RectTransform rect=eventEditItem.thisEventEditItemRect;
                    BPM newBpm = new BPM(nearBeatLineBpm) + (new BPM(eventEditItem.@event.startBeats) - new BPM(firstBpm));
                    float currentSecondsTime =
                        BPMManager.Instance.GetSecondsTimeByBeats(newBpm.ThisStartBPM);
                    float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);
                    rect.localPosition = new(rect.localPosition.x,positionY);
                }
                
            }
            //我有一计，放这里不就好了？
            
            List<Event> newEvents = null;
            List<Event> deletedEvents = null;
            List<Event> selectedEvents = GetSelectedEvents();
            newEvents = CopyEvents(selectedEvents, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            BPM nearBpm = nearBeatLine.thisBPM;
            bpm = new BPM(nearBpm);
            
            AlignEvents(newEvents, bpm);
            AddEvents(newEvents, currentBoxID, true);
            eventEditItems.AddRange(AddEvents2UI(newEvents));

            deletedEvents = DeleteEvents(selectedEvents, currentBoxID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Event> instNewEvents = AddEvents(deletedEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }

            void Redo()
            {
                List<Event> instNewEvents = AddEvents(newEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }
        private void MoveUp()
        {
            List<Event> newEvents = null;
            List<Event> deletedEvents = null;
            List<Event> selectedEvents = GetSelectedEvents();
            newEvents = CopyEvents(selectedEvents, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            BPM nearBPM =
                FindNearBeatLine(
                    (Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedEvents[0]
                        .chartEditEvent.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new BPM(nearBPM);
            if ((bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM) ||
                bpm.ThisStartBPM > basicLine.nextBPMWithAriseLine.ThisStartBPM)
            {
                bpm.AddOneBeat();
            }

            AlignEvents(newEvents, bpm);
            AddEvents(newEvents, currentBoxID, true);
            eventEditItems.AddRange(AddEvents2UI(newEvents));

            deletedEvents = DeleteEvents(selectedEvents, currentBoxID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Event> instNewEvents = AddEvents(deletedEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }

            void Redo()
            {
                List<Event> instNewEvents = AddEvents(newEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }

        private List<Event> GetSelectedEvents()
        {
            IEnumerable<EventEditItem> selectedEventEditItems = selectBox.TransmitObjects().Cast<EventEditItem>();

            return selectedEventEditItems.Select(item => item.@event).ToList();
        }

        private void MoveDown()
        {
            List<Event> newEvents = null;
            List<Event> deletedEvents = null;
            List<Event> selectedEvents = GetSelectedEvents();
            newEvents = CopyEvents(selectedEvents, currentBoxID, true);
            BPM bpm = new(newEvents[0].startBeats);
            BPM nearBPM =
                FindNearBeatLine(
                    (Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedEvents[0]
                        .chartEditEvent.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new BPM(nearBPM);
            if ((bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM) ||
                bpm.ThisStartBPM <
                BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime))
            {
                bpm.SubtractionOneBeat();
            }

            AlignEvents(newEvents, bpm);
            AddEvents(newEvents, currentBoxID, true);
            eventEditItems.AddRange(AddEvents2UI(newEvents));

            deletedEvents = DeleteEvents(selectedEvents, currentBoxID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Event> instNewEvents = AddEvents(deletedEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(newEvents, currentBoxID, isCopy);
            }

            void Redo()
            {
                List<Event> instNewEvents = AddEvents(newEvents, currentBoxID, true);
                eventEditItems.AddRange(AddEvents2UI(instNewEvents));
                DeleteEvents(deletedEvents, currentBoxID, isCopy);
            }
        }

        private void DeleteEventFromUI()
        {
            IEnumerable<EventEditItem> selectesBoxes = selectBox.TransmitObjects().Cast<EventEditItem>();
            List<Event> selectedEvents = new();
            foreach (EventEditItem eventEditItem in selectesBoxes)
            {
                selectedEvents.Add(eventEditItem.@event);
            }

            List<Event> deletedEvents = DeleteEvents(selectedEvents, currentBoxID);
            //RefreshAll();
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Event> newEvents = AddEvents(deletedEvents, currentBoxID, true);
                BatchEvents(newEvents, @event => @event.IsSelected = false);
                eventEditItems.AddRange(AddEvents2UI(newEvents));
            }

            void Redo()
            {
                DeleteEvents(deletedEvents, currentBoxID);
            }
        }

        private void AddSyncEventFromUI()
        {
            AddEventFromUI(true);
        }

        private void AddCommonEventFromUI()
        {
            AddEventFromUI();
        }

        private void AddEventFromUI(bool isSyncEvent = false)
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
                newEventEditItem.transform.localPosition = new Vector3(nearEventVerticalLine.transform.localPosition.x,
                    nearBeatLine.transform.localPosition.y);
                newEventEditItem.easeLine.enabled = false;
                newEventEditItem.@event.chartEditEvent = newEventEditItem;
                newEventEditItem.@event.startBeats = new BPM(nearBeatLine.thisBPM);
                newEventEditItem.@event.eventType = nearEventVerticalLine.eventType;
                newEventEditItem.@event.isSyncEvent = isSyncEvent;
                newEventEditItem.@event.id = TimeUtility.GetCurrentTime();
                StartCoroutine(WaitForPressureAgain(newEventEditItem));
            }
            else if (isFirstTime)
            {
                //第二次
                isFirstTime = false;
                waitForPressureAgain = true;
            } /*报错*/
        }

        private List<EventEditItem> AddEvents2UI(List<Event> keyValueList)
        {
            List<EventEditItem> eventEditItems = new();
            for (int i = 0; i < keyValueList.Count; i++)
            {
                foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
                {
                    if (eventVerticalLine.eventType == keyValueList[i].eventType)
                    {
                        EventEditItem newEventEditItem = AddEvent2UI(keyValueList[i], keyValueList[i].eventType,
                            eventVerticalLine.transform.localPosition.x);
                        eventEditItems.Add(newEventEditItem);
                    }
                }
            }

            onEventsAdded2UI(eventEditItems);
            return eventEditItems;
        }

        private EventEditItem AddEvent2UI(Event @event, EventType eventType, float localPositionX)
        {
            EventEditItem newEventEditItem =
                Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);


            float currentSecondsTime =
                BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM);
            float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

            newEventEditItem.transform.localPosition = new Vector3(localPositionX, positionY);

            float endBeatsSecondsTime =
                BPMManager.Instance.GetSecondsTimeByBeats(@event.endBeats.ThisStartBPM);
            float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(endBeatsSecondsTime);

            newEventEditItem.labelWindow = labelWindow;
            newEventEditItem.thisEventEditItemRect.sizeDelta = new Vector2(
                Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition),
                endBeatsPositionY - positionY);
            newEventEditItem.@event = @event;
            newEventEditItem.@event.eventType = eventType;
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
                if (item.@event.eventType == eventType)
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
    }
}