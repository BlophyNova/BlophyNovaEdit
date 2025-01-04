using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Data.ChartEdit;
using Data.Interface;
using Log;
using Manager;
using Scenes.DontDestroyOnLoad;
using UnityEngine;
using UtilityCode.ChartTool;
using Data.ChartData;
using Form.LabelWindow;
using Form.PropertyEdit;
using Scenes.Edit;
using Scenes.PublicScripts;
using TMPro;
using UnityEngine.InputSystem;
using GlobalData = Scenes.DontDestroyOnLoad.GlobalData;
using Note = Data.ChartEdit.Note;
namespace Form.NoteEdit
{
    public partial class NoteEdit
    {


        private void FindNearBeatLineAndVerticalLine(out BeatLine nearBeatLine, out RectTransform nearVerticalLine)
        {
            nearBeatLine = null;
            float nearBeatLineDis = float.MaxValue;
            //第一次
            foreach (BeatLine item in basicLine.beatLines)
            {
                Debug.Log(
                    $@"{noteEditRect.InverseTransformPoint(item.transform.position)}||{item.transform.position}||{(Vector2)noteEditRect.InverseTransformPoint(item.transform.position) + labelWindow.labelWindowRect.sizeDelta / 2}");
                float dis = Vector2.Distance(MousePositionInThisRectTransform,
                    (Vector2)noteEditRect.InverseTransformPoint(item.transform.position) +
                    labelWindow.labelWindowRect.sizeDelta / 2);
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
                float dis = Vector2.Distance(MousePositionInThisRectTransform,
                    (Vector2)item.transform.localPosition + labelWindow.labelWindowRect.sizeDelta / 2);
                if (dis < nearVerticalLineDis)
                {
                    nearVerticalLineDis = dis;
                    nearVerticalLine = item;
                }
            }
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
                AddNoteAndRefresh(newHoldEdit.thisNoteData, boxID, lineID);
            }
        }

    }
}