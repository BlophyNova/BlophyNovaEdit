using Scenes.DontDestoryOnLoad;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;

public class EventEdit : LabelWindowContent,IInputEventCallback
{
    public RectTransform eventEditRect;
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
    public override void Started(InputAction.CallbackContext callbackContext)
    {
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        Debug.Log($"{MousePositionInThisRectTransform}");
    }
}
