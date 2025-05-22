using System.Collections;
using System.Collections.Generic;
using CustomSystem;
using Data.ChartEdit;
using Log;
using UnityEngine;

namespace Form.NoteEdit
{
    public partial class NoteEdit
    {
        private void FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine)
        {
            nearBeatLine = FindNearBeatLine(MousePositionInThisRectTransform);

            nearVerticalLine = FindNearVerticalLine(MousePositionInThisRectTransform);
        }

        private RectTransform FindNearVerticalLine(Vector2 mousePosition)
        {
            RectTransform nearVerticalLine = null;
            float nearVerticalLineDis = float.MaxValue;
            foreach (RectTransform item in verticalLines)
            {
                float dis = Vector2.Distance(mousePosition,
                    (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearVerticalLineDis)
                {
                    nearVerticalLineDis = dis;
                    nearVerticalLine = item;
                }
            }

            return nearVerticalLine;
        }

        private BeatLine FindNearBeatLine(Vector2 mousePosition)
        {
            BeatLine nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                float dis = Vector2.Distance(mousePosition,
                    (Vector2)noteEditRect.InverseTransformPoint(item.transform.position) +
                    labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearBeatLineDis)
                {
                    nearBeatLineDis = dis;
                    nearBeatLine = item;
                }
            }

            return nearBeatLine;
        }

        public IEnumerator WaitForPressureAgain(Scenes.Edit.NoteEdit newHoldEdit, int boxID, int lineID)
        {
            Note newNote = newHoldEdit.thisNoteData;
            while (true)
            {
                if (waitForPressureAgain)
                {
                    break;
                }

                FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);
                try
                {
                    newHoldEdit.thisNoteRect.sizeDelta = new Vector2(newHoldEdit.thisNoteRect.sizeDelta.x,
                        nearBeatLine.transform.localPosition.y - newHoldEdit.transform.localPosition.y);
                    newHoldEdit.thisNoteData.holdBeats =
                        new BPM(new BPM(nearBeatLine.thisBPM) - new BPM(newHoldEdit.thisNoteData.HitBeats));
                }
                catch
                {
                }

                yield return new WaitForEndOfFrame();
            }

            waitForPressureAgain = false;

            if (newHoldEdit.thisNoteData.holdBeats.ThisStartBPM <= .0001f)
            {
                Debug.LogError("哒咩哒咩，长度为0的Hold！");
                LogCenter.Log("用户尝试放置长度为0的Hold音符");
                Destroy(newHoldEdit.gameObject);
            }
            else
            {
                notes.Add(newHoldEdit);
                //添加事件到对应的地方
                AddNote(newHoldEdit.thisNoteData, boxID, lineID);
            }

            Steps.Instance.Add(Undo, Redo, default);
            yield break;

            void Undo()
            {
                DeleteNote(newNote, currentBoxID, currentLineID);
            }

            void Redo()
            {
                List<Note> instNewNotes = AddNotes(new List<Note> { newNote }, currentBoxID, currentLineID);
                BatchNotes(instNewNotes, note => note.isSelected = false);
                notes.AddRange(AddNotes2UI(instNewNotes));
            }
        }

        private void NoteEdit_onNoteRefreshed(List<Note> notes)
        {
        }

        private void DestroyNotes()
        {
            foreach (Scenes.Edit.NoteEdit item in notes)
            {
                Destroy(item.gameObject);
            }

            notes.Clear();
        }

        private void SetState2False(int lineID, int boxID)
        {
            int targetLineID = lineID < 0 ? currentLineID : lineID;
            int targetBoxID = boxID < 0 ? currentBoxID : boxID;
            if (targetBoxID == currentBoxID && targetLineID == currentLineID)
            {
                return;
            }

            foreach (Scenes.Edit.NoteEdit noteEdit in notes)
            {
                noteEdit.thisNoteData.isSelected = false;
            }

            //调用UnsetAll方法
            selectBox.NotePropertyEdit.UnsetAll();
        }
    }
}