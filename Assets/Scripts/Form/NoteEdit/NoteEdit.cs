using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controller;
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
    //由于这个控件需要的功能太多，所以这里做个分类，此文件负责字段事件委托属性，以及Unity生命周期的方法和接口实现的方法
    public partial class NoteEdit : LabelWindowContent, IInputEventCallback, IRefresh, ISelectBox
    {
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
        public List<Scenes.Edit.NoteEdit> notes = new();

        public bool isFirstTime;
        public bool waitForPressureAgain;

        public delegate void OnNoteDeleted(Scenes.Edit.NoteEdit noteEdit);
        public event OnNoteDeleted onNoteDeleted = noteEdit => { };

        public delegate void OnNoteRefreshed(List<Scenes.Edit.NoteEdit> notes);
        public event OnNoteRefreshed onNoteRefreshed = notes => { };

        public List<Scenes.Edit.NoteEdit> otherLineNoteClipboard = new();
        public List<Scenes.Edit.NoteEdit> noteClipboard = new();
        public bool isCopy;

        public delegate void OnBoxRefreshed(object content);
        public event OnBoxRefreshed onBoxRefreshed = content => { };

        private void Start()
        {
            RefreshNotes(currentBoxID, currentLineID);
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
            onNoteRefreshed += NoteEdit_onNoteRefreshed;
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
            RefreshNotes(-1, -1);
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
    }
}