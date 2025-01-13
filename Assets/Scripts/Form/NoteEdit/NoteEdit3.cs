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

        private void AddNewNote(NoteType noteType, NoteEffect noteEffect, int boxID, int lineID)
        {
            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
            Note note = new();

            note.noteType = noteType;
            note.HitBeats = new BPM(nearBeatLine.thisBPM);
            note.holdBeats = BPM.One;
            note.effect = noteEffect;
            note.positionX =
                (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
            Scenes.Edit.NoteEdit instNewNoteEditPrefab = GetNoteType(noteType);
            FullFlickSpecialHandling(noteType, note);

            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas).Init(note);
            note.chartEditNote = newNoteEdit;
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition = new(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
            notes.Add(newNoteEdit);
            AddNote(note, currentBoxID, currentLineID);
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
            AddNewNote(NoteType.Tap, NoteEffect.CommonEffect, currentBoxID, currentLineID);
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
            AddNewNote(NoteType.FullFlick, 0, currentBoxID, currentLineID);
        }
        public void AddNewDrag()
        {
            AddNewNote(NoteType.Drag, NoteEffect.CommonEffect, currentBoxID, currentLineID);
        }

        public void AddNewFlick()
        {
            AddNewNote(NoteType.Flick, NoteEffect.CommonEffect, currentBoxID, currentLineID);
        }

        public void AddNewPoint()
        {
            AddNewNote(NoteType.Point, NoteEffect.Ripple, currentBoxID, currentLineID);
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

            isCopy = true;
            return;
        }
        private void DeleteNoteWithUI()
        {
            IEnumerable<Scenes.Edit.NoteEdit> selectesBoxes = selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>();
            List<Note> selectedNotes = new();
            foreach (Scenes.Edit.NoteEdit noteEdit in selectesBoxes)
            {
                selectedNotes.Add(noteEdit.thisNoteData);
            }
            List<Note> deletedEvents = DeleteNotes(selectedNotes, currentBoxID,currentLineID);
        }
        private void MoveUp()
        {
            List<Note> newNotes= null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedEvents();
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
        }
        private void MoveDown()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedEvents();
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
        }

        private void MoveLeft()
        {
            //List<Note> newNotes = null;
            //List<Note> deletedNotes = null;
            //List<Note> selectedNotes = GetSelectedEvents();
            //newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            //BPM bpm = new(newNotes[0].HitBeats);
            //BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0].chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            //bpm = new(nearBPM);
            //if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            //{
            //    bpm.AddOneBeat();
            //}
            //AlignNotes(newNotes, bpm);
            //AddNotes(newNotes, currentBoxID, currentLineID);
            //notes.AddRange(AddNotes2UI(newNotes));

            //deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
        }

        private void MoveRight()
        {
            //List<Note> newNotes = null;
            //List<Note> deletedNotes = null;
            //List<Note> selectedNotes = GetSelectedEvents();
            //newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            //BPM bpm = new(newNotes[0].HitBeats);
            //BPM nearBPM = FindNearBeatLine((Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0].chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            //bpm = new(nearBPM);
            //if (bpm.denominator == ChartEditData.beatSubdivision && bpm.ThisStartBPM == nearBPM.ThisStartBPM)
            //{
            //    bpm.AddOneBeat();
            //}
            //AlignNotes(newNotes, bpm);
            //AddNotes(newNotes, currentBoxID, currentLineID);
            //notes.AddRange(AddNotes2UI(newNotes));

            //deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
        }

        private void MirrorNote()
        {
            Debug.Log("镜像音符");
            if (noteClipboard.Count <= 0)
            {
                return;
            }

            for (int i=0;i< noteClipboard.Count;i++)
            {
                Note noteData = noteClipboard[i];
                if (noteData.positionX == 0)
                {
                    continue;
                }

                Note newNote = new(noteData);
                newNote.positionX = -newNote.positionX;
                newNote.isSelected = false;
                AddNote(newNote,currentBoxID,currentLineID);
            }
            RefreshAll();
        }
        void MirrorFlip()
        {
            if (noteClipboard.Count <= 0)
            {
                return;
            }
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                Note noteData = noteClipboard[i];
                noteData.positionX = -noteData.positionX;
            }
            RefreshAll();
        }
        private List<Note> GetSelectedEvents()
        {
            IEnumerable<Scenes.Edit.NoteEdit> selectedNoteEdits = selectBox.TransmitObjects().Cast<Scenes.Edit.NoteEdit>();
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

            return newNoteEdit;
        }

        private void DestroyNotes()
        {
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                Destroy(item.gameObject);
            }
            notes.Clear();
        }
    }
}