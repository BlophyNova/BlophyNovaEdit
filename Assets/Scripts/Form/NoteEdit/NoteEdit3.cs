using System;
using System.Collections.Generic;
using System.Linq;
using CustomSystem;
using Cysharp.Threading.Tasks;
using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using Form.EventEdit;
using Form.PropertyEdit;
using Manager;
using Newtonsoft.Json;
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
            note.positionX = CalculatePositionX(nearVerticalLine);
            Scenes.Edit.NoteEdit instNewNoteEditPrefab = GetNoteType(noteType);
            FullFlickSpecialHandling(noteType, note);

            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas).Init(note);
            note.chartEditNote = newNoteEdit;
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition =
                new Vector3(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
            notes.Add(newNoteEdit);
            AddNote(note, boxID, lineID);
            return note;
        }

        private float CalculatePositionX(RectTransform nearVerticalLine)
        {
            return (nearVerticalLine.localPosition.x +
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) /
                (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
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
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
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
            Note newNote = AddNewNote(NoteType.FullFlick, 0, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
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
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
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
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
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
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
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
            DeleteNoteWithUI();
        }

        private void PasteNote()
        {
            Debug.Log("粘贴音符");
            FindNearBeatLineAndVerticalLine(out BeatLine beatLine, out RectTransform verticalLine);
            string rawData = GUIUtility.systemCopyBuffer;
            try
            {
                List<Note> newNotes = JsonConvert.DeserializeObject<List<Note>>(rawData);
                Steps.Instance.Add(PasteUndo, PasteRedo, default);
                PasteRedo();
                isCopy = true;
                return;

                void PasteUndo()
                {
                    DeleteNotes(newNotes, currentBoxID, currentLineID);
                }

                void PasteRedo()
                {
                    AlignNotes(newNotes, beatLine.thisBPM);
                    BatchNotes(newNotes, note => note.isSelected = false);
                    List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                    notes.AddRange(AddNotes2UI(instNewNotes));
                }
            }
            catch (JsonException je)
            {
            }
        }

        private void DeleteNoteWithUI()
        {
            List<Note> selectedNotes = GetSelectedNotes();
            List<Note> deletedEvents = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedEvents, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }

            void Redo()
            {
                DeleteNotes(deletedEvents, currentBoxID, currentLineID);
            }
        }

        private void Move(bool isMoving)
        {
            this.isMoving = isMoving;
            MoveAsync(isMoving);
        }

        async void MoveAsync(bool isMoving)
        {
            if (!isMoving|| selectBox.selectedBoxItems.Count==0)
            {
                return;
            }
            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine,out RectTransform nearVerticalLine);
            
            BPM neatBeatLineBpm = new(nearBeatLine.thisBPM);
            while (this.isMoving && FocusIsMe && selectBox.selectedBoxItems.Count != 0)
            {
                await UniTask.NextFrame();//啊哈，没错，引入了UniTask导致的，真方便（
                FindNearBeatLineAndVerticalLine(out nearBeatLine,
                    out nearVerticalLine);
                try
                {
                    neatBeatLineBpm = new(nearBeatLine.thisBPM);
                }
                catch
                {
                    // ignored
                }

                //这，这对吗？还是不要频繁刷新比较好，想想别的方法吧
                
                float firstPositionX=((Scenes.Edit.NoteEdit)selectBox.selectedBoxItems[0]).thisNoteData.positionX;
                float firstLocalPositionX =
                    ((Scenes.Edit.NoteEdit)selectBox.selectedBoxItems[0]).thisNoteRect.localPosition.x;
                BPM firstBpm = ((Scenes.Edit.NoteEdit)selectBox.selectedBoxItems[0]).thisNoteData.HitBeats;

                float minPositionX=float.MaxValue;
                float maxPositionX = float.MinValue;
                foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.selectedBoxItems.Cast<Scenes.Edit.NoteEdit>())
                {
                    if (noteEdit.thisNoteData.positionX>maxPositionX)
                    {
                        maxPositionX = noteEdit.thisNoteData.positionX;
                    }

                    if (noteEdit.thisNoteData.positionX<minPositionX)
                    {
                        minPositionX = noteEdit.thisNoteData.positionX;
                    }
                }
                foreach (Scenes.Edit.NoteEdit noteEdit in selectBox.selectedBoxItems.Cast<Scenes.Edit.NoteEdit>())
                {
                    RectTransform rect=noteEdit.thisNoteRect;
                    
                    
                    float currentPositionX = CalculatePositionX(nearVerticalLine) + (noteEdit.thisNoteData.positionX - firstPositionX);
                    float currentLocalPositionX = nearVerticalLine.localPosition.x + (noteEdit.thisNoteRect.localPosition.x-firstLocalPositionX);
                    
                    BPM newBpm = new BPM(neatBeatLineBpm) + (new BPM(noteEdit.thisNoteData.HitBeats) - new BPM(firstBpm));
                    float currentSecondsTime =
                        BPMManager.Instance.GetSecondsTimeByBeats(newBpm.ThisStartBPM);
                    float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);
                    
                    float currentMaxPositionX=CalculatePositionX(nearVerticalLine) + (maxPositionX - firstPositionX);
                    float currentMinPositionX=CalculatePositionX(nearVerticalLine) + (minPositionX - firstPositionX);
                    
                    if (currentMaxPositionX>1||currentMinPositionX<-1)
                    {
                        rect.localPosition = new(rect.localPosition.x,positionY);
                    }
                    else
                    {
                        noteEdit.thisNoteData.positionX = currentPositionX;
                        rect.localPosition = new(currentLocalPositionX, positionY);
                    }
                }
            }
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            BPM nearBpm = nearBeatLine.thisBPM;
            bpm = new BPM(nearBpm);
            
            AlignNotes(newNotes, bpm);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID,currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
            }
        }

        private void MoveUp()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            BPM nearBPM =
                FindNearBeatLine(
                    (Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0]
                        .chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new BPM(nearBPM);
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
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
            }
        }

        private void MoveDown()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BPM bpm = new(newNotes[0].HitBeats);
            BPM nearBPM =
                FindNearBeatLine(
                    (Vector2)labelWindow.labelWindowContent.transform.InverseTransformPoint(selectedNotes[0]
                        .chartEditNote.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2).thisBPM;
            bpm = new BPM(nearBPM);
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
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
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
                if (note.positionX - verticalLineDeltaDataForChartData < -1.0001f)
                {
                    return;
                }
            }

            BatchNotes(newNotes, note => note.positionX -= verticalLineDeltaDataForChartData);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
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
                if (note.positionX + verticalLineDeltaDataForChartData > 1.0001f)
                {
                    return;
                }
            }

            BatchNotes(newNotes, note => note.positionX += verticalLineDeltaDataForChartData);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
            }
        }

        private void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<Note> newNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();

            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            BatchNotes(newNotes, note =>
            {
                note.isSelected = false;
                note.positionX = -note.positionX;
            });
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));
            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }

        private void MirrorFlip()
        {
            List<Note> newNotes = null;
            List<Note> deletedNotes = null;
            List<Note> selectedNotes = GetSelectedNotes();
            newNotes = CopyNotes(selectedNotes, currentBoxID, currentLineID);
            deletedNotes = DeleteNotes(selectedNotes, currentBoxID, currentLineID);
            BatchNotes(newNotes, note => note.positionX = -note.positionX);
            AddNotes(newNotes, currentBoxID, currentLineID);
            notes.AddRange(AddNotes2UI(newNotes));

            Steps.Instance.Add(Undo, Redo, default);
            return;

            void Undo()
            {
                List<Note> instNewNotes = AddNotes(deletedNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(newNotes, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(newNotes, currentBoxID, currentLineID);
                notes.AddRange(AddNotes2UI(instNewNotes));
                DeleteNotes(deletedNotes, currentBoxID, currentLineID);
            }
        }
    }
}