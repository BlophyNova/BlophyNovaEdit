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
using Event = Data.ChartEdit.Event;
using EventType = Data.Enumerate.EventType;
using static UtilityCode.ChartTool.ChartTool;
namespace Form.EventEdit
{
    //由于这个控件需要的功能太多，所以这里做个分类，此文件负责字段事件委托属性，以及Unity生命周期的方法和接口实现的方法
    public partial class EventEdit : LabelWindowContent, IInputEventCallback, IRefresh, ISelectBox,IRefreshEdit,IRefreshPlayer
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

        public delegate void OnEventsAdded(List<Event> events);
        public event OnEventsAdded onEventsAdded = events => { };

        public delegate void OnEventsAdded2UI(List<EventEditItem> eventEditItems);
        public event OnEventsAdded2UI onEventsAdded2UI = eventEditItems => { };

        public delegate void OnEventsDeleted(List<Event> events);
        public event OnEventsDeleted onEventsDeleted = events => { };

        public delegate void OnEventsRefreshed(List<Event> events);
        public event OnEventsRefreshed onEventsRefreshed = events => { };
        //public List<EventEditItem> otherBoxEventsClipboard = new();
        //public List<EventEditItem> eventClipboard = new(); 
        public bool isCopy;
        private IEnumerator Start()
        {
            yield return new WaitUntil(() => GlobalData.Instance.chartData.metaData.musicLength > 1);
            yield return new WaitUntil(() => GlobalData.Instance.chartData.boxes.Count > 0);
            labelWindow.onWindowMoved += LabelWindow_onWindowMoved;
            labelWindow.onWindowLostFocus += LabelWindow_onWindowLostFocus;
            labelWindow.onWindowGetFocus += LabelWindow_onWindowGetFocus;
            labelItem.onLabelGetFocus += LabelItem_onLabelGetFocus;
            labelItem.onLabelLostFocus += LabelItem_onLabelLostFocus;


            UpdateVerticalLineCount();
            UpdateNoteLocalPositionAndSize();
            eventLineRenderer = Instantiate(eventLineRendererPrefab, LabelWindowsManager.Instance.lineRendererParent);
            UpdateEventEditItemLineRendererRectSize();
            LabelWindow_onWindowMoved();

            RefreshEvents(currentBoxID);
            RefreshPlayer(currentBoxID);
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
                "AddEvent" => AddEventFromUI,
                "Delete" => DeleteEventFromUI,
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
        public void Refresh()
        {
            UpdateVerticalLineCount();
            //RefreshEvents(-1);
        }

        public void RefreshEdit(int lineID, int boxID)
        {
            RefreshEvents(boxID);
        }

        public void RefreshPlayer(int lineID,int boxID)
        {
            RefreshPlayer(boxID);
        }

        //public void RefreshAll(int lineID, int boxID)
        //{
        //    //GlobalData.Refresh<IRefreshEdit>(interfaceMethod => interfaceMethod.RefreshEdit(-1, -1), new() { typeof(EventEdit) });
        //    //GlobalData.Refresh<IRefreshPlayer>(interfaceMethod => interfaceMethod.RefreshPlayer(-1, -1), new() { typeof(EventEdit) });
        //}

    }
}