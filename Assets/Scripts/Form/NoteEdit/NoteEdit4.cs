using Controller;
using Data.ChartData;
using Data.Interface;
using Form.PropertyEdit;
using Log;
using Manager;
using System;
using System.Collections.Generic;
using UnityEngine;
using UtilityCode.ChartTool;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
namespace Form.NoteEdit
{
    //这里放所有的刷新方法
    public partial class NoteEdit
    {
        private void AddNoteAndRefresh(Note note, int boxID, int lineID)
        {
            LogCenter.Log(
                $"{boxID}号框{lineID}号线新增{note.noteType}音符，打击时间为:{note.HitBeats.integer}:{note.HitBeats.molecule}/{note.HitBeats.denominator}");
            ChartTool.AddNoteEdit2ChartData(note, boxID, lineID, GlobalData.Instance.chartEditData,
                GlobalData.Instance.chartData);
            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            onBoxRefreshed(currentBoxID);
        }
        private void NoteCopy()
        {
            if (noteClipboard.Count > 0)
            {
                for (int i = 0; i < otherLineNoteClipboard.Count; i++)
                {
                    Destroy(otherLineNoteClipboard[i].gameObject);
                }

                otherLineNoteClipboard.Clear();
            }

            foreach (Scenes.Edit.NoteEdit item in noteClipboard)
            {
                Scenes.Edit.NoteEdit instNewNoteEditPrefab = item.thisNoteData.noteType switch
                {
                    NoteType.Tap => GlobalData.Instance.tapEditPrefab,
                    NoteType.Drag => GlobalData.Instance.dragEditPrefab,
                    NoteType.Flick => GlobalData.Instance.flickEditPrefab,
                    NoteType.Point => GlobalData.Instance.pointEditPrefab,
                    NoteType.Hold => GlobalData.Instance.holdEditPrefab,
                    NoteType.FullFlickPink => GlobalData.Instance.fullFlickEditPrefab,
                    NoteType.FullFlickBlue => GlobalData.Instance.fullFlickEditPrefab,
                    _ => throw new Exception("怎么回事呢···有非通用note代码进入了通用生成note的通道")
                };
                Scenes.Edit.NoteEdit noteEdit = Instantiate(instNewNoteEditPrefab, basicLine.noteCanvas)
                    .Init(item.thisNoteData);
                noteEdit.gameObject.SetActive(false);
                otherLineNoteClipboard.Add(noteEdit);
                item.thisNoteData.isSelected = false;
            }
        }

        public void RefreshNotes(int boxID, int lineID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            lastLineID = boxID < 0 ? lastLineID : currentLineID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID;
            LogCenter.Log($"成功更改框号为{currentBoxID}｜线号为{currentLineID}");
            if (boxID >= 0 || lineID >= 0)
            {
                NoteCopy();
            }

            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                Destroy(item.gameObject);
            }

            notes.Clear();
            List<Note> needInstNotes =
                GlobalData.Instance.chartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes;
            foreach (Note item in needInstNotes)
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
                    _ => throw new Exception("滴滴~滴滴~错误~找不到音符拉~")
                };

                float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(item.HitBeats.ThisStartBPM);
                float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                Scenes.Edit.NoteEdit newNoteEdit = Instantiate(noteEditType, basicLine.noteCanvas).Init(item);
                newNoteEdit.labelWindow = labelWindow;
                newNoteEdit.transform.localPosition = new Vector3(
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) * item.positionX,
                    positionY);
                if (item.noteType == NoteType.Hold)
                {
                    //float endBeatsSecondsTime = BPMManager.Instance.GetSecondsTimeWithBeats(item.EndBeats.ThisStartBPM);
                    //float endBeatsPositionY = YScale.Instance.GetPositionYWithSecondsTime(item.EndBeats.ThisStartBPM);
                    //float hitBeatsPositionY= YScale.Instance.GetPositionYWithSecondsTime(item.HitBeats.ThisStartBPM);
                    float holdBeatsPositionY =
                        YScale.Instance.GetPositionYWithSecondsTime(
                            BPMManager.Instance.GetSecondsTimeByBeats(item.holdBeats.ThisStartBPM));
                    newNoteEdit.thisNoteRect.sizeDelta =
                        new Vector2(newNoteEdit.thisNoteRect.sizeDelta.x, holdBeatsPositionY);
                }

                //Debug.LogError("写到这里了，下次继续写");
                notes.Add(newNoteEdit);
            }

            onNoteRefreshed(notes);
        }
        private void RefreshNoteEditAndChartPreview()
        {
            //ChartTool.ConvertEditLine2ChartDataLine(GlobalData.Instance.chartEditData.boxes[currentBoxID],
            //    GlobalData.Instance.chartData.boxes[currentBoxID], currentLineID);
            GlobalData.Instance.chartData.boxes[currentBoxID] =
                ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID]);
            onBoxRefreshed(currentBoxID);
            //ChartTool.ConvertEditBox2ChartDataBox(GlobalData.Instance.chartEditData.boxes[currentBoxID])
            RefreshNotes(-1, -1);
            SpeckleManager.Instance.allLineNoteControllers.Clear();
            GameController.Instance.RefreshChartPreview();
            GlobalData.Refresh<IRefresh>(interfaceMethod => interfaceMethod.Refresh());
            GlobalData.Refresh<IRefreshUI>(interfaceMethod => interfaceMethod.RefreshUI());
        }
        private void NoteEdit_onNoteRefreshed(List<Scenes.Edit.NoteEdit> notes)
        {
            noteClipboard.Clear();
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                if (item.thisNoteData.isSelected)
                {
                    noteClipboard.Add(item);
                }
            }
        }
    }
}
