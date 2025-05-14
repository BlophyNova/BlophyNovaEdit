using Data.EaseData;
using Data.Enumerate;
using Data.Interface;
using Form.LabelWindow;
using Scenes.DontDestroyOnLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Form.VisualEase
{
    public class VisualEase : LabelWindowContent, IRefresh
    {
        public LineRenderer lineRenderer;
        public RectTransform lineRendererRectTransform;

        private void Start()
        {
            lineRenderer.positionCount = 100;
        }

        public void Refresh()
        {
            if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
                LabelWindowContentType.NotePropertyEdit)
            {
                NotePropertyEdit.NotePropertyEdit notePropertyEdit =
                    (NotePropertyEdit.NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelItem
                        .labelWindowContent;
                //EaseData ease = GlobalData.Instance.easeDatas[notePropertyEdit.EditEvent.easeEdit.value];
                EaseData ease = GlobalData.Instance.easeDatas[0];
                Vector3[] positions = new Vector3[100];
                Vector3[] corners = new Vector3[4];
                lineRendererRectTransform.GetLocalCorners(corners);
                for (int i = 0; i < positions.Length; i++)
                {
                    //positions[i].
                    Vector3 currentPosition = (corners[2] - corners[0]) * (i / (float)positions.Length) + corners[0];
                    currentPosition.y =
                        ease.thisCurve.Evaluate(i / (float)positions.Length) * (corners[2].y - corners[0].y) +
                        corners[0].y;
                    currentPosition.z = -1;
                    positions[i] = currentPosition;
                }

                lineRenderer.SetPositions(positions);
            }
        }
    }
}