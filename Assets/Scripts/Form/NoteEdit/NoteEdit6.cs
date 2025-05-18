using System;
using System.Collections.Generic;
using System.Linq;
using Data.ChartData;
using Data.ChartEdit;
using Form.PropertyEdit;
using Manager;
using Newtonsoft.Json;
using UnityEngine;
using UtilityCode.Algorithm;
using ChartData = Data.ChartEdit.ChartData;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;

namespace Form.NoteEdit
{
    //这里放处理数据的方法,不负责刷新
    public partial class NoteEdit
    {
        private ChartData ChartEditData => GlobalData.Instance.chartEditData;
        private Data.ChartData.ChartData ChartData => GlobalData.Instance.chartData;

        private void AddNote(Note note, int boxID, int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            int index = Algorithm.BinarySearch(notes, m => m.HitBeats.ThisStartBPM < note.HitBeats.ThisStartBPM, false);
            notes.Insert(index, note);
            AddNote2ChartData(note, boxID, lineID);
        }

        private void DeleteNote(Note note, int boxID, int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            notes.Remove(note);
            if (this.notes.Count > 0)
            {
                this.notes.Remove(note.chartEditNote);
            }

            if (note.chartEditNote != null)
            {
                selectBox.selectedBoxItems.Remove(note.chartEditNote);
                Destroy(note.chartEditNote.gameObject);
            }

            DeleteNote2ChartData(note, boxID, lineID);
        }

        private int FindNoteIndex(Note note, int boxID, int lineID)
        {
            List<Note> notes = ChartEditData.boxes[boxID].lines[lineID].onlineNotes;
            int findIndex = notes.FindIndex(m => m == note);
            return findIndex;
        }

        private void AddNote2NoteClipboard()
        {
            List<Note> notes = new();
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                notes.Add(selectedNote.thisNoteData);
            }

            GUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(notes);
        }

        private List<Note> CopyNotes(List<Note> noteClipboard, int boxID, int lineID)
        {
            List<Note> newNotes = new();
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note note = new(noteClipboard[i]);
                //AddNote(note, boxID, lineID);
                newNotes.Add(note);
            }

            return newNotes;
        }

        private List<Note> AddNotes(List<Note> noteClipboard, int boxID, int lineID)
        {
            List<Note> newNotes = new();
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note note = noteClipboard[i];
                AddNote(note, boxID, lineID);
                newNotes.Add(note);
            }

            onNotesAdded(newNotes);
            return newNotes;
        }

        private List<Note> DeleteNotes(List<Note> noteClipboard, int boxID, int lineID, bool isCopy = false)
        {
            List<Note> deletedNotes = new();
            if (isCopy)
            {
                return deletedNotes;
            }

            for (int i = 0; i < noteClipboard.Count; i++)
            {
                DeleteNote(noteClipboard[i], boxID, lineID);
                deletedNotes.Add(noteClipboard[i]);
            }

            onNotesDeleted(deletedNotes);
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

        private void BatchNotes(List<Note> notes, Action<Note> action)
        {
            for (int i = 0; i < notes.Count; i++)
            {
                action(notes[i]);
            }
        }

        private List<Note> GetSelectedNotes()
        {
            IEnumerable<Scenes.Edit.NoteEdit> selectedNoteEdits =
                selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>();
            List<Note> selectedNotes = new();
            foreach (Scenes.Edit.NoteEdit item in selectedNoteEdits)
            {
                selectedNotes.Add(item.thisNoteData);
            }

            return selectedNotes;
        }

        private List<Scenes.Edit.NoteEdit> AddNotes2UI(List<Note> needInstNotes)
        {
            List<Scenes.Edit.NoteEdit> notes = new();
            foreach (Note item in needInstNotes)
            {
                Scenes.Edit.NoteEdit newNoteEdit = AddNote2UI(item);

                //Debug.LogError("写到这里了，下次继续写");
                notes.Add(newNoteEdit);
            }

            onNotesAdded2UI(notes);
            return notes;
        }

        private Scenes.Edit.NoteEdit AddNote2UI(Note item)
        {
            Scenes.Edit.NoteEdit noteEditType = GetNoteType(item);

            float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(item.HitBeats.ThisStartBPM);
            float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(noteEditType, basicLine.noteCanvas).Init(item);
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition = new Vector3(
                (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                 (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) * item.positionX,
                positionY);
            item.chartEditNote = newNoteEdit;
            if (item.noteType == NoteType.Hold)
            {
                //float endBeatsSecondsTime = BPMManager.Instance.GetSecondsTimeWithBeats(item.EndBeats.ThisStartBPM);
                //float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(item.EndBeats.ThisStartBPM);
                //float hitBeatsPositionY= YScale.Instance.GetPositionYWithSecondsTime(item.HitBeats.ThisStartBPM);
                float holdBeatsPositionY =
                    YScale.Instance.GetPositionYWithSecondsTime(
                        BPMManager.Instance.GetSecondsTimeByBeats(item.holdBeats.ThisStartBPM));
                newNoteEdit.thisNoteRect.sizeDelta =
                    new Vector2(newNoteEdit.thisNoteRect.sizeDelta.x, holdBeatsPositionY);
            }

            newNoteEdit.SetSelectState(item.isSelected);
            return newNoteEdit;
        }
    }
}