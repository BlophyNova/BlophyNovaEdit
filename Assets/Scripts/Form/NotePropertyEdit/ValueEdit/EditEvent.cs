using Data.ChartEdit;
using Data.Interface;
using Form.EventEdit;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public Event originEvent;
        public Event @event;

        private List<ISelectBoxItem> selectedBoxItems = new();

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
            this.selectedBoxItems = new(selectedBoxItems);
            Set((EventEditItem)selectedBoxItems[0]);
        }
        void Set(EventEditItem eventEditItem)
        {
            Set(eventEditItem.@event);
        }
        void Set(Data.ChartEdit.Event @event)
        {
            originEvent = new(@event);
            this.@event = @event;
            SetNoteValue2Form();
            notePropertyEdit.editNote.gameObject.SetActive(false);
            gameObject.SetActive(true);

        }

        private void SetNoteValue2Form()
        {
            startTime.SetTextWithoutNotify(
                $"{@event.startBeats.integer}:{@event.startBeats.molecule}/{@event.startBeats.denominator}");
            endTime.SetTextWithoutNotify(
                $"{@event.endBeats.integer}:{@event.endBeats.molecule}/{@event.endBeats.denominator}");
            Func<float, float> func = @event.eventType switch
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
            startValue.SetTextWithoutNotify($"{func(@event.startValue)}");
            endValue.SetTextWithoutNotify($"{func(@event.endValue)}");
            ease.SetValueWithoutNotify(@event.curveIndex);
        }

    }
}
