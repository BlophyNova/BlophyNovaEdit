using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using Log;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
namespace Form.NoteEdit
{
    //������û��༭������Ӧ��ص�����
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
                    _ => throw new Exception("�����أ���ô�Ҳ��������Ƿ�ɫ��FullFlick������ɫ��FullFlick��...")
                };
                note.effect = 0;
                note.isClockwise = note.positionX switch
                {
                    <= 0 => true,
                    > 0 => false,
                    _ => throw new Exception("�����أ���ô�Ҳ���������˳ʱ�뻹����ʱ����...")
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
                _ => throw new Exception("��ô�����ء������з�ͨ��note���������ͨ������note��ͨ��")
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
                //�ڶ���
                isFirstTime = false;
                waitForPressureAgain = true;
            } /*����*/
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
            Debug.Log("��������");
            isCopy = true;
            AddNote2NoteClipboard();
        }

        private void CutNote()
        {
            Debug.Log("��������");
            isCopy = false;
            AddNote2NoteClipboard();
        }

        private void PasteNote()
        {
            Debug.Log("ճ������");
            FindNearBeatLineAndVerticalLine(out BeatLine beatLine, out RectTransform verticalLine);
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            if (noteClipboard.Count > 0)
            {
                newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
                AlignNotes(newNotes, beatLine.thisBPM);
                deletedNotes = DeleteNotes(noteClipboard, currentBoxID, currentLineID);
            }
            else
            {
                newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
                AlignNotes(newNotes, beatLine.thisBPM);
                deletedNotes = DeleteNotes(noteClipboard, lastBoxID, lastLineID);
            }
            RefreshAll();
        }
        private void DeleteNoteWithUI()
        {
            List<Note> deletedEvents = DeleteNotes(noteClipboard, currentBoxID,currentLineID);
            RefreshAll();
        }
        private void MoveUp()
        {
            List<Note> newNotes= null;
            List<Note> deletedNotes = null;
            newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            bpm.AddOneBeat();
            AlignNotes(newNotes, bpm);
            deletedNotes = DeleteNotes(noteClipboard, currentBoxID, currentLineID);

            RefreshAll();
        }

        private void MoveDown()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            newNotes = CopyNotes(noteClipboard, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            bpm.SubtractionOneBeat();
            AlignNotes(newNotes, bpm);
            deletedNotes = DeleteNotes(noteClipboard, currentBoxID, currentLineID);

            RefreshAll();
        }

        private void MoveLeft()
        {
            foreach (Note note in noteClipboard)
            {
                float positionX = note.positionX;
                positionX -= verticalLineDeltaDataForChartData;
                positionX = positionX < -1 ? -1 : positionX;
                note.positionX = positionX;
            }
            RefreshAll();
        }

        private void MoveRight()
        {
            foreach (Note note in noteClipboard)
            {
                float positionX = note.positionX;
                positionX += verticalLineDeltaDataForChartData;
                positionX = positionX > 1 ? 1 : positionX;
                note.positionX = positionX;
            }
            RefreshAll();
        }

        private void MirrorNote()
        {
            Debug.Log("��������");
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
    }
}