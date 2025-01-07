using Data.Interface;
using Form.LabelWindow;
using Form.NoteEdit;
using Manager;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using Scenes.PublicScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Form.EventEdit
{
    //由于这个控件需要的功能太多，所以这里做个分类，此文件负责字段事件委托属性，以及Unity生命周期的方法和接口实现的方法
    public partial class EventEdit : LabelWindowContent, IInputEventCallback, IRefresh, ISelectBox
    {
        public int lastBoxID;
        public int currentBoxID;

        public BasicLine basicLine;
        public EventLineRenderer eventLineRendererPrefab;
        public EventLineRenderer eventLineRenderer;
        public RectTransform thisEventEditRect;
        public RectTransform verticalLineLeft;
        public RectTransform verticalLineRight;
        public TMP_Text boxIDText;
        public SelectBox selectBox;
        public List<RectTransform> verticalLines = new();
        public List<EventVerticalLine> eventVerticalLines = new();
        public List<EventEditItem> eventEditItems = new();
        public bool isFirstTime;
        public bool waitForPressureAgain;
        [SerializeField] private bool isRef = true;

        public float VerticalLineDistance =>
            Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition);

        public delegate void OnBoxRefreshed(object content);
        public event OnBoxRefreshed onBoxRefreshed = content => { };
        public delegate void OnEventDeleted(EventEditItem eventEditItem);
        public event OnEventDeleted onEventDeleted = eventEditItem => { };
        public delegate void OnEventRefreshed(List<EventEditItem> eventEditItems);
        public event OnEventRefreshed onEventRefreshed = eventEditItems => { };
        public List<EventEditItem> otherBoxEventsClipboard = new();
        public List<EventEditItem> eventClipboard = new();
        public bool isCopy;
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GlobalData.Instance.chartData.globalData.musicLength > 1);
            labelWindow.onWindowMoved += LabelWindow_onWindowMoved;
            labelWindow.onWindowLostFocus += LabelWindow_onWindowLostFocus;
            labelWindow.onWindowGetFocus += LabelWindow_onWindowGetFocus;
            labelItem.onLabelGetFocus += LabelItem_onLabelGetFocus;
            labelItem.onLabelLostFocus += LabelItem_onLabelLostFocus;
            onEventRefreshed += EventEdit_onEventRefreshed;
            RefreshEditEvents(currentBoxID);
            UpdateVerticalLineCount();
            UpdateNoteLocalPositionAndSize();
            eventLineRenderer = Instantiate(eventLineRendererPrefab, LabelWindowsManager.Instance.lineRendererParent);
            UpdateEventEditItemLineRendererRectSize();
            LabelWindow_onWindowMoved();
        }

        private void Update()
        {
            boxIDText.text = $"框号:{currentBoxID}";
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
            //事件w抬起的时候调用
            Action action = callbackContext.action.name switch
            {
                "AddEvent" => AddEvent,
                "Delete" => DeleteEventWithUI,
                "SelectBox" => SelectBoxUp,
                "Undo" => UndoNote,
                "Redo" => RedoNote,
                "Copy" => CopyEvent,
                "Paste" => PasteEvent,
                "Cut" => CutEvent,
                "MoveUp" => MoveUp,
                "MoveDown" => MoveDown,
                _ => () => Alert.EnableAlert("欸···？怎么回事，怎么会找不到你想添加的是哪个音符呢···")
            };
            action();
        }

        public void Refresh()
        {
            UpdateVerticalLineCount();
            RefreshEditEvents(-1);
        }

        public List<ISelectBoxItem> TransmitObjects()
        {
            List<ISelectBoxItem> res = new();
            foreach (EventEditItem item in eventEditItems)
            {
                //Vector3[] corners = new Vector3[4];
                //item.thisEventEditItemRect.GetLocalCorners(corners);
                res.Add(item);
            }
            return res;
        }
    }
}