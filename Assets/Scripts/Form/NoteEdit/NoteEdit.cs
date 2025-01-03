using System;
using System.Collections;
using System.Collections.Generic;
using Data.ChartData;
using Data.ChartEdit;
using Data.Interface;
using Form.LabelWindow;
using Form.PropertyEdit;
using Log;
using Manager;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.ChartTool;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;

namespace Form.NoteEdit
{
    public partial class NoteEdit : LabelWindowContent, IInputEventCallback, IRefresh, ISelectBox
    {
        public int currentBoxID;
        public int currentLineID;


        public RectTransform noteEditRect;

        public RectTransform verticalLineLeft;
        public RectTransform verticalLineRight;
        public RectTransform verticalLinePrefab;
        public SelectBox selectBox;

        public BasicLine basicLine;

        public TMP_Text boxAndLineIDText;

        public List<RectTransform> verticalLines = new();

        public List<Scenes.Edit.NoteEdit> notes = new();


        public bool isFirstTime;
        public bool waitForPressureAgain;

        private void Start()
        {
            RefreshNotes(currentBoxID, currentLineID);
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
            Start2();
        }

        private void Update()
        {
            boxAndLineIDText.text = $"框号:{currentBoxID}\n线号:{currentLineID}";
        }

        public override void Started(InputAction.CallbackContext callbackContext)
        {
            Action action = callbackContext.action.name switch
            {
                "SelectBox" => () => SelectBoxDown(),
                _ => () => Debug.Log("欸···？怎么回事，怎么会找不到事件呢···")
            };
            action();
        }

        public override void Performed(InputAction.CallbackContext callbackContext)
        {
            //鼠标按下抬起的时候调用
            //selectBox.isPressing = !selectBox.isPressing;
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            Debug.Log($"{MousePositionInThisRectTransform}||{callbackContext.action.name}");
            Action action = callbackContext.action.name switch
            {
                "AddTap" => AddNewTap,
                "AddHold" => AddNewHold,
                "AddDrag" => AddNewDrag,
                "AddFlick" => AddNewFlick,
                "AddPoint" => AddNewPoint,
                "AddFullFlick" => AddNewFullFlick,
                "Delete" => DeleteNoteWithUI,
                "SelectBox" => SelectBoxUp,
                "Undo" => UndoNote,
                "Redo" => RedoNote,
                "Copy" => CopyNote,
                "Paste" => PasteNote,
                "Cut" => CutNote,
                "Mirror" => MirrorNote,
                "MoveUp" => MoveUp,
                "MoveDown" => MoveDown,
                "MoveLeft" => MoveLeft,
                "MoveRight" => MoveRight,
                _ => () => Alert.EnableAlert("欸···？怎么回事，怎么会找不到事件呢···")
            };
            action();
        }

        public void Refresh()
        {
            UpdateVerticalLineCount();
        }

        public List<ISelectBoxItem> TransmitObjects()
        {
            List<ISelectBoxItem> res = new();
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                //Vector3[] corners = new Vector3[4];
                //item.thisNoteRect.GetLocalCorners(corners);
                res.Add(item);
            }

            return res;
        }

        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
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

        public void UpdateVerticalLineCount()
        {
            for (int i = 0; i < verticalLines.Count;)
            {
                RectTransform verticalLine = verticalLines[0];
                verticalLines.Remove(verticalLine);
                Destroy(verticalLine.gameObject);
            }

            int subdivision = GlobalData.Instance.chartEditData.verticalSubdivision;
            Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
            Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
            for (int i = 1; i < subdivision; i++)
            {
                RectTransform newVerticalLine = Instantiate(verticalLinePrefab, transform);
                newVerticalLine.localPosition =
                    (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) *
                    Vector2.right;
                newVerticalLine.SetSiblingIndex(4);
                verticalLines.Add(newVerticalLine);
            }
        }

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
            note.holdBeats = new BPM();
            note.effect = noteEffect;
            note.positionX =
                (nearVerticalLine.localPosition.x +
                 (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) /
                (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
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

        private void AddNoteAndRefresh(Note note, int boxID, int lineID)
        {
            LogCenter.Log(
                $"{boxID}号框{lineID}号线新增{note.noteType}音符，打击时间为:{note.HitBeats.integer}:{note.HitBeats.molecule}/{note.HitBeats.denominator}");
            ChartTool.AddNoteEdit2ChartData(note, boxID, lineID, GlobalData.Instance.chartEditData,
                GlobalData.Instance.chartData);
            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            onBoxRefreshed(currentBoxID);
        }

        private void FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine)
        {
            nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log(
                    $@"{noteEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)noteEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis = Vector2.Distance(MousePositionInThisRectTransform,
                    (Vector2)noteEditRect.InverseTransformPoint(item.transform.position) +
                    labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }

            nearVerticalLine = null;
            float nearVerticalLineDis = float.MaxValue;
            foreach (RectTransform item in verticalLines)
            {
                float dis = Vector2.Distance(MousePositionInThisRectTransform,
                    (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearVerticalLineDis)
                {
                    nearVerticalLineDis = dis;
                    nearVerticalLine = item;
                }
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

        public IEnumerator WaitForPressureAgain(Scenes.Edit.NoteEdit newHoldEdit, int boxID, int lineID)
        {
            while (true)
            {
                if (waitForPressureAgain)
                {
                    break;
                }

                FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);

                newHoldEdit.thisNoteRect.sizeDelta = new Vector2(newHoldEdit.thisNoteRect.sizeDelta.x,
                    nearBeatLine.transform.localPosition.y - newHoldEdit.transform.localPosition.y);
                newHoldEdit.thisNoteData.holdBeats =
                    new BPM(new BPM(nearBeatLine.thisBPM) - new BPM(newHoldEdit.thisNoteData.HitBeats));
                yield return new WaitForEndOfFrame();
            }

            waitForPressureAgain = false;

            if (newHoldEdit.thisNoteData.holdBeats.ThisStartBPM <= .0001f)
            {
                Debug.LogError("哒咩哒咩，长度为0的Hold！");
                LogCenter.Log("用户尝试放置长度为0的Hold音符");
                Destroy(newHoldEdit.gameObject);
            }
            else
            {
                notes.Add(newHoldEdit);
                //添加事件到对应的地方
                AddNoteAndRefresh(newHoldEdit.thisNoteData, boxID, lineID);
            }
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
            note.holdBeats = new BPM();
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

        private void NoteCopy()
        {
            if (noteClipboard.Count > 0)
            {
                for (int i = 0; i < otherLineNoteClipboard.Count; i++)
                {
                    Destroy(otherLineNoteClipboard[i].gameObject);
                }

                otherLineNoteClipboard.Clear();
            }

            foreach (Scenes.Edit.NoteEdit item in noteClipboard)
            {
                Scenes.Edit.NoteEdit instNewNoteEditPrefab = item.thisNoteData.noteType switch
                {
                    NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                    NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                    NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                    NoteType.Point => GlobalData.Instance.pointEditPrefab,
                    NoteType.Hold => GlobalData.Instance.holdEditPrefab,
                    NoteType.FullFlickPink => GlobalData.Instance.fullFlickEditPrefab,
                    NoteType.FullFlickBlue => GlobalData.Instance.fullFlickEditPrefab,
                    _ => throw new Exception("怎么回事呢···有非通用note代码进入了通用生成note的通道")
                };
                Scenes.Edit.NoteEdit noteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas)
                    .Init(new Note(item.thisNoteData));
                noteEdit.gameObject.SetActive(false);
                otherLineNoteClipboard.Add(noteEdit);
                item.thisNoteData.isSelected = false;
            }
        }

        public void RefreshNotes(int boxID, int lineID)
        {
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            LogCenter.Log($"成功更改框号为{currentBoxID}｜线号为{currentLineID}");
            if (boxID >= 0 || lineID >= 0)
            {
                NoteCopy();
            }

            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                Destroy(item.gameObject);
            }

            notes.Clear();
            List<Note> needInstNotes =
                GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
            foreach (Note item in needInstNotes)
            {
                Scenes.Edit.NoteEdit noteEditType = item.noteType switch
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

                float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(item.HitBeats.ThisStartBPM);
                float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                Scenes.Edit.NoteEdit newNoteEdit = Instantiate(noteEditType, basicLine.noteCanvas).Init(item);
                newNoteEdit.labelWindow = labelWindow;
                newNoteEdit.transform.localPosition = new Vector3(
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) * item.positionX,
                    positionY);
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

                //Debug.LogError("写到这里了，下次继续写");
                notes.Add(newNoteEdit);
            }

            onNoteRefreshed(notes);
        }
    }
}