using Data.ChartData;
using Data.ChartEdit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using Box = Data.ChartData.Box;

public class ChartTool
{
    /// <summary>
    /// 添加NoteEdit到chartEditNotes和chartDataNotes
    /// </summary>
    /// <param name="noteEdit"></param>
    /// <param name="boxID"></param>
    /// <param name="lineID"></param>
    /// <param name="chartEditData"></param>
    /// <param name="chartData"></param>
    public static void AddNoteEdit2ChartData(Data.ChartEdit.Note noteEdit, int boxID, int lineID, Data.ChartEdit.ChartData chartEditData, Data.ChartData.ChartData chartData)
    {
        int index_noteEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[lineID].onlineNotes, m => m.HitBeats.ThisStartBPM < noteEdit.HitBeats.ThisStartBPM, false);
        chartEditData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteEdits, noteEdit);

        Data.ChartData.Note note = new(noteEdit);
        int index_noteChartData = Algorithm.BinarySearch(chartData.boxes[boxID].lines[lineID].onlineNotes, m => m.hitTime < note.hitTime, false);
        if (noteEdit.noteType != NoteType.Point) chartData.boxes[boxID].lines[lineID].onlineNotes.Insert(index_noteChartData, note);
        else
            chartData.boxes[boxID].lines[4].onlineNotes.Insert(Algorithm.BinarySearch(chartData.boxes[boxID].lines[4].onlineNotes, m => m.hitTime < note.hitTime, false), note);
        //else
        //{
        //    int index_pointEdits = Algorithm.BinarySearch(chartEditData.boxes[boxID].lines[4].onlineNotes, m => m.hitBeats.ThisStartBPM < noteEdit.hitBeats.ThisStartBPM, false);
        //    chartData.boxes[boxID].lines[4].onlineNotes.Insert(index_pointEdits, note);
        //}
    }
    public static void RefreshChartEventByChartEditEvent(List<Data.ChartData.Event> chartDataEvent, Data.ChartEdit.Event chartEditDataEvent)
    {
        int index_noteEdits = Algorithm.BinarySearch(chartDataEvent, m => m.startTime < BPMManager.Instance.GetSecondsTimeWithBeats(chartEditDataEvent.startBeats.ThisStartBPM), false);
        Data.ChartData.Event @event = new(chartEditDataEvent);
        chartDataEvent.Insert(index_noteEdits, @event);
    }
    /// <summary>
    /// 创建一个新的Edit谱面
    /// </summary>
    /// <param name="chartEditData"></param>
    /// <param name="easeData"></param>
    public static void CreateNewChart(Data.ChartEdit.ChartData chartEditData, List<EaseData> easeData)
    {
        chartEditData.boxes = new();
        Data.ChartEdit.Box chartEditBox = CreateNewBox(easeData);
        chartEditData.boxes.Add(chartEditBox);
    }
    /// <summary>
    /// 创建一个新框
    /// </summary>
    /// <param name="easeData"></param>
    /// <returns></returns>
    public static Data.ChartEdit.Box CreateNewBox(List<EaseData> easeData)
    {
        Data.ChartEdit.Box chartEditBox = new()
        {
            lines = new() { new(), new(), new(), new(), new() },
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
        chartEditBox.boxEvents.scaleX.Add(new() { startBeats = BPM.Zero, endBeats = new(1,0,1), startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
        chartEditBox.boxEvents.scaleY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 2.7f, endValue = 2.7f, curve = easeData[0] });
        chartEditBox.boxEvents.moveX.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0, curve = easeData[0] });
        chartEditBox.boxEvents.moveY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0, curve = easeData[0] });
        chartEditBox.boxEvents.centerX.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = .5f, endValue = .5f, curve = easeData[0] });
        chartEditBox.boxEvents.centerY.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = .5f, endValue = .5f, curve = easeData[0] });
        chartEditBox.boxEvents.alpha.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 1, endValue = 1, curve = easeData[0] });
        chartEditBox.boxEvents.lineAlpha.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0, curve = easeData[0] });
        chartEditBox.boxEvents.rotate.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 0, endValue = 0, curve = easeData[0] });
        chartEditBox.boxEvents.speed.Add(new() { startBeats = BPM.Zero, endBeats = new(1, 0, 1), startValue = 3, endValue = 3, curve = easeData[0] });
        for (int i = 0; i < chartEditBox.lines.Count; i++)
        {
            chartEditBox.lines[i].offlineNotes = new();
            chartEditBox.lines[i].onlineNotes = new();
        }

        return chartEditBox;
    }
    /// <summary>
    /// 转换EditBox到谱面预览器的Box
    /// </summary>
    /// <param name="boxes"></param>
    /// <returns></returns>
    public static List<Data.ChartData.Box> ConvertChartEdit2ChartData(List<Data.ChartEdit.Box> boxes)
    {
        List<Data.ChartData.Box> result = new();
        for (var index = 0; index < boxes.Count; index++)
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
            ConvertEditLine2ChartDataLine(box, chartDataBox, i);
            List<Data.ChartEdit.Event> filledVoid = GameUtility.FillVoid(box.boxEvents.speed);
            chartDataBox.lines[i].speed = new();
            ForeachBoxEvents(filledVoid, chartDataBox.lines[i].speed);
            chartDataBox.lines[i].career = new()
            {
                postWrapMode = WrapMode.ClampForever,
                preWrapMode = WrapMode.ClampForever,
                keys = GameUtility.CalculatedSpeedCurve(chartDataBox.lines[i].speed.ToArray()).ToArray()
            };
            chartDataBox.lines[i].far = new()
            {
                postWrapMode = WrapMode.ClampForever,
                preWrapMode = WrapMode.ClampForever,
                keys = GameUtility.CalculatedFarCurveByChartEditSpeed(filledVoid).ToArray()
            };
        }

        return chartDataBox;
    }

    public static void ConvertEditLine2ChartDataLine(Data.ChartEdit.Box box, Data.ChartData.Box chartDataBox, int i)
    {
        foreach (Data.ChartEdit.Note item in box.lines[i].offlineNotes)
        {
            Data.ChartData.Note newChartDataNote = new(item);
            if (newChartDataNote.noteType == NoteType.Point) chartDataBox.lines[4].offlineNotes.Add(newChartDataNote);
            else chartDataBox.lines[i].offlineNotes.Add(newChartDataNote);
        }
        foreach (Data.ChartEdit.Note item in box.lines[i].onlineNotes)
        {
            Data.ChartData.Note newChartDataNote = new(item);
            if(newChartDataNote.noteType==NoteType.Point) chartDataBox.lines[4].onlineNotes.Add(newChartDataNote);
            else chartDataBox.lines[i].onlineNotes.Add(newChartDataNote);
        }
    }

    public static void ForeachBoxEvents(List<Data.ChartEdit.Event> editBoxEvent, List<Data.ChartData.Event> chartDataBoxEvent)
    {
        chartDataBoxEvent.Clear();
        foreach (Data.ChartEdit.Event item in editBoxEvent)
        {
            chartDataBoxEvent.Add(new(item));
        }
    }
}
