using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Stretch : MonoBehaviour
{
    public RectTransform labelWindowRect;
    public LabelWindow labelWindow;
    //public Vector2 mouseAndLabelWindowDelta;//意思就是，鼠标和标签窗口位置之间的差值（delta）
    public Vector2 GetMousePosition 
    {
        get
        {
            Vector2 ret=(Vector2)Input.mousePosition;
            ret.y=Screen.height-ret.y;
            return ret;
        }
    }
    private void OnMouseDrag()
    {
        Vector2 labelWindowPosition = labelWindowRect.anchoredPosition;
        labelWindowPosition.Set(Mathf.Abs(labelWindowPosition.x),Mathf.Abs(labelWindowPosition.y));
        Vector2 size = GetMousePosition - labelWindowPosition;
        size.x = size.x < labelWindow.minX ? labelWindow.minX : size.x;
        size.y = size.y < labelWindow.minY ? labelWindow.minY : size.y;
        size.x = size.x > labelWindow.maxX ? labelWindow.maxX : size.x;
        size.y = size.y > labelWindow.maxY ? labelWindow.maxY : size.y;
        labelWindowRect.sizeDelta = size;
        Debug.Log($"{GetMousePosition}||{labelWindowRect.anchoredPosition}");
    }
}
