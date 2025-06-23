using System.Collections.Generic;
using Data.ChartEdit;
using Data.EaseData;
using UnityEngine;
using Box = Data.ChartData.Box;
using BoxEvents = Data.ChartEdit.BoxEvents;
using ChartData = Data.ChartEdit.ChartData;
using Event = Data.ChartData.Event;
using EventType = Data.Enumerate.EventType;
using GU = UtilityCode.GameUtility.GameUtility;
using Line = Data.ChartEdit.Line;
using Note = Data.ChartData.Note;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

namespace UtilityCode.ChartTool
{
    //这个类应该只负责生成新的ChartEdit数据和转换ChartEdit数据到ChartData数据
    public class ChartTool
    {
        /// <summary>
        ///     创建一个新的Edit谱面
        /// </summary>
        /// <param name="chartEditData"></param>
        /// <param name="easeData"></param>
        public static void CreateNewChart(ChartData chartEditData, List<EaseData> easeData)
        {
            chartEditData.boxes = new List<Data.ChartEdit.Box>();
            Data.ChartEdit.Box chartEditBox = CreateNewBox(easeData);
            chartEditData.boxes.Add(chartEditBox);
        }

        /// <summary>
        ///     创建一个新框
        /// </summary>
        /// <param name="easeData"></param>
        /// <returns></returns>
        public static Data.ChartEdit.Box CreateNewBox(List<EaseData> easeData)
        {
            Data.ChartEdit.Box chartEditBox = new()
            {
                lines = new List<Line> { new(), new(), new(), new(), new() },
                boxEvents = new BoxEvents
                {
                    scaleX = new List<Data.ChartEdit.Event>(),
                    scaleY = new List<Data.ChartEdit.Event>(),
                    moveX = new List<Data.ChartEdit.Event>(),
                    moveY = new List<Data.ChartEdit.Event>(),
                    centerX = new List<Data.ChartEdit.Event>(),
                    centerY = new List<Data.ChartEdit.Event>(),
                    alpha = new List<Data.ChartEdit.Event>(),
                    lineAlpha = new List<Data.ChartEdit.Event>(),
                    rotate = new List<Data.ChartEdit.Event>(),
                    speed = new List<Data.ChartEdit.Event>()
                },
                id = TimeUtility.TimeUtility.GetCurrentTime()
            };
            chartEditBox.boxEvents.scaleX.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 2.7f, endValue = 2.7f,
                disallowDelete = true, disallowMove = true, isSyncEvent = true,
                id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.scaleY.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 2.7f, endValue = 2.7f,
                disallowDelete = true, disallowMove = true, isSyncEvent = true,
                id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.moveX.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.moveY.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.centerX.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = .5f, endValue = .5f,
                disallowDelete = true, disallowMove = true, isSyncEvent = true,
                id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.centerY.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = .5f, endValue = .5f,
                disallowDelete = true, disallowMove = true, isSyncEvent = true,
                id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.alpha.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1),
                startValue = GlobalData.Instance.generalData.NewBoxAlpha ? 0 : 1,
                endValue = GlobalData.Instance.generalData.NewBoxAlpha ? 0 : 1, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.lineAlpha.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.rotate.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            chartEditBox.boxEvents.speed.Add(new Data.ChartEdit.Event
            {
                startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 3, endValue = 3, disallowDelete = true,
                disallowMove = true, isSyncEvent = true, id = TimeUtility.TimeUtility.GetCurrentTime()
            });
            for (int i = 0; i < chartEditBox.lines.Count; i++)
            {
                chartEditBox.lines[i].offlineNotes = new List<Data.ChartEdit.Note>();
                chartEditBox.lines[i].onlineNotes = new List<Data.ChartEdit.Note>();
            }

            return chartEditBox;
        }

        /// <summary>
        ///     转换EditBox到谱面预览器的Box
        /// </summary>
        /// <param name="boxes"></param>
        /// <returns></returns>
        public static List<Box> ConvertChartEdit2ChartData(List<Data.ChartEdit.Box> boxes)
        {
            List<Box> result = new();
            for (int index = 0; index < boxes.Count; index++)
            {
                Box chartDataBox = ConvertEditBox2ChartDataBox(boxes[index]);

                result.Add(chartDataBox);
            }

            return result;
        }

        public static Box ConvertEditBox2ChartDataBox(Data.ChartEdit.Box box)
        {
            Box chartDataBox = new()
            {
                lines = new List<Data.ChartData.Line>
                {
                    new() { offlineNotes = new List<Note>(), onlineNotes = new List<Note>() },
                    new() { offlineNotes = new List<Note>(), onlineNotes = new List<Note>() },
                    new() { offlineNotes = new List<Note>(), onlineNotes = new List<Note>() },
                    new() { offlineNotes = new List<Note>(), onlineNotes = new List<Note>() },
                    new() { offlineNotes = new List<Note>(), onlineNotes = new List<Note>() }
                },
                boxEvents = new Data.ChartData.BoxEvents
                {
                    scaleX = new List<Event>(),
                    scaleY = new List<Event>(),
                    moveX = new List<Event>(),
                    moveY = new List<Event>(),
                    centerX = new List<Event>(),
                    centerY = new List<Event>(),
                    alpha = new List<Event>(),
                    lineAlpha = new List<Event>(),
                    rotate = new List<Event>()
                }
            };
            //var box = boxes[index];
            for (int i = 0; i < chartDataBox.lines.Count; i++)
            {
                ConvertLines(box, chartDataBox, i);
            }

            ConvertAllEvents(box, chartDataBox);


            return chartDataBox;
        }

        public static void ConvertLines(Data.ChartEdit.Box box, Box chartDataBox, int i)
        {
            ConvertLine(box.lines[i].onlineNotes, chartDataBox.lines[i].onlineNotes);
            ConvertLine(box.lines[i].offlineNotes, chartDataBox.lines[i].offlineNotes);
        }

        public static void ConvertLine(List<Data.ChartEdit.Note> chartEditNotes, List<Note> chartDataNotes)
        {
            chartDataNotes.Clear();
            foreach (Data.ChartEdit.Note chartEditNote in chartEditNotes)
            {
                Note newChartDataNote = new(chartEditNote);
                chartEditNote.chartDataNote = newChartDataNote;
                chartDataNotes.Add(newChartDataNote);
            }
        }

        public static void ConvertAllEvents(Data.ChartEdit.Box box, Box chartDataBox)
        {
            ForeachBoxEvents(box.boxEvents.scaleX, chartDataBox.boxEvents.scaleX);
            ForeachBoxEvents(box.boxEvents.scaleY, chartDataBox.boxEvents.scaleY);
            ForeachBoxEvents(box.boxEvents.moveX, chartDataBox.boxEvents.moveX);
            ForeachBoxEvents(box.boxEvents.moveY, chartDataBox.boxEvents.moveY);
            ForeachBoxEvents(box.boxEvents.centerX, chartDataBox.boxEvents.centerX);
            ForeachBoxEvents(box.boxEvents.centerY, chartDataBox.boxEvents.centerY);
            ForeachBoxEvents(box.boxEvents.alpha, chartDataBox.boxEvents.alpha);
            ForeachBoxEvents(box.boxEvents.lineAlpha, chartDataBox.boxEvents.lineAlpha);
            ForeachBoxEvents(box.boxEvents.rotate, chartDataBox.boxEvents.rotate);
            ForeachSpeedEvents(box, chartDataBox);
        }

        public static void ConvertEvents(Data.ChartEdit.Box box, Box chartDataBox, EventType eventType, int boxID)
        {
            if (eventType == EventType.Speed)
            {
                ForeachSpeedEvents(box, chartDataBox);
                return;
            }

            List<Data.ChartEdit.Event> chartEditEvents =
                FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[boxID], eventType);
            List<Event> chartDataEvents = FindChartDataEventList(GlobalData.Instance.chartData.boxes[boxID], eventType);
            ForeachBoxEvents(chartEditEvents, chartDataEvents);
        }

        public static List<Data.ChartEdit.Event> FindChartEditEventList(Data.ChartEdit.Box box, EventType eventType)
        {
            return eventType switch
            {
                EventType.Speed => box.boxEvents.speed,
                EventType.CenterX => box.boxEvents.centerX,
                EventType.CenterY => box.boxEvents.centerY,
                EventType.MoveX => box.boxEvents.moveX,
                EventType.MoveY => box.boxEvents.moveY,
                EventType.ScaleX => box.boxEvents.scaleX,
                EventType.ScaleY => box.boxEvents.scaleY,
                EventType.Rotate => box.boxEvents.rotate,
                EventType.Alpha => box.boxEvents.alpha,
                EventType.LineAlpha => box.boxEvents.lineAlpha,
                _ => null
            };
        }

        public static List<Event> FindChartDataEventList(Box box, EventType eventType)
        {
            return eventType switch
            {
                EventType.CenterX => box.boxEvents.centerX,
                EventType.CenterY => box.boxEvents.centerY,
                EventType.MoveX => box.boxEvents.moveX,
                EventType.MoveY => box.boxEvents.moveY,
                EventType.ScaleX => box.boxEvents.scaleX,
                EventType.ScaleY => box.boxEvents.scaleY,
                EventType.Rotate => box.boxEvents.rotate,
                EventType.Alpha => box.boxEvents.alpha,
                EventType.LineAlpha => box.boxEvents.lineAlpha,
                _ => null
            };
        }

        public static void ForeachSpeedEvents(Data.ChartEdit.Box box, Box chartDataBox)
        {
            List<Data.ChartEdit.Event> filledVoid = GU.FillVoid(box.boxEvents.speed);
            for (int i = 0; i < chartDataBox.lines.Count; i++)
            {
                chartDataBox.lines[i].speed = new List<Event>();
                ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
                //chartDataBox.lines[i].career = new()
                //{
                //    postWrapMode = WrapMode.ClampForever,
                //    preWrapMode = WrapMode.ClampForever,
                //    keys = GU.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray()
                //};
                chartDataBox.lines[i].far = new AnimationCurve
                {
                    postWrapMode = WrapMode.ClampForever,
                    preWrapMode = WrapMode.ClampForever,
                    keys = GU.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray()
                };
            }
        }

        public static void ForeachBoxEvents(List<Data.ChartEdit.Event> editBoxEvent, List<Event> chartDataBoxEvent)
        {
            chartDataBoxEvent.Clear();
            foreach (Data.ChartEdit.Event item in editBoxEvent)
            {
                Event newEvent = new(item);
                item.chartDataEvent = newEvent;
                chartDataBoxEvent.Add(newEvent);
            }
        }
    }
}