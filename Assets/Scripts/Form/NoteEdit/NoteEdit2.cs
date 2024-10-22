using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data.ChartEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {

        public delegate void OnNoteDeleted(Scenes.Edit.NoteEdit noteEdit);
        public event OnNoteDeleted onNoteDeleted = noteEdit => { };
        public delegate void OnNoteRefreshed(List<Scenes.Edit.NoteEdit>  notes);
        public event OnNoteRefreshed onNoteRefreshed = notes => { };

        public List<Scenes.Edit.NoteEdit> noteClipboard=new();
        public bool isCopy = false;
        void Start2()
        {
            onNoteRefreshed += NoteEdit_onNoteRefreshed;
        }

        private void NoteEdit_onNoteRefreshed(List<Scenes.Edit.NoteEdit> notes)
        {
            noteClipboard.Clear();
            foreach (var item in notes)
            {
                if (item.thisNoteData.isSelected)
                {
                    noteClipboard.Add(item);
                }
            }
        }

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
        void PasteNote() 
        {
            Debug.Log("粘贴音符");
            FindNearBeatLineAndVerticalLine(out BeatLine beatLine,out var verticalLine );
            BPM firstNoteBPM = noteClipboard[0].thisNoteData.HitBeats;
            foreach (Scenes.Edit.NoteEdit note in noteClipboard)
            {
                Note copyNewNote = new(note.thisNoteData);
                copyNewNote.HitBeats = new BPM(beatLine.thisBPM)+(new BPM(note.thisNoteData.HitBeats)-new BPM(firstNoteBPM));
                if (isCopy) copyNewNote.isSelected = false;
                AddNoteAndRefresh(copyNewNote,currentBoxID,currentLineID);
            }

            if (!isCopy)
            {
                foreach (Scenes.Edit.NoteEdit note in noteClipboard)
                {
                    DeleteNote(note);
                }
            }
            LogCenter.Log($"成功{isCopy switch{true=>"复制",false=>"粘贴"}}{noteClipboard.Count}个音符");
            noteClipboard.Clear();

            RefreshNoteEditAndChartPreview();

        }
        void CutNote()
        {
            Debug.Log("剪切音符");
            isCopy=false;
            AddNote2NoteClipboard();
        }
        void AddNote2NoteClipboard()
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.TransmitObjects())
            {
                noteClipboard.Add(selectedNote);
            }
            LogCenter.Log($@"已将{noteClipboard.Count}个音符发送至剪切板！");
        }
        void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<ISelectBoxItem> selectedBoxItems = selectBox.TransmitObjects();
            if (selectedBoxItems.Count<=0) return;
            foreach (Scenes.Edit.NoteEdit selectedBoxItem in selectedBoxItems)
            {
                Note noteData = selectedBoxItem.thisNoteData;
                if(noteData.positionX==0)continue;
                Note newNote = new(noteData);
                newNote.positionX = -newNote.positionX;
                if (isCopy) newNote.isSelected = false;
                AddNoteAndRefresh(newNote, currentBoxID, currentLineID);
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功镜像{selectedBoxItems.Count}个音符");
        }


        private void DeleteNoteWithUI()
        {
            for (int i = selectBox.TransmitObjects().Count - 1; i >= 0; i--)
            {
                DeleteNote((Scenes.Edit.NoteEdit)selectBox.TransmitObjects()[i]);
            }
            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功删除{selectBox.TransmitObjects().Count}个音符");
        }

        void DeleteNote(Scenes.Edit.NoteEdit note)
        {
            List<Data.ChartEdit.Note> notes = GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
            //events.Remove(notePropertyEdit.@event.@event);
            notes.Remove(note.thisNoteData);
            onNoteDeleted(note);
        }

        void MoveUp()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteEdit.thisNoteData.HitBeats.AddOneBeat();
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向上移动");
        }

        void MoveDown()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteEdit.thisNoteData.HitBeats.SubtractionOneBeat();
            }
            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向下移动");
        }

        void MoveLeft()
        {

            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                noteEdit.thisNoteData.positionX -= verticalLineDelta;
            }
            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向左移动");
        }

        void MoveRight()
        {

            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                noteEdit.thisNoteData.positionX += verticalLineDelta;
            }
            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向右移动");
        }

        private void RefreshNoteEditAndChartPreview()
        {
            //ChartTool.ConvertEditLine2ChartDataLine(GlobalData.Instance.chartEditData.boxes[currentBoxID],
            //    GlobalData.Instance.chartData.boxes[currentBoxID], currentLineID);
            GlobalData.Instance.chartData.boxes[currentBoxID]=ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID]);
            //ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID])
            RefreshNotes(-1, -1);
            SpeckleManager.Instance.allLineNoteControllers.Clear();
            GameController.Instance.RefreshChartPreview();
            GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>((interfaceMethod) => interfaceMethod.RefreshUI());
        }
    }
}