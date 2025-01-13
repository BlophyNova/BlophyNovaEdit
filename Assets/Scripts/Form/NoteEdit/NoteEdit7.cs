using Data.ChartEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UtilityCode.ChartTool.ChartTool;
using UtilityCode.Algorithm;
using Manager;
namespace Form.NoteEdit
{
    //这里放为ChartData加入数据的方法,不负责刷新
    public partial class NoteEdit
    {
        public void AddNote2ChartData(Note note, int boxID,int lineID)
        {
            List<Data.ChartData.Note> notes= ChartData.boxes[boxID].lines[lineID].onlineNotes;
            int index = Algorithm.BinarySearch(notes, m => m.hitTime <  BPMManager.Instance.GetSecondsTimeByBeats(note.HitBeats.ThisStartBPM), false);
            Data.ChartData.Note newNote = new(note);

            newNote.hitFloorPosition =(float)Math.Round(ChartData.boxes[boxID].lines[lineID].far.Evaluate(newNote.hitTime),3);
            note.chartDataNote= newNote;
            notes.Insert(index, note.chartDataNote);
        }
        public void DeleteNote2ChartData(Note note, int boxID, int lineID)
        {
            List<Data.ChartData.Note> notes = ChartData.boxes[boxID].lines[lineID].onlineNotes;
            notes.Remove(note.chartDataNote);
        }

    }
}