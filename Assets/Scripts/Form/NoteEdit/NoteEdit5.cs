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
    //这里放控件本身的方法
    public partial class NoteEdit
    {
        public override void WindowSizeChanged()
        {
            base.WindowSizeChanged();
            UpdateVerticalLineCount();
            UpdateNoteLocalPosition();
        }
        public void UpdateVerticalLineCount()
        {
            for (int i = 0; i < verticalLines.Count;)
            {
                RectTransform verticalLine = verticalLines[0];
                verticalLines.Remove(verticalLine);
                Destroy(verticalLine.gameObject);
            }

            int subdivision = GlobalData.Instance.chartEditData.verticalSubdivision;
            Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
            Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
            for (int i = 1; i < subdivision; i++)
            {
                RectTransform newVerticalLine = Instantiate(verticalLinePrefab, transform);
                newVerticalLine.localPosition =
                    (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) *
                    Vector2.right;
                newVerticalLine.SetSiblingIndex(4);
                verticalLines.Add(newVerticalLine);
            }
            //note.positionX = (nearVerticalLine.localPosition.x + (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) / 2) / (verticalLineRight.localPosition.x - verticalLineLeft.localPosition.x) * 2 - 1;
            verticalLineDeltaDataForChartData = ((verticalLines[1].localPosition.x - verticalLines[0].localPosition.x) / verticalLineLeftAndRightDelta.x)*2;
        }
    }
}
