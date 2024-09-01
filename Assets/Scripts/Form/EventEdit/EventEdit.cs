using Data.ChartEdit;
using Form.NotePropertyEdit;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UtilityCode.Algorithm;
using UtilityCode.GameUtility;
using static UnityEngine.Camera;

public partial class EventEdit : LabelWindowContent,IInputEventCallback,IRefresh,ISelectBox
{
    public int currentBoxID;


    public BasicLine basicLine;
    public RectTransform thisEventEditRect;
    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public SelectBox selectBox;
    public List<RectTransform> verticalLines = new();
    public List<EventVerticalLine> eventVerticalLines = new();
    public List<EventEditItem> eventEditItems = new();
    public bool isFirstTime = false;
    public bool waitForPressureAgain = false;
    public float VerticalLineDistance=> Vector2.Distance(verticalLines[0].localPosition, verticalLines[1].localPosition);
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => GlobalData.Instance.chartData.globalData.musicLength > 1);
        Debug.Log($"GlobalData.Instance.chartData.globalData.musicLength:{GlobalData.Instance.chartData.globalData.musicLength}");
        //GlobalData.Instance.chartData.boxes = ChartTool.ConvertChartEdit2ChartData(GlobalData.Instance.chartEditData.boxes);
        RefreshNotes(currentBoxID);
        UpdateVerticalLineCount();
        UpdateNoteLocalPositionAndSize();
    }
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        UpdateVerticalLineCount(); 
        UpdateNoteLocalPositionAndSize();
    }
    public void UpdateNoteLocalPositionAndSize()
    {
        for (int i = 0; i < eventEditItems.Count; i++)
        {
            foreach (EventVerticalLine item in eventVerticalLines)
            {
                if(item.eventType == eventEditItems[i].eventType)
                {
                    float positionX=item.transform.localPosition.x;
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
            verticalLines[i-1].localPosition = (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) * Vector2.right;
        }
        List<RectTransform> allVerticalLines = new(verticalLines);
        allVerticalLines.Insert(0,verticalLineLeft);
        allVerticalLines.Add(verticalLineRight);
        for (int i = 0; i < eventVerticalLines.Count; i++)
        {
            eventVerticalLines[i].transform.localPosition= (allVerticalLines[i + 1].localPosition + allVerticalLines[i].localPosition)/2;
        }
    }

    public override void Started(InputAction.CallbackContext callbackContext)
    {
        Action action = callbackContext.action.name switch
        {
            "SelectBox" =>()=> SelectBoxDown(),
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
            "Delete"=> () => DeleteEvent(),
            "SelectBox"=>()=> SelectBoxUp(),
            _ => () => Alert.EnableAlert($"欸···？怎么回事，怎么会找不到你想添加的是哪个音符呢···")
        };
        action();
    }
    private void DeleteEvent()
    {
        if (labelWindow.associateLabelWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.NotePropertyEdit)
        {
            NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelWindow;
            List<Data.ChartEdit.Event> events = notePropertyEdit.@event.eventType switch
            {
                Data.Enumerate.EventType.Speed=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed,
                Data.Enumerate.EventType.Rotate=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate,
                Data.Enumerate.EventType.Alpha=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha,
                Data.Enumerate.EventType.LineAlpha=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha,
                Data.Enumerate.EventType.MoveX=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX,
                Data.Enumerate.EventType.MoveY=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY,
                Data.Enumerate.EventType.ScaleX=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX,
                Data.Enumerate.EventType.ScaleY=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY,
                Data.Enumerate.EventType.CenterX=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX,
                Data.Enumerate.EventType.CenterY=> GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY,
                _=>throw new Exception("耳朵耷拉下来，呜呜呜，没找到事件类型")
            };
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
            BeatLine nearBeatLine = null;
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
            EventVerticalLine nearEventVerticalLine = null;
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
            EventEditItem newEventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
            eventEditItems.Add(newEventEditItem); WindowSizeChanged();
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
            yield return new WaitForEndOfFrame();
        }
        waitForPressureAgain = false;

        if (eventEditItem.@event.endBeats.ThisStartBPM - eventEditItem.@event.startBeats.ThisStartBPM <= .0001f)
        {
            Debug.LogError("哒咩哒咩，长度为0的Hold！");
            eventEditItems.Remove(eventEditItem);
            Destroy(eventEditItem.easeRenderer.line.rectTransform.gameObject);
            Destroy(eventEditItem.gameObject);
        }
        else
        {
            //添加事件到对应的地方
            AddNewEvent2EventList(eventEditItem);
        }
    }
    public void Refresh()
    {
        UpdateVerticalLineCount();
    }
    [SerializeField] bool isRef = true;
    public void RefreshNotes(int boxID)
    {
        currentBoxID = boxID < 0 ? currentBoxID : boxID;
        StartCoroutine(RefreshNotes());
    }
    public IEnumerator RefreshNotes()
    {
        yield return new WaitForEndOfFrame();
        foreach (EventEditItem item in eventEditItems)
        {
            Destroy(item.easeRenderer.line.rectTransform.gameObject);
            Destroy(item.gameObject);
        }
        eventEditItems.Clear();
        if (isRef)
        {
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.speed, Data.Enumerate.EventType.Speed);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerX, Data.Enumerate.EventType.CenterX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.centerY, Data.Enumerate.EventType.CenterY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveX, Data.Enumerate.EventType.MoveX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.moveY, Data.Enumerate.EventType.MoveY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleX, Data.Enumerate.EventType.ScaleX);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.scaleY, Data.Enumerate.EventType.ScaleY);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.rotate, Data.Enumerate.EventType.Rotate);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.alpha, Data.Enumerate.EventType.Alpha);
            RefreshEvent(GlobalData.Instance.chartEditData.boxes[currentBoxID].boxEvents.lineAlpha, Data.Enumerate.EventType.LineAlpha);
        }
        UpdateNoteLocalPositionAndSize();

    }
    void RefreshEvent(List<Data.ChartEdit.Event> events, Data.Enumerate.EventType eventType)
    {
        foreach (Data.ChartEdit.Event @event in events)
        {
            foreach (EventVerticalLine eventVerticalLine in eventVerticalLines)
            {
                if (eventVerticalLine.eventType == eventType)
                {
                    EventEditItem newEventEditItem = Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);


                    float currentSecondsTime = BPMManager.Instance.GetSecondsTimeWithBeats(@event.startBeats.ThisStartBPM);
                    float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                    newEventEditItem.transform.localPosition = new Vector2(eventVerticalLine.transform.localPosition.x, positionY);

                    float endBeatsSecondsTime = BPMManager.Instance.GetSecondsTimeWithBeats(@event.endBeats.ThisStartBPM);
                    float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(endBeatsSecondsTime);

                    newEventEditItem.labelWindow = labelWindow;
                    newEventEditItem.thisEventEditItemRect.sizeDelta = new(newEventEditItem.thisEventEditItemRect.sizeDelta.x, endBeatsPositionY - positionY);
                    newEventEditItem.@event = @event;
                    newEventEditItem.eventType = eventType;
                    Debug.Log($"{currentBoxID}号方框的{eventVerticalLine.eventType}生成了一个新的eei");
                    eventEditItems.Add(newEventEditItem);
                    newEventEditItem.Init();
                    newEventEditItem.easeRenderer.RefreshUI();
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
