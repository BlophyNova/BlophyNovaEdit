using Data.ChartData;
using Data.ChartEdit;
using Scenes.DontDestroyOnLoad;
using Scenes.Edit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Algorithm;
using static TreeEditor.TreeEditorHelper;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;

public class NoteEdit : LabelWindowContent,IInputEventCallback,IRefresh
{

    public int currentBoxID;
    public int currentLineID;



    public RectTransform noteEditRect;

    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public RectTransform verticalLinePrefab;
    public List<RectTransform> verticalLines = new();

    public BasicLine basicLine;

    public List<Scenes.Edit.NoteEdit> notes = new();



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
        AddNewNote(NoteType.Tap, NoteEffect.CommonEffect | NoteEffect.Ripple, currentBoxID,currentLineID);
    }

    private void AddNewNote(NoteType noteType, NoteEffect noteEffect, int boxID, int lineID)
    {
        FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
        Data.ChartEdit.Note note = new();

        note.noteType = noteType;
        note.hitBeats = nearBeatLine.thisBPM;
        note.holdBeats = new();
        note.effect = noteEffect;
        note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
        Scenes.Edit.NoteEdit instNewNoteEditPrefab = note.noteType switch 
        { 
            NoteType.Tap=> GlobalData.Instance.tapEditPrefab,
            NoteType.Drag=>GlobalData.Instance.dragEditPrefab,
            NoteType.Flick=>GlobalData.Instance.flickEditPrefab,
            NoteType.Point=>GlobalData.Instance.pointEditPrefab,
            _ => throw new Exception("怎么回事呢···有非通用note代码进入了通用生成note的通道")
        };
        Scenes.Edit.NoteEdit newNoteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas).Init(note);
        newNoteEdit.transform.localPosition = new(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
        //Debug.LogError("写到这里了，下次继续写");
        notes.Add(newNoteEdit);

        AddNoteAndRedresh(note, boxID,lineID);
    }

    private void AddNoteAndRedresh(Data.ChartEdit.Note note,int boxID,int lineID)
    {
        GlobalData.Instance.AddNoteEdit2ChartData(note, boxID, lineID);
        GlobalData.Refresh<IRefresh>((interfaceMethod) => interfaceMethod.Refresh());
    }

    private void FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine)
    {
        nearBeatLine = null;
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
        nearVerticalLine = null;
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
    }
    public void AddNewHold() 
    {
        Debug.Log($"{MousePositionInThisRectTransform}");
        if (!isFirstTime)
        {
            isFirstTime = true;

            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
            Data.ChartEdit.Note note = new();
            note.noteType = NoteType.Hold;
            note.hitBeats = nearBeatLine.thisBPM;
            note.effect = NoteEffect.Ripple | NoteEffect.CommonEffect;
            note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
            Scenes.Edit.NoteEdit newHoldEdit = Instantiate(GlobalData.Instance.holdEditPrefab,basicLine.noteCanvas).Init(note);
            newHoldEdit.transform.localPosition = new Vector2(nearVerticalLine.transform.localPosition.x, nearBeatLine.transform.localPosition.y);
            StartCoroutine(WaitForPressureAgain(newHoldEdit, currentBoxID, currentLineID));
        }
        else if (isFirstTime)
        {
            //第二次
            isFirstTime = false;
            waitForPressureAgain = true;
        }
        else {/*报错*/}
    }
    public IEnumerator WaitForPressureAgain(Scenes.Edit.NoteEdit newHoldEdit,int boxID,int lineID)
    {
        while (true)
        {
            if (waitForPressureAgain) break;
            FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);

            newHoldEdit.thisNoteRect.sizeDelta =
                new(
                    newHoldEdit.thisNoteRect.sizeDelta.x, 
                    nearBeatLine.transform.localPosition.y -
                    newHoldEdit.transform.localPosition.y);
            newHoldEdit.thisNoteData.holdBeats =new(new BPM( nearBeatLine.thisBPM) - new BPM(newHoldEdit.thisNoteData.hitBeats));
            yield return new WaitForEndOfFrame();
        }
        waitForPressureAgain = false;

        if (newHoldEdit.thisNoteData.holdBeats.ThisStartBPM <= .0001f)
        {
            Debug.LogError("哒咩哒咩，长度为0的Hold！");
            Destroy(newHoldEdit.gameObject);
        }
        else
        {
            notes.Add(newHoldEdit);
            //添加事件到对应的地方
            AddNoteAndRedresh(newHoldEdit.thisNoteData, boxID, lineID);
        }
    }











    public void AddNewFullFlick()
    {
        FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
        Data.ChartEdit.Note note = new();

        note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
        note.noteType = note.positionX switch 
        {
            <= 0 =>NoteType.FullFlickPink,
            > 0 =>NoteType.FullFlickBlue,
            _=>throw new Exception("呜呜呜，怎么找不到究竟是粉色的FullFlick还是蓝色的FullFlick呢...")
        };
        note.hitBeats = nearBeatLine.thisBPM;
        note.holdBeats = new();
        note.effect = NoteEffect.Ripple;
        note.isClockwise = note.positionX switch
        {
            <= 0 => true,
            > 0 => false,
            _ => throw new Exception("呜呜呜，怎么找不到究竟是顺时针还是逆时针呢...")
        };
        Scenes.Edit.NoteEdit newNoteEdit = Instantiate(GlobalData.Instance.fullFlickEditPrefab, basicLine.noteCanvas).Init(note);
        newNoteEdit.transform.localPosition = new(nearVerticalLine.localPosition.x, nearBeatLine.transform.localPosition.y);
        //Debug.LogError("写到这里了，下次继续写");
        notes.Add(newNoteEdit);

        AddNoteAndRedresh(note, currentBoxID, currentLineID);
    }
    public void AddNewDrag()
    {
        AddNewNote(NoteType.Drag, NoteEffect.CommonEffect | NoteEffect.Ripple, currentBoxID, currentLineID);
    }

    public void AddNewFlick()
    {
        AddNewNote(NoteType.Flick, NoteEffect.CommonEffect | NoteEffect.Ripple, currentBoxID, currentLineID);
    }

    public void AddNewPoint()
    {

        AddNewNote(NoteType.Point, NoteEffect.Ripple, currentBoxID, 4);
    }
    public void Refresh()
    {
        UpdateVerticalLineCount();
    }
    public void RefreshNotes(int boxID,int lineID)
    {
        currentBoxID = boxID<0?currentBoxID:boxID;
        currentLineID = lineID<0?currentLineID:lineID;
        foreach (Scenes.Edit.NoteEdit item in notes)
        {
            Destroy(item.gameObject);
        }
        notes.Clear();
        List<Data.ChartEdit.Note> needInstNotes = GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
        foreach (Data.ChartEdit.Note item in needInstNotes)
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
                _=>throw new Exception("滴滴~滴滴~错误~找不到音符拉~")
            };

            float currentSecondsTime = BPMManager.Instance.GetSecondsTimeWithBeats(item.hitBeats.ThisStartBPM);
            float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

            Scenes.Edit.NoteEdit newNoteEdit = Instantiate(noteEditType, basicLine.noteCanvas).Init(item);
            newNoteEdit.transform.localPosition = new(((verticalLineRight.localPosition.x-verticalLineLeft.localPosition.x)- (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x)/2) * item.positionX, positionY);
            //Debug.LogError("写到这里了，下次继续写");
            notes.Add(newNoteEdit);
        }
    }
}
