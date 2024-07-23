using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventEdit : LabelWindowContent
{

    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public List<RectTransform> verticalLines = new();
    private void Start()
    {
        UpdateVerticalLineCount();
    }
    public override void WindowSizeChanged()
    {
        UpdateVerticalLineCount();
    }
    public void UpdateVerticalLineCount()
    {
        int subdivision = GlobalData.Instance.chartEditData.eventVerticalSubdivision;
        Vector3 verticalLineLeftAndRightDelta = verticalLineRight.localPosition - verticalLineLeft.localPosition;
        Debug.Log($"{verticalLineRight.anchoredPosition}||{verticalLineLeft.anchoredPosition}");
        for (int i = 1; i < subdivision; i++)
        {
            verticalLines[i-1].localPosition = (verticalLineLeftAndRightDelta / subdivision * i - verticalLineLeftAndRightDelta / 2) * Vector2.right;
        }
    }
}
