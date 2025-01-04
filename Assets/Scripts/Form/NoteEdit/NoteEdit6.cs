using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data.ChartEdit;
using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.ChartTool;
using Data.ChartData;
using Form.LabelWindow;
using Form.PropertyEdit;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine.InputSystem;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;

namespace Form.NoteEdit
{
    //这里放处理数据的方法
    public partial class NoteEdit
    {
        private void InstNewNotes(BeatLine beatLine, List<Scenes.Edit.NoteEdit> noteClipboard)
        {
            BPM firstNoteBPM = noteClipboard[0].thisNoteData.HitBeats;
            foreach (Scenes.Edit.NoteEdit note in noteClipboard)
            {
                Note copyNewNote = new(note.thisNoteData);
                copyNewNote.HitBeats = new BPM(beatLine.thisBPM) +
                                       (new BPM(note.thisNoteData.HitBeats) - new BPM(firstNoteBPM));
                if (isCopy)
                {
                    copyNewNote.isSelected = false;
                }

                AddNoteAndRefresh(copyNewNote, currentBoxID, currentLineID);
            }
        }
        private void AddNote2NoteClipboard()
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteClipboard.Add(selectedNote);
            }

            LogCenter.Log($@"已将{noteClipboard.Count}个音符发送至剪切板！");
        }

        private void DeleteNote(Scenes.Edit.NoteEdit note)
        {
            List<Note> notes = GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
            //events.Remove(notePropertyEdit.@event.@event);
            notes.Remove(note.thisNoteData);
            onNoteDeleted(note);
            onBoxRefreshed(currentBoxID);
        }
        public void UpdateNoteLocalPosition()
        {
            for (int i = 0; i < notes.Count; i++)
            {
                notes[i].transform.localPosition = new Vector3(
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) *
                    notes[i].thisNoteData.positionX,
                    YScale.Instance.GetPositionYWithBeats(notes[i].thisNoteData.HitBeats.ThisStartBPM));
            }
        }
    }
}
