using System;
using System.Collections.Generic;
using Data.ChartEdit;
using Data.Interface;
using Form.LabelWindow;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UtilityCode.ChartTool.ChartTool;

namespace Form.NoteEdit
{
    //由于这个控件需要的功能太多，所以这里做个分类，此文件负责字段事件委托属性，以及Unity生命周期的方法和接口实现的方法
    public partial class NoteEdit : LabelWindowContent, IInputEventCallback, ISelectBox, IRefreshEdit, IRefreshPlayer, IRefreshUI
    {
        public delegate void OnNotesAdded(List<Note> notes);

        public delegate void OnNotesAdded2UI(List<Scenes.Edit.NoteEditItem> notes);

        public delegate void OnNotesDeleted(List<Note> notes);

        public delegate void OnNotesRefreshed(List<Note> notes);

        public int lastBoxID;
        public int lastLineID;
        public int currentBoxID;
        public int currentLineID;

        public float verticalLineDeltaDataForChartData;

        public RectTransform noteEditRect;

        public RectTransform verticalLineLeft;
        public RectTransform verticalLineRight;
        public RectTransform verticalLinePrefab;
        public SelectBox selectBox;
        public BasicLine basicLine;
        public TMP_Text boxAndLineIDText;

        public List<RectTransform> verticalLines = new();
        public List<Scenes.Edit.NoteEditItem> notes = new();

        public bool isFirstTime;
        public bool waitForPressureAgain;


        public bool isCopy;
        public bool isMoving;


        private void Start()
        {
            RefreshEdit(currentBoxID, currentLineID);
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
            onNotesRefreshed += NoteEdit_onNoteRefreshed;
        }

        private void Update()
        {
            boxAndLineIDText.text = $"框号:{currentBoxID}\n线号:{currentLineID}";
        }

        public override void Started(InputAction.CallbackContext callbackContext)
        {
            Action action = callbackContext.action.name switch
            {
                "SelectBox" => SelectBoxDown,
                "Move" => () => Move(true),
                _ => () => Debug.Log("欸···？怎么回事，怎么会找不到快捷键呢···")
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
                "Move" => () => Move(false),
                "MoveUp" => MoveUp,
                "MoveDown" => MoveDown,
                "MoveLeft" => MoveLeft,
                "MoveRight" => MoveRight,
                "MirrorFlip" => MirrorFlip,
                _ => () => Alert.EnableAlert("欸···？怎么回事，怎么会找不到快捷键呢···")
            };
            action();
        }

        public void RefreshEdit(int lineID, int boxID)
        {
            SetState2False(lineID, boxID);
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            lastLineID = boxID < 0 ? lastLineID : currentLineID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            DestroyNotes();
            List<Scenes.Edit.NoteEditItem> newNotes =
                AddNotes2UI(ChartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes);
            notes.AddRange(newNotes);
            List<Note> refreshedNotes = new();
            foreach (Scenes.Edit.NoteEditItem note in notes)
            {
                refreshedNotes.Add(note.thisNoteData);
            }

            onNotesRefreshed(refreshedNotes);
        }

        public void RefreshPlayer(int lineID, int boxID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            lastLineID = boxID < 0 ? lastLineID : currentLineID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            ConvertLine(ChartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes,
                ChartData.boxes[currentBoxID].lines[currentLineID].onlineNotes);
        }

        public void RefreshUI()
        {
            UpdateVerticalLineCount();
        }

        public List<ISelectBoxItem> TransmitObjects()
        {
            List<ISelectBoxItem> res = new();
            foreach (Scenes.Edit.NoteEditItem item in notes)
            {
                //Vector3[] corners = new Vector3[4];
                //item.thisNoteRect.GetLocalCorners(corners);
                res.Add(item);
            }

            return res;
        }

        public event OnNotesAdded onNotesAdded = notes => { };
        public event OnNotesAdded2UI onNotesAdded2UI = notes => { };
        public event OnNotesDeleted onNotesDeleted = notes => { };
        public event OnNotesRefreshed onNotesRefreshed = notes => { };

        //public void RefreshAll(int lineID, int boxID)
        //{
        //    RefreshEdit(lineID, boxID);
        //    RefreshPlayer(lineID, boxID);
        //    //GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1), new() { typeof(NoteEdit) });
        //    //GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), new() { typeof(NoteEdit),typeof(LineNoteController) });
        //}
    }
}