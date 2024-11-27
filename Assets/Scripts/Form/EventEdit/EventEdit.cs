using Data.ChartEdit;
using Form.NotePropertyEdit;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using static UnityEngine.Camera;
using EventType = Data.Enumerate.EventType;

public partial class EventEdit : LabelWindowContent, IInputEventCallback, IRefresh, ISelectBox
{
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
    public bool isFirstTime = false;
    public bool waitForPressureAgain = false;
    public float VerticalLineDistance => Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition);
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GlobalData.Instance.chartData.globalData.musicLength > 1);
        Debug.Log($"GlobalData.Instance.chartData.globalData.musicLength:{GlobalData.Instance.chartData.globalData.musicLength}");
        //GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
        RefreshEvents(currentBoxID);
        UpdateVerticalLineCount();
        UpdateNoteLocalPositionAndSize();
        eventLineRenderer = Instantiate(eventLineRendererPrefab, LabelWindowsManager.Instance.lineRendererParent);
        labelWindow.onWindowMoved += LabelWindow_onWindowMoved;
        WindowSizeChanged_EventEdit2();
        LabelWindow_onWindowMoved();
        labelWindow.onWindowLostFocus += LabelWindow_onWindowLostFocus;
        labelWindow.onWindowGetFocus += LabelWindow_onWindowGetFocus;
        labelItem.onLabelGetFocus += LabelItem_onLabelGetFocus;
        labelItem.onLabelLostFocus += LabelItem_onLabelLostFocus;
        Start2();
    }
    private void Update()
    {
        boxIDText.text = $"{currentBoxID}";
    }
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        UpdateVerticalLineCount();
        UpdateNoteLocalPositionAndSize();
        WindowSizeChanged_EventEdit2();
    }
    public void UpdateNoteLocalPositionAndSize()
    {
        for (int i = 0; i < eventEditItems.Count; i++)
        {
            foreach (EventVerticalLine item in eventVerticalLines)
            {
                if (item.eventType == eventEditItems[i].eventType)
                {
                    float positionX = item.transform.localPosition.x;
                    eventEditItems[i].transform.localPosition = new(positionX, YScale.Instance.GetPositionYWithBeats(eventEditItems[i].@event.startBeats.ThisStartBPM));
                    eventEditItems[i].thisEventEditItemRect.sizeDelta = new(Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition), eventEditItems[i].thisEventEditItemRect.sizeDelta.y);
                }
            }
        }
    }
    public void UpdateVerticalLineCount()
    {
        int subdivision = GlobalData.Instance.chartEditData.eventVerticalSubdivision;
        Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
        Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
        for (int i = 1; i < subdivision; i++)
        {
            verticalLines[i - 1].localPosition = (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) * Vector2.right;
        }
        List<RectTransform> allVerticalLines = new(verticalLines);
        allVerticalLines.Insert(0, verticalLineLeft);
        allVerticalLines.Add(verticalLineRight);
        for (int i = 0; i < eventVerticalLines.Count; i++)
        {
            eventVerticalLines[i].transform.localPosition = (allVerticalLines[i + 1].localPosition + allVerticalLines[i].localPosition) / 2;
        }
    }

    public override void Started(InputAction.CallbackContext callbackContext)
    {
        Action action = callbackContext.action.name switch
        {
            "SelectBox" => () => SelectBoxDown(),
            _ => () => Debug.Log($"欸···？怎么回事，怎么会找不到事件呢···")
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
            "AddEvent" => () => AddEvent(),
            "Delete" => () => DeleteEventWithUI(),
            "SelectBox" => () => SelectBoxUp(),
            "Undo" => () => UndoNote(),
            "Redo" => () => RedoNote(),
            "Copy" => () => CopyEvent(),
            "Paste" => () => PasteEvent(),
            "Cut" => () => CutEvent(),
            "MoveUp" => () => MoveUp(),
            "MoveDown" => () => MoveDown(),
            _ => () => Alert.EnableAlert($"欸···？怎么回事，怎么会找不到你想添加的是哪个音符呢···")
        };
        action();
    }

    private void DeleteEventWithUI()
    {
        if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
        {
            NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            List<Data.ChartEdit.Event> events = notePropertyEdit.@event.eventType switch
            {
                EventType.Speed => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
                EventType.Rotate => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
                EventType.Alpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
                EventType.LineAlpha => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
                EventType.MoveX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
                EventType.MoveY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
                EventType.ScaleX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
                EventType.ScaleY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
                EventType.CenterX => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
                EventType.CenterY => GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
                _ => throw new Exception("耳朵耷拉下来，呜呜呜，没找到事件类型")
            };
            if (events.FindIndex(item => item.Equals(notePropertyEdit.@event.@event)) == 0)
            {
                LogCenter.Log($"用户尝试删除{notePropertyEdit.@event.eventType}的第一个事件");
                Alert.EnableAlert("这是第一个事件，不支持删除了啦~");
                return;
            }
            LogCenter.Log($"{notePropertyEdit.@event.eventType}的{notePropertyEdit.@event.@event.startBeats.integer}:{notePropertyEdit.@event.@event.startBeats.molecule}/{notePropertyEdit.@event.@event.startBeats.denominator}事件被删除");
            events.Remove(notePropertyEdit.@event.@event);
            onEventDeleted(notePropertyEdit.@event);
            notePropertyEdit.RefreshEvents();
        }

    }

    private void AddEvent()
    {
        Debug.Log($"{MousePositionInThisRectTransform}");
        if (!isFirstTime)
        {
            isFirstTime = true;
            FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine, out EventVerticalLine nearEventVerticalLine);

            if (nearEventVerticalLine.eventType == EventType.LineAlpha)
            {
                isFirstTime = false;
                return;
            }
            EventEditItem newEventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
            WindowSizeChanged();
            newEventEditItem.labelWindow = labelWindow;
            newEventEditItem.transform.localPosition = new Vector2(nearEventVerticalLine.transform.localPosition.x, nearBeatLine.transform.localPosition.y);
            newEventEditItem.@event.startBeats = new(nearBeatLine.thisBPM);
            newEventEditItem.eventType = nearEventVerticalLine.eventType;
            StartCoroutine(WaitForPressureAgain(newEventEditItem));
        }
        else if (isFirstTime)
        {
            //第二次
            isFirstTime = false;
            waitForPressureAgain = true;
        }
        else {/*报错*/}
    }

    private void FindNearBeatLineAndEventVerticalLine(out BeatLine nearBeatLine, out EventVerticalLine nearEventVerticalLine)
    {
        nearBeatLine = null;
        float nearBeatLineDis = float.MaxValue;
        //第一次
        foreach (BeatLine item in basicLine.beatLines)
        {
            Debug.Log($@"{thisEventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)thisEventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
            float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)thisEventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2);
            if (dis < nearBeatLineDis)
            {
                nearBeatLineDis = dis;
                nearBeatLine = item;
            }
        }
        nearEventVerticalLine = null;
        float nearEventVerticalLineDis = float.MaxValue;
        foreach (EventVerticalLine item in eventVerticalLines)
        {
            float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
            if (dis < nearEventVerticalLineDis)
            {
                nearEventVerticalLineDis = dis;
                nearEventVerticalLine = item;
            }
        }
    }

    public IEnumerator WaitForPressureAgain(EventEditItem eventEditItem)
    {
        while (true)
        {
            if (waitForPressureAgain) break;
            BeatLine nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log($@"{thisEventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)thisEventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)thisEventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }
            eventEditItem.thisEventEditItemRect.sizeDelta = new(eventEditItem.thisEventEditItemRect.sizeDelta.x, nearBeatLine.transform.localPosition.y - eventEditItem.transform.localPosition.y);
            eventEditItem.@event.endBeats = new(nearBeatLine.thisBPM);
            StartCoroutine(eventEditItem.DrawLineOnEEI());
            yield return new WaitForEndOfFrame();
        }
        waitForPressureAgain = false;

        if (eventEditItem.@event.endBeats.ThisStartBPM - eventEditItem.@event.startBeats.ThisStartBPM <= .0001f)
        {
            Debug.LogError("哒咩哒咩，长度为0的Hold！");
            LogCenter.Log($"用户尝试放置长度为0的Hold音符");
            eventEditItems.Remove(eventEditItem);
            Destroy(eventEditItem.gameObject);
        }
        else
        {
            //添加事件到对应的地方
            LogCenter.Log($"{eventEditItem.eventType}新事件：{eventEditItem.@event.startBeats.integer}:{eventEditItem.@event.startBeats.molecule}/{eventEditItem.@event.startBeats.denominator}");
            eventEditItems.Add(eventEditItem);
            AddNewEvent2EventList(eventEditItem);
        }
    }
    public void Refresh()
    {
        UpdateVerticalLineCount();
        RefreshEvents(-1);
    }
    [SerializeField] bool isRef = true;
    public void RefreshEvents(int boxID)
    {
        currentBoxID = boxID < 0 ? currentBoxID : boxID;
        LogCenter.Log($"成功更改框号为{currentBoxID}");
        if (boxID >= 0) EventCopy();
        StartCoroutine(RefreshEvents());

    }
    public void EventCopy()
    {
        if (eventClipboard.Count > 0)
        {
            for (int i = 0; i < otherBoxEventsClipboard.Count; i++)
            {
                Destroy(otherBoxEventsClipboard[i].gameObject);
            }
            otherBoxEventsClipboard.Clear();
        }
        foreach (EventEditItem item in eventClipboard)
        {
            EventEditItem eventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
            eventEditItem.gameObject.SetActive(false);
            eventEditItem.@event = item.@event;
            eventEditItem.eventType = item.eventType;
            otherBoxEventsClipboard.Add(eventEditItem);
            item.@event.IsSelected = false;
        }
    }
    public IEnumerator RefreshEvents()
    {
        yield return new WaitForEndOfFrame();
        foreach (EventEditItem item in eventEditItems)
        {
            Destroy(item.gameObject);
        }
        eventEditItems.Clear();
        if (isRef)
        {
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed, EventType.Speed);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX, EventType.CenterX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY, EventType.CenterY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX, EventType.MoveX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY, EventType.MoveY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX, EventType.ScaleX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY, EventType.ScaleY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate, EventType.Rotate);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha, EventType.Alpha);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha, EventType.LineAlpha);
        }
        UpdateNoteLocalPositionAndSize();
        onEventRefreshed(eventEditItems);
        onBoxRefreshed(currentBoxID);
    }
    void RefreshEvent(List<Data.ChartEdit.Event> events, EventType eventType)
    {
        foreach (Data.ChartEdit.Event @event in events)
        {
            foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
            {
                if (eventVerticalLine.eventType == eventType)
                {
                    EventEditItem newEventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);


                    float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(@event.startBeats.ThisStartBPM);
                    float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                    newEventEditItem.transform.localPosition = new Vector2(eventVerticalLine.transform.localPosition.x, positionY);

                    float endBeatsSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(@event.endBeats.ThisStartBPM);
                    float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(endBeatsSecondsTime);

                    newEventEditItem.labelWindow = labelWindow;
                    newEventEditItem.thisEventEditItemRect.sizeDelta = new(newEventEditItem.thisEventEditItemRect.sizeDelta.x, endBeatsPositionY - positionY);
                    newEventEditItem.@event = @event;
                    newEventEditItem.eventType = eventType;
                    newEventEditItem.SetSelectState(@event.IsSelected);
                    Debug.Log($"{currentBoxID}号方框的{eventVerticalLine.eventType}生成了一个新的eei");
                    eventEditItems.Add(newEventEditItem);
                    newEventEditItem.Init();
                }
            }
        }
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
