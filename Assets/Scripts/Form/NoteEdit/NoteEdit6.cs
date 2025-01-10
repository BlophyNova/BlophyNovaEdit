using Data.ChartEdit;
using Form.PropertyEdit;
using Log;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
using static UtilityCode.ChartTool.ChartTool;
using EventType = Data.Enumerate.EventType;
using UtilityCode.Algorithm;
namespace Form.NoteEdit
{
    //这里放处理数据的方法,不负责刷新以及为ChartData加入数据
    public partial class NoteEdit
    {
        ChartData ChartEditData => GlobalData.Instance.chartEditData;
        Data.ChartData.ChartData ChartData => GlobalData.Instance.chartData;

        void AddNote(Note note, int boxID,int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            int index = Algorithm.BinarySearch(notes, m => m.HitBeats.ThisStartBPM < note.HitBeats.ThisStartBPM, false);
            notes.Insert(index, note);
        }
        void DeleteNote(Note note, int boxID, int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            notes.Remove(note);
        }
        int FindNoteIndex(Note note, int boxID, int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            int findIndex = notes.FindIndex(m => m == note);
            return findIndex;
        }
        private void AddNote2NoteClipboard()
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteClipboard.Add(selectedNote.thisNoteData);
            }

            LogCenter.Log($@"已将{noteClipboard.Count}个音符发送至剪切板！");
        }
        private List<Note> CopyNotes(List<Note> noteClipboard, int boxID,int lineID)
        {
            List<Note> newNotes = null;
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note note = new(noteClipboard[i]);
                AddNote(note, boxID, lineID);
                newNotes.Add(note);
            }
            return newNotes;
        }
        private List<Note> AddNotes(List<Note> noteClipboard, int boxID, int lineID)
        {
            List<Note> newNotes = null;
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note note = noteClipboard[i];
                AddNote(note, boxID, lineID);
                newNotes.Add(note);
            }
            return newNotes;
        }
        private List<Note> DeleteNotes(List<Note> noteClipboard, int boxID,int lineID, bool isCopy = false)
        {
            List<Note> deletedNotes = new();
            if (isCopy) return deletedNotes;
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                DeleteNote(noteClipboard[i], boxID,lineID);
                deletedNotes.Add(noteClipboard[i]);
            }
            return deletedNotes;
        }
        private void AlignNotes(List<Note> noteClipboard, BPM bpm)
        {
            BPM firstNoteStartBeats = noteClipboard[0].HitBeats;
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note note = noteClipboard[i];
                noteClipboard[i].HitBeats = new BPM(bpm) + (new BPM(note.HitBeats) - new BPM(firstNoteStartBeats));
            }
        }
    }
}
