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

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        public delegate void OnNoteDeleted(Scenes.Edit.NoteEdit noteEdit);

        public delegate void OnNoteRefreshed(List<Scenes.Edit.NoteEdit> notes);

        public List<Scenes.Edit.NoteEdit> otherLineNoteClipboard = new();
        public List<Scenes.Edit.NoteEdit> noteClipboard = new();
        public bool isCopy;
        public event OnNoteDeleted onNoteDeleted = noteEdit => { };
        public event OnNoteRefreshed onNoteRefreshed = notes => { };

        private void Start2()
        {
            onNoteRefreshed += NoteEdit_onNoteRefreshed;
        }

        private void NoteEdit_onNoteRefreshed(List<Scenes.Edit.NoteEdit> notes)
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                if (item.thisNoteData.isSelected)
                {
                    noteClipboard.Add(item);
                }
            }
        }

        private void SelectBoxDown()
        {
            selectBox.isPressing = true;
            selectBox.transform.SetAsLastSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        private void SelectBoxUp()
        {
            selectBox.isPressing = false;
            selectBox.transform.SetAsFirstSibling();
            Debug.Log($@"selectBox.isPressing={selectBox.isPressing}");
        }

        private void UndoNote()
        {
        }

        private void RedoNote()
        {
        }

        private void CopyNote()
        {
            Debug.Log("复制音符");
            isCopy = true;
            AddNote2NoteClipboard();
        }

        private void PasteNote()
        {
            Debug.Log("粘贴音符");
            FindNearBeatLineAndVerticalLine(out BeatLine beatLine, out RectTransform verticalLine);
            if (noteClipboard.Count > 0)
            {
                InstNewNotes(beatLine, noteClipboard);
            }
            else
            {
                InstNewNotes(beatLine, otherLineNoteClipboard);
            }

            if (!isCopy)
            {
                foreach (Scenes.Edit.NoteEdit note in noteClipboard)
                {
                    DeleteNote(note);
                }
            }

            LogCenter.Log($"成功{isCopy switch { true => "复制", false => "粘贴" }}{noteClipboard.Count}个音符");

            RefreshNoteEditAndChartPreview();
        }

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

        private void CutNote()
        {
            Debug.Log("剪切音符");
            isCopy = false;
            AddNote2NoteClipboard();
        }

        private void AddNote2NoteClipboard()
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit selectedNote in selectBox.TransmitObjects())
            {
                noteClipboard.Add(selectedNote);
            }

            LogCenter.Log($@"已将{noteClipboard.Count}个音符发送至剪切板！");
        }

        private void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<ISelectBoxItem> selectedBoxItems = selectBox.TransmitObjects();
            if (selectedBoxItems.Count <= 0)
            {
                return;
            }

            foreach (Scenes.Edit.NoteEdit selectedBoxItem in selectedBoxItems)
            {
                Note noteData = selectedBoxItem.thisNoteData;
                if (noteData.positionX == 0)
                {
                    continue;
                }

                Note newNote = new(noteData);
                newNote.positionX = -newNote.positionX;
                if (isCopy)
                {
                    newNote.isSelected = false;
                }

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

        private void DeleteNote(Scenes.Edit.NoteEdit note)
        {
            List<Note> notes = GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
            //events.Remove(notePropertyEdit.@event.@event);
            notes.Remove(note.thisNoteData);
            onNoteDeleted(note);
            onBoxRefreshed(currentBoxID);
        }

        private void MoveUp()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteEdit.thisNoteData.HitBeats.AddOneBeat();
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向上移动");
        }

        private void MoveDown()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                noteEdit.thisNoteData.HitBeats.SubtractionOneBeat();
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向下移动");
        }

        private void MoveLeft()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                float positionX = noteEdit.thisNoteData.positionX;
                positionX -= verticalLineDelta;
                positionX = positionX < -1 ? -1 : positionX;
                noteEdit.thisNoteData.positionX = positionX;
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向左移动");
        }

        private void MoveRight()
        {
            foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>())
            {
                float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                float positionX = noteEdit.thisNoteData.positionX;
                positionX += verticalLineDelta;
                positionX = positionX > 1 ? 1 : positionX;
                noteEdit.thisNoteData.positionX = positionX;
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向右移动");
        }

        private void RefreshNoteEditAndChartPreview()
        {
            //ChartTool.ConvertEditLine2ChartDataLine(GlobalData.Instance.chartEditData.boxes[currentBoxID],
            //    GlobalData.Instance.chartData.boxes[currentBoxID], currentLineID);
            GlobalData.Instance.chartData.boxes[currentBoxID] =
                ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID]);
            onBoxRefreshed(currentBoxID);
            //ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID])
            RefreshNotes(-1, -1);
            SpeckleManager.Instance.allLineNoteControllers.Clear();
            GameController.Instance.RefreshChartPreview();
            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI());
        }
    }
}