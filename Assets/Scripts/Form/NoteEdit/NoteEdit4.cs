using Controller;
using Data.ChartData;
using Data.Interface;
using Form.PropertyEdit;
using Log;
using Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UtilityCode.ChartTool;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
using static UtilityCode.ChartTool.ChartTool;
namespace Form.NoteEdit
{
    //这里放所有的刷新方法
    public partial class NoteEdit
    {
        public void RefreshAll()
        {
            RefreshNotes(currentBoxID, currentLineID);
            RefreshPlayer(currentBoxID, currentLineID);
        }
        public void RefreshNotes(int boxID,int lineID)
        {
            lastBoxID = boxID < 0 ? lastBoxID : currentBoxID;
            lastLineID = boxID < 0 ? lastLineID : currentLineID;
            currentBoxID = boxID < 0 ? currentBoxID : boxID;
            currentLineID = lineID < 0 ? currentLineID : lineID; 
            if (boxID >= 0 || lineID >= 0)
            {
                NoteCopy();
            }
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                Destroy(item.gameObject);
            }

            notes.Clear();
            List<Note> needInstNotes = ChartEditData.boxes[currentBoxID].lines[currentLineID].onlineNotes; 
            foreach (Note item in needInstNotes)
            {
                Scenes.Edit.NoteEdit noteEditType = GetNoteType(item);

                float currentSecondsTime = BPMManager.Instance.GetSecondsTimeByBeats(item.HitBeats.ThisStartBPM);
                float positionY = YScale.Instance.GetPositionYWithSecondsTime(currentSecondsTime);

                Scenes.Edit.NoteEdit newNoteEdit = Instantiate(noteEditType, basicLine.noteCanvas).Init(item);
                newNoteEdit.labelWindow = labelWindow;
                newNoteEdit.transform.localPosition = new Vector3(
                    (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x -
                     (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) * item.positionX,
                    positionY);
                item.chartEditNote = newNoteEdit;
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
        }

        private Scenes.Edit.NoteEdit GetNoteType(Note item)
        {
            return item.noteType switch
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
        }

        public void RefreshPlayer(int boxID,int lineID)
        {
            ConvertLine(ChartEditData.boxes[boxID].lines[lineID].onlineNotes, ChartData.boxes[boxID].lines[lineID].onlineNotes);
        }

        private void NoteCopy()
        {
            if (noteClipboard.Count <= 0)
            {
                return;
            }
            otherLineNoteClipboard.Clear();
            for (int i = 0; i < noteClipboard.Count; i++)
            {
                otherLineNoteClipboard.Add(noteClipboard[i]);
            }
        }
    }
}
