using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Camera;
public class Stretch : MonoBehaviour
{
    public RectTransform labelWindowRect;
    public LabelWindow labelWindow;
    public Vector2 GetMousePosition 
    {
        get
        {
            Vector2 ret=(Vector2)main.ScreenToViewportPoint(Input.mousePosition)*main.pixelRect.size;
            Debug.Log($"{main.ScreenToViewportPoint(Input.mousePosition)}");
            ret.y= main.pixelHeight - ret.y;
            float scaleFactorX=1920f/main.pixelWidth;
            float scaleFactorY=1080f/main.pixelHeight;
            ret.Set(ret.x*scaleFactorX,ret.y*scaleFactorY);
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
        size.x = size.x > labelWindow.MaxX ? labelWindow.MaxX : size.x;
        size.y = size.y > labelWindow.MaxY ? labelWindow.MaxY : size.y;
        labelWindowRect.sizeDelta = size;
    }
}
