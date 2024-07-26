using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Camera;
public class LabelWindowContent : MonoBehaviour,IInputEventCallback
{
    public string labelWindowName;
    public LabelWindow labelWindow;
    public LabelWindowContentType labelWindowContentType;
    public int minX=100;
    public int minY=100;
    public Vector2 MousePositionInThisRectTransform => (Vector2)transform.InverseTransformPoint(main.ScreenToWorldPoint(Mouse.current.position.value)) + labelWindow.labelWindowRect.sizeDelta / 2;
    public virtual void WindowSizeChanged()
    {
        Vector2 windowSize = labelWindow.labelWindowRect.sizeDelta;
        windowSize.x = windowSize.x < minX ? minX : windowSize.x;
        windowSize.y = windowSize.y < minY ? minY : windowSize.y;
        labelWindow.labelWindowRect.sizeDelta = windowSize;
    }
    public virtual void Started(InputAction.CallbackContext callbackContext){ }
    public virtual void Performed(InputAction.CallbackContext callbackContext){}
    public virtual void Canceled(InputAction.CallbackContext callbackContext){}
    private void OnEnable()
    {
        WindowSizeChanged();
    }
}
