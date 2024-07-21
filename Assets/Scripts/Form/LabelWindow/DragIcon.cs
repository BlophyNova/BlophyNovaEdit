using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Camera;
public class DragIcon : MonoBehaviour
{
    public Image dragIcon;
    public RectTransform labelWindow;
    public Vector2 mouseAndLabelWindowDelta;//意思就是，鼠标和标签窗口位置之间的差值（delta）
    public Vector2 GetMousePosition => (Vector2)Input.mousePosition - new Vector2(Screen.width, Screen.height) / 2;
    private void OnMouseDown()
    {
        mouseAndLabelWindowDelta = GetMousePosition - labelWindow.anchoredPosition;
        
    }
    private void OnMouseDrag()
    {
        labelWindow.anchoredPosition = GetMousePosition -mouseAndLabelWindowDelta;
    }
}
