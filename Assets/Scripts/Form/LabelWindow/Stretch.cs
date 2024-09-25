using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Camera;
public class Stretch : MonoBehaviour
{
    public RectTransform labelWindowRect;
    public LabelWindow labelWindow;
    public BoxCollider2D labelBoxCollider2D;
    public BoxCollider2D contentBoxCollider2D;
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
    private void Start()
    {
        UpdateDragArea(labelWindowRect.sizeDelta);
    }
    private void OnMouseDown()
    {
        labelWindow.transform.SetAsLastSibling();
        LabelWindowsManager.Instance.SetFocusWindow(labelWindow);
    }
    private void OnMouseDrag()
    {
        Vector2 labelWindowPosition = labelWindowRect.anchoredPosition;
        labelWindowPosition.Set(Mathf.Abs(labelWindowPosition.x), Mathf.Abs(labelWindowPosition.y));
        Vector2 size = GetMousePosition - labelWindowPosition;
        size.x = size.x < labelWindow.MinX ? labelWindow.MinX : size.x;
        size.y = size.y < labelWindow.MinY ? labelWindow.MinY : size.y;
        size.x = size.x > labelWindow.MaxX ? labelWindow.MaxX : size.x;
        size.y = size.y > labelWindow.MaxY ? labelWindow.MaxY : size.y;
        labelWindowRect.sizeDelta = size;
        //labelWindow.vectrosityLineMask.rectTransform.anchoredPosition = labelWindowRect.anchoredPosition;
        UpdateDragArea(size);

        foreach (LabelItem item in labelWindow.labels)
            item.labelWindowContent.WindowSizeChanged();
        labelWindow.WindowSizeChanged();
    }

    private void UpdateDragArea(Vector2 size)
    {
        labelBoxCollider2D.size = new(size.x, 30);
        labelBoxCollider2D.offset = new(size.x / 2, -15);
        contentBoxCollider2D.size=new(labelWindowRect.sizeDelta.x,labelWindowRect.sizeDelta.y-30);
    }
}
