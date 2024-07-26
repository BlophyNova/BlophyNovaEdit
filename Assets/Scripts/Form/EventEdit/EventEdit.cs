using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Camera;

public class EventEdit : LabelWindowContent,IInputEventCallback
{
    public BasicLine basicLine;
    public RectTransform eventEditRect;
    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public List<RectTransform> verticalLines = new();
    public List<EventVerticalLine> eventVerticalLines = new();
    public bool isFirstTime = false;
    public bool waitForPressureAgain = false;
    private void Start()
    {
        UpdateVerticalLineCount();
    }
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        UpdateVerticalLineCount();
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
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        Debug.Log($"{MousePositionInThisRectTransform}");
        if (!isFirstTime)
        {
            isFirstTime = true;
            BeatLine nearBeatLine=new();
            float nearBeatLineDis= float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log($@"{eventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis= Vector2.Distance(MousePositionInThisRectTransform, (Vector2)eventEditRect.InverseTransformPoint(item.transform.position)+ labelWindow.labelWindowRect.sizeDelta /2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }
            EventVerticalLine nearEventVerticalLine = new();
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
            EventEditItem newEventEditItem =Instantiate(GlobalData.Instance.eventEditItem, basicLine.noteCanvas);
            newEventEditItem.transform.localPosition = new Vector2(nearEventVerticalLine.transform.localPosition.x,nearBeatLine.transform.localPosition.y);
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
            BeatLine nearBeatLine = new();
            float nearBeatLineDis = float.MaxValue;
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log($@"{eventEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)eventEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }
            eventEditItem.rectTransform.sizeDelta = new(eventEditItem.rectTransform.sizeDelta.x, nearBeatLine.transform.localPosition.y - eventEditItem.transform.localPosition.y);
            eventEditItem.@event.endBeats = new(nearBeatLine.thisBPM);
            yield return new WaitForEndOfFrame();
        }
        waitForPressureAgain = false;

        if (eventEditItem.@event.endBeats.ThisStartBPM - eventEditItem.@event.startBeats.ThisStartBPM <= .0001f)
        {
            Debug.LogError("哒咩哒咩，长度为0的Hold！");
            Destroy(eventEditItem.gameObject);
        }
        else
        {
            //添加事件到对应的地方
        }
    }
}
