using Data.ChartEdit;
using Log;
using System.Collections;
using System.Collections.Generic;
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
                Debug.Log(
                    $@"{noteEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)noteEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
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
            while (true)
            {
                if (waitForPressureAgain)
                {
                    break;
                }

                FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine);

                newHoldEdit.thisNoteRect.sizeDelta = new Vector2(newHoldEdit.thisNoteRect.sizeDelta.x,
                    nearBeatLine.transform.localPosition.y - newHoldEdit.transform.localPosition.y);
                newHoldEdit.thisNoteData.holdBeats =
                    new BPM(new BPM(nearBeatLine.thisBPM) - new BPM(newHoldEdit.thisNoteData.HitBeats));
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
        }
        private void NoteEdit_onNoteRefreshed(List<Note> notes)
        {
            noteClipboard.Clear();
            for (int i = 0; i < notes.Count; i++) 
            {
                if (notes[i].isSelected)
                    noteClipboard.Add(notes[i]);
            }
        }
    }
}