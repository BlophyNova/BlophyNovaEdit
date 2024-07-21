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
    public Vector2 GetMousePosition 
    {
        get
        {

            Vector2 ret = (Vector2)Input.mousePosition;
            float scaleFactorX = 1920f / main.pixelWidth;
            float scaleFactorY = 1080f / main.pixelHeight;
            ret.Set(ret.x * scaleFactorX, ret.y * scaleFactorY);
            return ret - new Vector2(main.pixelWidth, main.pixelHeight) / 2;
        }
    }
    private void OnMouseDown()
    {
        mouseAndLabelWindowDelta = GetMousePosition - labelWindow.anchoredPosition;
        
    }
    private void OnMouseDrag()
    {
        Vector2 newPosition = GetMousePosition - mouseAndLabelWindowDelta;
        newPosition.x = newPosition.x < 0 ? 0 : newPosition.x;
        newPosition.y = newPosition.y > 0 ? 0 : newPosition.y;
        labelWindow.anchoredPosition = newPosition;
    }
}
