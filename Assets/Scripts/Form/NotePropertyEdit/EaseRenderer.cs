using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
public class EaseRenderer : MonoBehaviour
{
    public EventEditItem eventEditItem;
    private void Start()
    {
        Debug.LogError($"事件画线相关代码，测试代码");
        List<Vector2> linePoints = new()
        {
            Vector2.zero,
            Vector2.one * 540
        };

        // Make a VectorLine object using the above points, with a width of 2 pixels
        VectorLine line = new("Line", linePoints, .05f);
        line.SetCanvas(GetComponentInChildren<Canvas>());
        line.color = new(92,206,250,255);
        //line.SetMask();
        // Draw the line
        line.Draw();
    }
}
