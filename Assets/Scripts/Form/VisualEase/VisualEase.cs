using System.Collections;
using System.Collections.Generic;
using Form.NotePropertyEdit;
using Scenes.DontDestroyOnLoad;
using UnityEngine;

public class VisualEase : LabelWindowContent,IRefresh
{
    public LineRenderer lineRenderer;
    public RectTransform lineRendererRectTransform;

    void Start()
    {
        lineRenderer.positionCount = 100;
    }

    public void Refresh()
    {
        if (labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent.labelWindowContentType ==
            LabelWindowContentType.NotePropertyEdit)
        {
            NotePropertyEdit notePropertyEdit = (NotePropertyEdit)labelWindow.associateLabelWindow.currentLabelItem.labelWindowContent;
            EaseData ease = GlobalData.Instance.easeDatas[notePropertyEdit.ease.value];
            Vector3[] positions = new Vector3[100];
            Vector3[] corners = new Vector3[4];
            lineRendererRectTransform.GetLocalCorners(corners);
            for (int i = 0; i < positions.Length; i++)
            {
                //positions[i].
                Vector3 currentPosition = (corners[2] - corners[0]) * (i / (float)positions.Length)+corners[0];
                currentPosition.y = ease.thisCurve.Evaluate(i / (float)positions.Length) * (corners[2].y - corners[0].y)+corners[0].y;
                currentPosition.z = -1;
                positions[i] = currentPosition;
            }
            lineRenderer.SetPositions(positions);
        }
    }
}
