using CustomSystem;
using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using Form.PropertyEdit;
using Log;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
namespace Form.NoteEdit
{
    //这里放用户编辑操作响应相关的事情
    public partial class NoteEdit
    {

        private Note AddNewNote(NoteType noteType, NoteEffect noteEffect, int boxID, int lineID)
        {
            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
            Note note = new();

            note.noteType = noteType;
            note.HitBeats = new BPM(nearBeatLine.thisBPM);
            note.holdBeats = BPM.One;
            note.effect = noteEffect;
            note.positionX= CalculatePositionX(nearVerticalLine);
            Scenes.Edit.NoteEdit instNewNoteEditPrefab = GetNoteType(noteType);
            FullFlickSpecialHandling(noteType, note);

            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas).Init(note);
            note.chartEditNote = newNoteEdit;
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition = new(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
            notes.Add(newNoteEdit);
            AddNote(note, boxID, lineID);
            return note;
        }

        private float CalculatePositionX(RectTransform nearVerticalLine)
        {
                return(nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
        }

        private static void FullFlickSpecialHandling(NoteType noteType, Note note)
        {
            if (noteType == NoteType.FullFlick)
            {
                note.noteType = note.positionX switch
                {
                    <= 0 => NoteType.FullFlickPink,
                    > 0 => NoteType.FullFlickBlue,
                    _ => throw new Exception("呜呜呜，怎么找不到究竟是粉色的FullFlick还是蓝色的FullFlick呢...")
                };
                note.effect = 0;
                note.isClockwise = note.positionX switch
                {
                    <= 0 => true,
                    > 0 => false,
                    _ => throw new Exception("呜呜呜，怎么找不到究竟是顺时针还是逆时针呢...")
                };
            }
        }

        private static Scenes.Edit.NoteEdit GetNoteType(NoteType noteType)
        {
            return noteType switch
            {
                NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                NoteType.Point => GlobalData.Instance.pointEditPrefab,
                NoteType.FullFlick => GlobalData.Instance.fullFlickEditPrefab,
                _ => throw new Exception("怎么回事呢···有非通用note代码进入了通用生成note的通道")
            };
        }

        public void AddNewTap()
        {
            Note newNote = AddNewNote(NoteType.Tap, NoteEffect.CommonEffect, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new() { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }
        public void AddNewHold()
        {
            Debug.Log($"{MousePositionInThisRectTransform}");
            if (!isFirstTime)
            {
                isFirstTime = true;

                FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
                Note note = new();
                note.noteType = NoteType.Hold;
                note.HitBeats = new BPM(nearBeatLine.thisBPM);
                note.effect = NoteEffect.CommonEffect;
                note.positionX =
                    (nearVerticalLine.localPosition.x +
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) /
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
                Scenes.Edit.NoteEdit newHoldEdit =
                    Instantiate(GlobalData.Instance.holdEditPrefab, basicLine.noteCanvas).Init(note);
                note.chartEditNote = newHoldEdit;
                newHoldEdit.labelWindow = labelWindow;
                newHoldEdit.transform.localPosition = new Vector2(nearVerticalLine.transform.localPosition.x,
                    nearBeatLine.transform.localPosition.y);
                StartCoroutine(WaitForPressureAgain(newHoldEdit, currentBoxID, currentLineID));
            }
            else if (isFirstTime)
            {
                //第二次
                isFirstTime = false;
                waitForPressureAgain = true;
            } /*报错*/
        }
        public void AddNewFullFlick()
        {
            Note newNote =AddNewNote(NoteType.FullFlick, 0, currentBoxID, currentLineID); 
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new() { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }
        public void AddNewDrag()
        {
            Note newNote = AddNewNote(NoteType.Drag, NoteEffect.CommonEffect, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new() { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }

        public void AddNewFlick()
        {
            Note newNote = AddNewNote(NoteType.Flick, NoteEffect.CommonEffect, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new() { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }

        public void AddNewPoint()
        {
            Note newNote = AddNewNote(NoteType.Point, NoteEffect.Ripple, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new() { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
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

        private void CutNote()
        {
            Debug.Log("剪切音符");
            isCopy = false;
            AddNote2NoteClipboard();
        }

        private void PasteNote()
        {
            Debug.Log("粘贴音符");
            FindNearBeatLineAndVerticalLine(out BeatLine beatLine, out RectTransform verticalLine);
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            if (noteClipboard.Count > 0)
            {               
                newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
                AlignNotes(newNotes, beatLine.thisBPM);
                deletedNotes = DeleteNotes(noteClipboard, currentBoxID, currentLineID,isCopy);
                BatchNotes(newNotes, note => note.isSelected = false);
                AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(newNotes));
            }
            else
            {
                newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
                AlignNotes(newNotes, beatLine.thisBPM);
                deletedNotes = DeleteNotes(noteClipboard, lastBoxID, lastLineID, isCopy);
                BatchNotes(newNotes, note => note.isSelected = false);
                AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(newNotes));
            }

            //这里还有很多关于撤销重做的事情
            if (isCopy)
            {
                Steps.Instance.Add(CopyUndo, CopyRedo, default);
            }
            else
            {
                Steps.Instance.Add(CutUndo, CutRedo, default);
            }
            isCopy = true;
            return;
            void CopyUndo()
            {
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void CopyRedo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
            void CutUndo()
            {
                List<Note> instNewNotes=AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID,false);
            }
            void CutRedo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes,currentBoxID, currentLineID,false);
            }
        }
        private void DeleteNoteWithUI()
        {
            List<Note> selectedNotes = GetSelectedNotes();
            List<Note> deletedEvents = DeleteNotes(selectedNotes, currentBoxID,currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedEvents, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected=false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
            void Redo()
            {
                DeleteNotes(deletedEvents, currentBoxID, currentLineID);
            }
        }
        private void MoveUp()
        {
            List<Note> newNotes= null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0].chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new(nearBPM);
            if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            {
                bpm.AddOneBeat();
            }
            AlignNotes(newNotes, bpm);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID, false);
            }
        }
        private void MoveDown()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0].chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new(nearBPM);
            if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            {
                bpm.SubtractionOneBeat();
            }
            AlignNotes(newNotes, bpm);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID); 
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID, false);
            }
        }

        private void MoveLeft()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            foreach (Note note in newNotes)
            {
                if (note.positionX - verticalLineDeltaDataForChartData < -1.0001f) return;
            }
            BatchNotes(newNotes, note => note.positionX -=verticalLineDeltaDataForChartData);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID, false);
            }
        }

        private void MoveRight()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            foreach (Note note in newNotes)
            {
                if (note.positionX + verticalLineDeltaDataForChartData > 1.0001f) return;
            }
            BatchNotes(newNotes, note => note.positionX+=verticalLineDeltaDataForChartData);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID, false);
            }
        }

        private void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<Note> newNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();

            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BatchNotes(newNotes, note => { note.isSelected = false;note.positionX = -note.positionX; });
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));
            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }
        void MirrorFlip()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID, false);
            BatchNotes(newNotes, note => note.positionX=-note.positionX);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            Steps.Instance.Add(Undo, Redo, default);
            return;
            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID, false);
            }
            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID, false);
            }
        }


    }
}