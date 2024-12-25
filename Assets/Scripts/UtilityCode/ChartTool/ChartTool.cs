using System.Collections.Generic;
using Data.ChartData;
using Data.ChartEdit;
using Data.EaseData;
using Manager;
using UnityEngine;
using Box = Data.ChartData.Box;
using BoxEvents = Data.ChartEdit.BoxEvents;
using ChartData = Data.ChartEdit.ChartData;
using Event = Data.ChartData.Event;
using Line = Data.ChartEdit.Line;
using Note = Data.ChartData.Note;

namespace UtilityCode.ChartTool
{
    public class ChartTool
    {
        /// <summary>
        ///     添加NoteEdit到chartEditNotes和chartDataNotes
        /// </summary>
        /// <param name="noteEdit"></param>
        /// <param name="boxID"></param>
        /// <param name="lineID"></param>
        /// <param name="chartEditData"></param>
        /// <param name="chartData"></param>
        public static void AddNoteEdit2ChartData(Data.ChartEdit.Note noteEdit, int boxID, int lineID,
            ChartData chartEditData, Data.ChartData.ChartData chartData)
        {
            int index_noteEdits = Algorithm.Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[lineID].onlineNotes,
                m => m.HitBeats.ThisStartBPM < noteEdit.HitBeats.ThisStartBPM, false);
            chartEditData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteEdits, noteEdit);

            Note note = new(noteEdit);
            int index_noteChartData = Algorithm.Algorithm.BinarySearch(chartData.boxes[boxID].lines[lineID].onlineNotes,
                m => m.hitTime < note.hitTime, false);
            if (noteEdit.noteType != NoteType.Point)
            {
                chartData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteChartData, note);
            }
            else
            {
                chartData.boxes[boxID].lines[4].onlineNotes.Insert(
                    Algorithm.Algorithm.BinarySearch(chartData.boxes[boxID].lines[4].onlineNotes,
                        m => m.hitTime < note.hitTime, false), note);
            }
            //else
            //{
            //    int index_pointEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[4].onlineNotes, m => m.hitBeats.ThisStartBPM < noteEdit.hitBeats.ThisStartBPM, false);
            //    chartData.boxes[boxID].lines[4].onlineNotes.Insert(index_pointEdits, note);
            //}
        }

        //public static void RefreshChartEventByChartEditEvent(List<Event> chartDataEvent,
        public static void InsertEditEvent2PlayerEvent(List<Event> chartDataEvent,
            Data.ChartEdit.Event chartEditDataEvent)
        {
            int index_noteEdits = Algorithm.Algorithm.BinarySearch(chartDataEvent,
                m => m.startTime <
                     BPMManager.Instance.GetSecondsTimeByBeats(chartEditDataEvent.startBeats.ThisStartBPM), false);
            Event @event = new(chartEditDataEvent);
            chartDataEvent.Insert(index_noteEdits, @event);
        }

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
                }
            };
            chartEditBox.boxEvents.scaleX.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 2.7f, endValue = 2.7f });
            chartEditBox.boxEvents.scaleY.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 2.7f, endValue = 2.7f });
            chartEditBox.boxEvents.moveX.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.moveY.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.centerX.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = .5f, endValue = .5f });
            chartEditBox.boxEvents.centerY.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = .5f, endValue = .5f });
            chartEditBox.boxEvents.alpha.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 1, endValue = 1 });
            chartEditBox.boxEvents.lineAlpha.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.rotate.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 0, endValue = 0 });
            chartEditBox.boxEvents.speed.Add(new Data.ChartEdit.Event
                { startBeats = BPM.Zero, endBeats = new BPM(1, 0, 1), startValue = 3, endValue = 3 });
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
                ConvertEditLine2ChartDataLine(box, chartDataBox, i);
            }

            ConvertAllEditEvents2ChartDataEvents(box, chartDataBox);


            return chartDataBox;
        }

        public static void ConvertAllEditEvents2ChartDataEvents(Data.ChartEdit.Box box, Box chartDataBox)
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
            for (int i = 0; i < chartDataBox.lines.Count; i++)
            {
                List<Data.ChartEdit.Event> filledVoid = GameUtility.GameUtility.FillVoid(box.boxEvents.speed);
                chartDataBox.lines[i].speed = new List<Event>();
                ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
                chartDataBox.lines[i].career = new AnimationCurve
                {
                    postWrapMode = WrapMode.ClampForever,
                    preWrapMode = WrapMode.ClampForever,
                    keys = GameUtility.GameUtility.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray()
                };
                chartDataBox.lines[i].far = new AnimationCurve
                {
                    postWrapMode = WrapMode.ClampForever,
                    preWrapMode = WrapMode.ClampForever,
                    keys = GameUtility.GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray()
                };
            }
        }

        public static void ConvertEditLine2ChartDataLine(Data.ChartEdit.Box box, Box chartDataBox, int i)
        {
            foreach (Data.ChartEdit.Note item in box.lines[i].offlineNotes)
            {
                Note newChartDataNote = new(item);
                if (newChartDataNote.noteType == NoteType.Point)
                {
                    chartDataBox.lines[4].offlineNotes.Add(newChartDataNote);
                }
                else
                {
                    chartDataBox.lines[i].offlineNotes.Add(newChartDataNote);
                }
            }

            foreach (Data.ChartEdit.Note item in box.lines[i].onlineNotes)
            {
                Note newChartDataNote = new(item);
                if (newChartDataNote.noteType == NoteType.Point)
                {
                    chartDataBox.lines[4].onlineNotes.Add(newChartDataNote);
                }
                else
                {
                    chartDataBox.lines[i].onlineNotes.Add(newChartDataNote);
                }
            }
        }

        public static void ForeachBoxEvents(List<Data.ChartEdit.Event> editBoxEvent, List<Event> chartDataBoxEvent)
        {
            chartDataBoxEvent.Clear();
            foreach (Data.ChartEdit.Event item in editBoxEvent)
            {
                chartDataBoxEvent.Add(new Event(item));
            }
        }
    }
}