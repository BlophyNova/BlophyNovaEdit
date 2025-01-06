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
using static UnityEngine.Camera;
namespace Form.NoteEdit
{
    //这里放用户编辑操作响应相关的事情
    public partial class NoteEdit
    {
        public void AddNewTap()
        {
            AddNewNote(NoteType.Tap, NoteEffect.CommonEffect, currentBoxID, currentLineID);
        }

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
            Scenes.Edit.NoteEdit instNewNoteEditPrefab = note.noteType switch
            {
                NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                NoteType.Point => GlobalData.Instance.pointEditPrefab,
                _ => throw new Exception("怎么回事呢···有非通用note代码进入了通用生成note的通道")
            };
            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas).Init(note);
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition =
                new Vector3(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
            //Debug.LogError("写到这里了，下次继续写");
            notes.Add(newNoteEdit);


            AddNoteAndRefresh(note, boxID, lineID);
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
            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
            Note note = new();

            note.positionX =
                (nearVerticalLine.localPosition.x +
                 (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) /
                (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
            note.noteType = note.positionX switch
            {
                <= 0 => NoteType.FullFlickPink,
                > 0 => NoteType.FullFlickBlue,
                _ => throw new Exception("呜呜呜，怎么找不到究竟是粉色的FullFlick还是蓝色的FullFlick呢...")
            };
            note.HitBeats = nearBeatLine.thisBPM;
            note.holdBeats = BPM.One;
            note.effect = 0;
            note.isClockwise = note.positionX switch
            {
                <= 0 => true,
                > 0 => false,
                _ => throw new Exception("呜呜呜，怎么找不到究竟是顺时针还是逆时针呢...")
            };
            Scenes.Edit.NoteEdit newNoteEdit =
                Instantiate(GlobalData.Instance.fullFlickEditPrefab, basicLine.noteCanvas).Init(note);
            newNoteEdit.labelWindow = labelWindow;
            newNoteEdit.transform.localPosition =
                new Vector3(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
            //Debug.LogError("写到这里了，下次继续写");
            notes.Add(newNoteEdit);

            AddNoteAndRefresh(note, currentBoxID, currentLineID);
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
                foreach (Scenes.Edit.NoteEdit note in otherLineNoteClipboard)
                {
                    List<Note> notes = GlobalData.Instance.chartEditData.boxes[lastBoxID].lines[lastLineID].onlineNotes;
                    notes.Remove(note.thisNoteData);
                    onNoteDeleted(note);
                    onBoxRefreshed(currentBoxID);
                }
            }

            LogCenter.Log($"成功{isCopy switch { true => "复制", false => "粘贴" }}{noteClipboard.Count}个音符");

            RefreshNoteEditAndChartPreview();
        }
        private void CutNote()
        {
            Debug.Log("剪切音符");
            isCopy = false;
            AddNote2NoteClipboard();
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
                //float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                //RectTransform nearVerticalLine = FindNearVerticalLine((Vector2)transform.InverseTransformPoint(PositionConvert.UIPointToScreenPoint(noteEdit.transform.position)) + labelWindow.labelWindowRect.sizeDelta / 2);
                //float poxitionX01 = ((nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x)) / 2) / 2 / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x);
                //float positionX = poxitionX01 * 2 - 1;
                float positionX = noteEdit.thisNoteData.positionX;
                positionX -= verticalLineDeltaDataForChartData;
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
                //float verticalLineDelta = 2 / (float)GlobalData.Instance.chartEditData.eventVerticalSubdivision;
                float positionX = noteEdit.thisNoteData.positionX;
                positionX += verticalLineDeltaDataForChartData;
                positionX = positionX > 1 ? 1 : positionX;
                noteEdit.thisNoteData.positionX = positionX;
            }

            RefreshNoteEditAndChartPreview();
            LogCenter.Log($"成功将{selectBox.TransmitObjects().Count}个音符向右移动");
        }

        private void MirrorNote()
        {
            Debug.Log("镜像音符");
            List<ISelectBoxItem> selectedBoxItems = selectBox.TransmitObjects();
            if (selectedBoxItems.Count <= 0)
            {
                return;
            }

            foreach (Scenes.Edit.NoteEdit selectedBoxItem in selectedBoxItems.Cast<Scenes.Edit.NoteEdit>())
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
    }
}