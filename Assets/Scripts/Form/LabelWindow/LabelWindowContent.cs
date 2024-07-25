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
    public Vector2 MousePositionInThisRectTransform => (Vector2)transform.InverseTransformPoint(main.ScreenToWorldPoint(Mouse.current.position.value)) + labelWindow.labelWindowRect.sizeDelta / 2;
    public virtual void WindowSizeChanged(){}
    public virtual void Started(InputAction.CallbackContext callbackContext){ }
    public virtual void Performed(InputAction.CallbackContext callbackContext){}
    public virtual void Canceled(InputAction.CallbackContext callbackContext){}
}
