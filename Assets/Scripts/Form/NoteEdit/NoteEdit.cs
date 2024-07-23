using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteEdit : LabelWindowContent
{
    public RectTransform verticalLineLeft;
    public RectTransform verticalLineRight;
    public RectTransform verticalLinePrefab;
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
            verticalLines.Add(newVerticalLine);
        }
    }
}
