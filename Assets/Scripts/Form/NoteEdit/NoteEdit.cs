using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NoteEdit : LabelWindowContent,IInputEventCallback
{
    public RectTransform noteEditRect;

    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public RectTransform verticalLinePrefab;
    public List<RectTransform> verticalLines = new();

    public BasicLine basicLine;
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
        for (int i = 0; i < verticalLines.Count;)
        {
            var verticalLine = verticalLines[0];
            verticalLines.Remove(verticalLine);
            Destroy(verticalLine.gameObject);
        }
        int subdivision = GlobalData.Instance.chartEditData.verticalSubdivision;
        Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
        Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
        for (int i = 1; i < subdivision; i++)
        {
            RectTransform newVerticalLine = Instantiate(verticalLinePrefab, transform);
            newVerticalLine.localPosition = (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) * Vector2.right;
            verticalLines.Add(newVerticalLine);
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
        Debug.Log($"{MousePositionInThisRectTransform}||{callbackContext.action.name}");
        Action action = callbackContext.action.name switch
        {
            "AddTap" => () => AddNewTap(),
            "AddHold" => () => AddNewHold(),
            "AddDrag" => () => AddNewDrag(),
            "AddFlick" => () => AddNewFlick(),
            "AddPoint" => () => AddNewPoint(),
            "AddFullFlick" => () => AddNewFullFlick(),
            _ => () => Alert.EnableAlert($"欸···？怎么回事，怎么会找不到你想添加的是哪个音符呢···")
        };
        action();
    }
    public void AddNewTap() 
    {
        BeatLine nearBeatLine = new();
        float nearBeatLineDis = float.MaxValue;
        //第一次
        foreach (BeatLine item in basicLine.beatLines)
        {
            Debug.Log($@"{noteEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)noteEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
            float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)noteEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2);
            if (dis < nearBeatLineDis)
            {
                nearBeatLineDis = dis;
                nearBeatLine = item;
            }
        }
        RectTransform nearEventVerticalLine = new();
        float nearEventVerticalLineDis = float.MaxValue;
        foreach (RectTransform item in verticalLines)
        {
            float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
            if (dis < nearEventVerticalLineDis)
            {
                nearEventVerticalLineDis = dis;
                nearEventVerticalLine = item;
            }
        }
        Note note = new();
        note.hitBeats = nearBeatLine.thisBPM;
        note.noteType = NoteType.Tap;
        note.effect = NoteEffect.CommonEffect | NoteEffect.Ripple;
        note.positionX = (nearEventVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2)/(verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x)*2-1;
        Instantiate(GlobalData.Instance.tapEditPrefab, basicLine.noteCanvas).Init(note);
        Debug.LogError("写到这里了，下次继续写");
    }

    public void AddNewHold() 
    {

    }

    public void AddNewDrag() 
    {

    }

    public void AddNewFlick() 
    {

    }

    public void AddNewPoint()
    {

    }

    public void AddNewFullFlick() 
    { 

    }
}
