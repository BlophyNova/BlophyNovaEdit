using Data.ChartData;
using Data.ChartEdit;
using Data.EaseData;
using Manager;
using System.Collections.Generic;
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
            chartEditData.boxes = new();
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
                lines = new(){ new(), new(), new(), new(), new() },
                boxEvents = new()
                {
                    scaleX = new(),
                    scaleY = new(),
                    moveX = new(),
                    moveY = new(),
                    centerX = new(),
                    centerY = new(),
                    alpha = new(),
                    lineAlpha = new(),
                    rotate = new(),
                    speed = new()
                }
            };
            chartEditBox.boxEvents.scaleX.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 2.7f, endValue = 2.7f });
            chartEditBox.boxEvents.scaleY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 2.7f, endValue = 2.7f });
            chartEditBox.boxEvents.moveX.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.moveY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.centerX.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = .5f, endValue = .5f });
            chartEditBox.boxEvents.centerY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = .5f, endValue = .5f });
            chartEditBox.boxEvents.alpha.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 1, endValue = 1 });
            chartEditBox.boxEvents.lineAlpha.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.rotate.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.speed.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 3, endValue = 3 });
            for (int i = 0; i < chartEditBox.lines.Count; i++)
            {
                chartEditBox.lines[i].offlineNotes = new();
                chartEditBox.lines[i].onlineNotes = new();
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
                lines = new()
                {
                    new() { offlineNotes = new(), onlineNotes = new() },
                    new() { offlineNotes = new(), onlineNotes = new() },
                    new() { offlineNotes = new(), onlineNotes = new() },
                    new() { offlineNotes = new(), onlineNotes = new() },
                    new() { offlineNotes = new(), onlineNotes = new() }
                },
                boxEvents = new()
                {
                    scaleX = new(),
                    scaleY = new(),
                    moveX = new(),
                    moveY = new(),
                    centerX = new(),
                    centerY = new(),
                    alpha = new(),
                    lineAlpha = new(),
                    rotate = new()
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

        public static void ConvertLine(List<Data.ChartEdit.Note> chartEditNotes,List<Data.ChartData.Note> chartDataNotes)
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
        public static void ConvertEvents(Data.ChartEdit.Box box, Box chartDataBox,EventType eventType,int boxID)
        {
            if (eventType == EventType.Speed)
            {
                ForeachSpeedEvents(box, chartDataBox);
                return;
            }
            List<Data.ChartEdit.Event> chartEditEvents=FindChartEditEventList(GlobalData.Instance.chartEditData.boxes[boxID], eventType);
            List<Data.ChartData.Event> chartDataEvents=FindChartDataEventList(GlobalData.Instance.chartData.boxes[boxID], eventType);
            ForeachBoxEvents(chartEditEvents,chartDataEvents);
        }

        public static List<Data.ChartEdit.Event> FindChartEditEventList(Data.ChartEdit.Box box,EventType eventType)
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
        public static List<Event> FindChartDataEventList( Box box,EventType eventType)
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
                chartDataBox.lines[i].speed = new();
                ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
                //chartDataBox.lines[i].career = new()
                //{
                //    postWrapMode = WrapMode.ClampForever,
                //    preWrapMode = WrapMode.ClampForever,
                //    keys = GU.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray()
                //};
                chartDataBox.lines[i].far = new()
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