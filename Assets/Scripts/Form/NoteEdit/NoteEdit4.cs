using Controller;
using Data.ChartData;
using Data.Interface;
using Form.PropertyEdit;
using Log;
using Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilityCode.ChartTool;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
using static UtilityCode.ChartTool.ChartTool;

namespace Form.NoteEdit
{
    //这里放所有的刷新方法
    public partial class NoteEdit
    {
        public void RefreshAll()
        {
            RefreshNotes(currentBoxID, currentLineID);
            RefreshPlayer(currentBoxID, currentLineID);
        }
        public void RefreshNotes(int boxID,int lineID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            lastLineID = boxID < 0 ? lastLineID : currentLineID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            DestroyNotes();
            List<Scenes.Edit.NoteEdit> newNotes =AddNotes2UI(ChartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes);
            notes.AddRange(newNotes);
            List<Note> refreshedNotes = new();
            foreach (Scenes.Edit.NoteEdit note in notes)
            {
                refreshedNotes.Add(note.thisNoteData);
            }
            onNotesRefreshed(refreshedNotes);
        }

        

        private Scenes.Edit.NoteEdit GetNoteType(Note item)
        {
            return item.noteType switch
            {
                NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                NoteType.Hold => GlobalData.Instance.holdEditPrefab,
                NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                NoteType.Point => GlobalData.Instance.pointEditPrefab,
                NoteType.FullFlickPink => GlobalData.Instance.fullFlickEditPrefab,
                NoteType.FullFlickBlue => GlobalData.Instance.fullFlickEditPrefab,
                _ => throw new Exception("滴滴~滴滴~错误~找不到音符拉~")
            };
        }

        public void RefreshPlayer(int boxID,int lineID)
        {
            ConvertLine(ChartEditData.boxes[boxID].lines[lineID].onlineNotes, ChartData.boxes[boxID].lines[lineID].onlineNotes);
        }

    }
}
