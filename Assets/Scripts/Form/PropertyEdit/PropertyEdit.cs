using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PropertyEdit : LabelWindowContent
{
    public GridLayoutGroup gridLayoutGroup;
    public override void WindowSizeChanged()
    {
        Debug.Log(new Vector2(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y));
        gridLayoutGroup.cellSize = new Vector2(labelWindow.labelWindowRect.sizeDelta.x, gridLayoutGroup.cellSize.y);
    }
}
