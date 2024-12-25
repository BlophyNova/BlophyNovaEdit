using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomSystem;
using Data.ChartEdit;
using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Form.NoteEdit;
using Form.PropertyEdit;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Algorithm;
using UtilityCode.ChartTool;
using UtilityCode.GameUtility;
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;

namespace Form.EventEdit
{
    //这里放处理数据的方法
    public partial class EventEdit
    {

        private void DeleteEvent(EventEditItem eventEditItem)
        {
            List<Data.ChartEdit.Event> events = eventEditItem.eventType switch
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
            events.Remove(eventEditItem.@event);
            onEventDeleted(eventEditItem);
        }
        private void AddNewEvent2EventList(EventEditItem eventEditItem)
        {
            AddNewEvent2EventList(eventEditItem.@event, eventEditItem.eventType);
        }

        private void AddNewEvent2EventList(Data.ChartEdit.Event @event, EventType eventType, bool isPaste = false)
        {
            List<Data.ChartEdit.Event> events = eventType switch
            {
                EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
                _ => null
            };
            if (!isPaste)
            {
                @event.startValue = @event.endValue = events[^1].endValue;
                //@event.Curve = GlobalData.Instance.easeData[0];
                @event.curveIndex = 0;
            }

            events.Add(@event);
            Algorithm.BubbleSort(events, (a, b) => //排序
            {
                if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
                {
                    return 1;
                }

                if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
                {
                    return -1;
                }

                return 0;
            });
            List<Data.ChartData.Event> chartDataEvents = eventType switch
            {
                EventType.CenterX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.centerY,
                EventType.MoveX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
                EventType.Rotate => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.lineAlpha,
                _ => null
            };
            if (chartDataEvents == null)
            {
                SpeedEvent(eventType);
                return;
            }

            ChartTool.RefreshChartEventByChartEditEvent(chartDataEvents, @event);

            if (eventType == EventType.ScaleX && !isPaste) //同步scaleY
            {
                List<Data.ChartEdit.Event> scaleYEvents =
                    GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY;
                scaleYEvents.Add(new Data.ChartEdit.Event(@event));
                Algorithm.BubbleSort(scaleYEvents, (a, b) => //排序
                {
                    if (a.startBeats.ThisStartBPM > b.startBeats.ThisStartBPM)
                    {
                        return 1;
                    }

                    if (a.startBeats.ThisStartBPM < b.startBeats.ThisStartBPM)
                    {
                        return -1;
                    }

                    return 0;
                });
                ChartTool.RefreshChartEventByChartEditEvent(
                    GlobalData.Instance.chartData.boxes[currentBoxID].boxEvents.scaleY,
                    new Data.ChartEdit.Event(@event));
                RefreshEvents(-1);
            }
            //Debug.LogError("错误记忆");
        }
        private void AddEvent2EventClipboard()
        {
            eventClipboard.Clear();
            foreach (EventEditItem selectedEventEditItem in selectBox.TransmitObjects().Cast<EventEditItem>())
            {
                eventClipboard.Add(selectedEventEditItem);
            }

            LogCenter.Log($@"已将{eventClipboard.Count}个事件发送至剪切板！");
        }
        private void SpeedEvent(EventType eventType)
        {
            #region 以下代码为speed事件处理相关专属代码，没啥bug的情况下一个字都别改

            if (eventType != EventType.Speed)
            {
                return;
            }

            List<Data.ChartEdit.Event> filledVoid =
                GameUtility.FillVoid(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed);
            for (int i = 0; i < GlobalData.Instance.chartData.boxes[currentBoxID].lines.Count; i++)
            {
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed = new List<Data.ChartData.Event>();


                ChartTool.ForeachBoxEvents(filledVoid,
                    GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed);
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career = new AnimationCurve
                { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].career.keys = GameUtility
                    .CalculatedSpeedCurve(GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].speed.ToArray())
                    .ToArray();


                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far = new AnimationCurve
                { postWrapMode = WrapMode.ClampForever, preWrapMode = WrapMode.ClampForever };
                GlobalData.Instance.chartData.boxes[currentBoxID].lines[i].far.keys =
                    GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray();
            }

            #endregion
        }

        private void InstNewEvents(List<EventEditItem> eventClipboard, BeatLine beatLine)
        {
            BPM firstEventStartBeats = eventClipboard[0].@event.startBeats;
            for (int i = 0; i < eventClipboard.Count; i++)
            {
                EventEditItem @event = eventClipboard[i];
                Data.ChartEdit.Event copyNewEvent = new(@event.@event);
                copyNewEvent.startBeats = new BPM(beatLine.thisBPM) +
                                          (new BPM(@event.@event.startBeats) - new BPM(firstEventStartBeats));
                copyNewEvent.endBeats = new BPM(beatLine.thisBPM) +
                                        (new BPM(@event.@event.endBeats) - new BPM(firstEventStartBeats));
                if (isCopy)
                {
                    copyNewEvent.IsSelected = false;
                }

                //AddEventAndRefresh(copyNewEvent, currentBoxID);
                AddNewEvent2EventList(copyNewEvent, @event.eventType, true);
                //Debug.LogError("这里有问题");
            }
        }
    }
}