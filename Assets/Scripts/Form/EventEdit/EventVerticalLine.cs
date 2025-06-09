using System;
using System.Collections.Generic;
using Data.ChartEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using TMPro;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ValueConvert.ValueConvert;
using Event = Data.ChartEdit.Event;

namespace Form.EventEdit
{
    public class EventVerticalLine : MonoBehaviour
    {
        public float defaultValue;
        public int currentBoxID;


        public EventEdit eventEdit;
        public TextMeshProUGUI displayEventTypeName;
        public TextMeshProUGUI displayCurrentEventValue;

        public EventType eventType;
        private Func<float, float> func;

        public List<Event> events
        {
            get
            {
                BoxEvents boxEvents = GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents;
                return eventType switch
                {
                    EventType.CenterX => boxEvents.centerX,
                    EventType.CenterY => boxEvents.centerY,
                    EventType.MoveX => boxEvents.moveX,
                    EventType.MoveY => boxEvents.moveY,
                    EventType.ScaleX => boxEvents.scaleX,
                    EventType.ScaleY => boxEvents.scaleY,
                    EventType.Rotate => boxEvents.rotate,
                    EventType.Alpha => boxEvents.alpha,
                    EventType.LineAlpha => boxEvents.lineAlpha,
                    EventType.Speed => boxEvents.speed,
                    _ => throw new Exception("找不到事件")
                };
            }
        }

        // Start is called before the first frame update
        private void Start()
        {
            displayEventTypeName.text = eventType.ToString();
            eventEdit.onIndexChanged += eventEdit_onIndexChanged;
            func = eventType switch
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
        }

        private void Update()
        {
            displayCurrentEventValue.text =
                $"{(int)func(CalculateCurrentValue(events, BPMManager.Instance.GetCurrentBeatsWithSecondsTime((float)ProgressManager.Instance.CurrentTime), ref defaultValue))}";
        }

        /// <summary>
        ///     计算当前数值
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private static float CalculateCurrentValue(List<Event> events, float currentBeats, ref float defaultValue)
        {
            if (events.Count <= 0 || currentBeats < events[0].startBeats.ThisStartBPM)
            {
                return defaultValue;
            }

            int eventIndex = Algorithm.BinarySearch(events, IsCurrentEvent, true, ref currentBeats); //找到当前时间下，应该是哪个事件

            if (currentBeats > events[eventIndex].endBeats.ThisStartBPM)
            {
                return events[eventIndex].endValue;
            }

            return GameUtility.GetValueWithEvent(events[eventIndex], currentBeats); //拿到事件后根据时间Get到当前值

            static bool IsCurrentEvent(Event m, ref float currentTime)
            {
                return currentTime >= m.startBeats.ThisStartBPM;
            }
        }

        public void eventEdit_onIndexChanged(int boxID)
        {
            currentBoxID = boxID;
        }
    }
}