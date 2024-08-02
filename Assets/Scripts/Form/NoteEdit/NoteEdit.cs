using Data.ChartData;
using Data.ChartEdit;
using Scenes.DontDestoryOnLoad;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Algorithm;
using GlobalData = Scenes.DontDestoryOnLoad.GlobalData;

public class NoteEdit : LabelWindowContent,IInputEventCallback,IRefresh,IRefreshCanvas
{
    public RectTransform noteEditRect;

    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public RectTransform verticalLinePrefab;
    public List<RectTransform> verticalLines = new();

    public BasicLine basicLine;

    public List<Scenes.Edit.NoteEdit> notes = new();
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
            newVerticalLine.SetAsFirstSibling();
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
        BeatLine nearBeatLine = null;
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
        RectTransform nearVerticalLine = new();
        float nearVerticalLineDis = float.MaxValue;
        foreach (RectTransform item in verticalLines)
        {
            float dis = Vector2.Distance(MousePositionInThisRectTransform, (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
            if (dis < nearVerticalLineDis)
            {
                nearVerticalLineDis = dis;
                nearVerticalLine = item;
            }
        }
        Data.ChartEdit.Note note = new();

        note.noteType = NoteType.Tap; 
        note.hitBeats = nearBeatLine.thisBPM;
        note.holdBeats = new();
        note.effect = NoteEffect.CommonEffect | NoteEffect.Ripple;
        note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2)/(verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x)*2-1;
        Scenes.Edit.NoteEdit newNoteEdit = Instantiate(GlobalData.Instance.tapEditPrefab, basicLine.noteCanvas).Init(note);
        newNoteEdit.transform.localPosition = new(nearVerticalLine.localPosition.x,nearBeatLine.transform.localPosition.y);
        //Debug.LogError("写到这里了，下次继续写");
        notes.Add(newNoteEdit);

        GlobalData.Instance.AddNoteEdit2ChartData(note,0,0);
        GlobalData.Refresh();
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

    public void Refresh()
    {
        UpdateVerticalLineCount();
    }

    public void Refresh(int boxID, int lineID)
    {
        foreach (Scenes.Edit.NoteEdit item in notes)
        {
            Destroy(item.gameObject);
        }
        notes.Clear();
        foreach (Data.ChartEdit.Note item in GlobalData.Instance.chartEditData.boxes[boxID].lines[lineID].onlineNotes)
        {

        }
    }
}
