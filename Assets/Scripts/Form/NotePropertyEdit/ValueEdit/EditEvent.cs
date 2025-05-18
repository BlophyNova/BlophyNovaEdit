using System;
using System.Collections.Generic;
using System.Linq;
using Data.Interface;
using Form.EventEdit;
using Form.NotePropertyEdit.ValueEdit.Ease;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ValueConvert.ValueConvert;

namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditEvent : MonoBehaviour
    {
        public RectTransform viewport;
        public VerticalLayoutGroup verticalLayoutGroup;
        public List<RectTransform> contentList;

        public NotePropertyEdit notePropertyEdit;

        public List<Event> originEvents;
        public List<Event> events;

        public TextMeshProUGUI eventEditText;
        public TMP_InputField startTime;
        public TMP_InputField endTime;
        public TMP_InputField startValue;
        public TMP_InputField endValue;
        public Toggle syncEvent;
        public EaseEdit easeEdit;

        /// <summary>
        ///     多选编辑用的玩意，还没写.jpg
        /// </summary>
        /// <param name="selectedBoxItems"></param>
        public void Set(List<ISelectBoxItem> selectedBoxItems)
        {
            if (selectedBoxItems.Count <= 0)
            {
                return;
            }

            eventEditText.text = $"事件编辑 {selectedBoxItems.Count}";
            List<EventEditItem> eventEditItems = selectedBoxItems.Cast<EventEditItem>().ToList();
            originEvents = new List<Event>();
            events.Clear();
            foreach (EventEditItem eventEditItem in eventEditItems)
            {
                originEvents.Add(new Event(eventEditItem.@event));
                events.Add(eventEditItem.@event);
            }

            SetNoteValue2Form();
            notePropertyEdit.EditNote.gameObject.SetActive(false);
            gameObject.SetActive(true);
            transform.SetAsLastSibling();
        }

        private void SetNoteValue2Form()
        {
            startTime.SetTextWithoutNotify(
                $"{events[0].startBeats.integer}:{events[0].startBeats.molecule}/{events[0].startBeats.denominator}");
            endTime.SetTextWithoutNotify(
                $"{events[0].endBeats.integer}:{events[0].endBeats.molecule}/{events[0].endBeats.denominator}");
            Func<float, float> func = events[0].eventType switch
            {
                EventType.CenterX => value => CenterXYSnapTo16_9(value, true),
                EventType.CenterY => value => CenterXYSnapTo16_9(value, false),
                EventType.MoveX => value => MoveXYSnapTo16_9(value, true),
                EventType.MoveY => value => MoveXYSnapTo16_9(value, false),
                EventType.ScaleX => value => ScaleXYSnapTo16_9(value, true),
                EventType.ScaleY => value => ScaleXYSnapTo16_9(value, false),
                EventType.Alpha => value => AlphaSnapTo0_255(value, true),
                EventType.LineAlpha => value => AlphaSnapTo0_255(value, false),
                _ => value => value
            };
            syncEvent.SetIsOnWithoutNotify(events[0].isSyncEvent);
            startValue.SetTextWithoutNotify($"{func(events[0].startValue)}");
            endValue.SetTextWithoutNotify($"{func(events[0].endValue)}");
            if (events[0].isCustomCurve)
            {
                easeEdit.easeStyle.value = 1;
                if (GlobalData.Instance.chartEditData.customCurves[events[0].curveIndex].isDeleted)
                {
                    events[0].isCustomCurve = false;
                    events[0].curveIndex = 0;
                    Finally();
                }

                easeEdit.SetCustomValueWithoutNotify(events[0].curveIndex + 1);
                easeEdit.visualEase.EaseEdit_onValueChanged(events[0].curveIndex + 1);
            }
            else
            {
                easeEdit.easeStyle.value = 0;
                easeEdit.SetValueWithoutNotify(events[0].curveIndex);
                easeEdit.visualEase.EaseEdit_onValueChanged(events[0].curveIndex);
            }

            if (events[0].isSyncEvent)
            {
                endValue.interactable = false;
                endValue.SetTextWithoutNotify($"{endValue.text}-不可编辑");
            }
            else
            {
                endValue.interactable = true;
            }
        }
    }
}