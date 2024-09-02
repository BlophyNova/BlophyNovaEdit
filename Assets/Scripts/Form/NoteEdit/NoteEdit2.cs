using System.Collections;
using System.Collections.Generic;
using Data.ChartEdit;
using UnityEngine;

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {

        public delegate void OnNoteDeleted(Scenes.Edit.NoteEdit noteEdit);
        public event OnNoteDeleted onNoteDeleted = noteEdit => { };
        public delegate void OnNoteRefreshed(List<Scenes.Edit.NoteEdit>  notes);
        public event OnNoteRefreshed onNoteRefreshed = notes => { };

        public List<Note> noteClipboard=new();
        public bool isCopy = false;
        void SelectBoxDown()
        {
            selectBox.isPressing = true;
            selectBox.transform.SetAsLastSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }
        void SelectBoxUp()
        {
            selectBox.isPressing = false;
            selectBox.transform.SetAsFirstSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        void UndoNote()
        {

        }

        void RedoNote()
        {

        }

        void CopyNote()
        {
            Debug.Log("复制音符");
            isCopy=true;
            AddNote2NoteClipboard();
        }

        void AddNote2NoteClipboard()
        {
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.selectedBoxItems)
            {
                noteClipboard.Add(selectedNote.thisNoteData);
            }
        }

        void PasteNote()
        {
            Debug.Log("粘贴音符");

        }

        void CutNote()
        {
            Debug.Log("剪切音符");
            isCopy=false; 
            AddNote2NoteClipboard();
        }

        void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<ISelectBoxItem> selectedBoxItems = selectBox.TransmitObjects();
            if (selectedBoxItems.Count<=0) return;
            foreach (Scenes.Edit.NoteEdit selectedBoxItem in selectedBoxItems)
            {
                var noteData = selectedBoxItem.thisNoteData;
                if(noteData.positionX==0)continue;
                Note newNote = new(noteData);
                newNote.positionX = -newNote.positionX; 
                AddNoteAndRefresh(newNote, currentBoxID, currentLineID);
            }
            RefreshNotes(-1, -1);
        }
    }
}