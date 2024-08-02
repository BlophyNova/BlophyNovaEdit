using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyEdit : LabelWindowContent
{
    public GridLayoutGroup gridLayoutGroup;
    public BoxID boxID;
    public LineID lineID;
    public VerticalLineCount verticalLineCount;
    public PlaySpeed playSpeed;
    public Offset offset;
    public YScale yScale;
    public LoopPlayback loopPlayback;
    public override void WindowSizeChanged()
    {
        base.WindowSizeChanged();
        Debug.Log(new Vector2(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y));
        gridLayoutGroup.cellSize = new(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y);
    }
}
