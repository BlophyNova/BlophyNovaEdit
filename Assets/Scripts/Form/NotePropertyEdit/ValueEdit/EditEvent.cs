using Data.ChartEdit;
using Data.Interface;
using Form.EventEdit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
namespace Form.NotePropertyEdit.ValueEdit
{
    public partial class EditEvent : MonoBehaviour
    {
        public NotePropertyEdit notePropertyEdit;

        public List<Event> originEvents;
        public List<Event> events;

        public TextMeshProUGUI eventEditText;
        public TMP_InputField startTime;
        public TMP_InputField endTime;
        public TMP_InputField startValue;
        public TMP_InputField endValue;
        public Toggle syncEvent;
        public TMP_InputField easeIndex;
        public TMP_Dropdown easeIO;
        public TMP_Dropdown ease;
        /// <summary>
        /// 多选编辑用的玩意，还没写.jpg
        /// </summary>
        /// <param name="selectedBoxItems"></param>
        public void Set(List<ISelectBoxItem> selectedBoxItems)
        {
            if (selectedBoxItems.Count <= 0) return;
            eventEditText.text = $"事件编辑 {selectedBoxItems.Count}"; 
            events=selectedBoxItems.Cast<Event>().ToList();
            originEvents = new();
            foreach (Event @event in events)
            {
                originEvents.Add(new(@event));
            }
            SetNoteValue2Form();
            notePropertyEdit.EditNote.gameObject.SetActive(false);
            gameObject.SetActive(true);
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
            startValue.SetTextWithoutNotify($"{func(events[0].startValue)}");
            endValue.SetTextWithoutNotify($"{func(events[0].endValue)}");
            ease.SetValueWithoutNotify(events[0].curveIndex);
        }

    }
}
