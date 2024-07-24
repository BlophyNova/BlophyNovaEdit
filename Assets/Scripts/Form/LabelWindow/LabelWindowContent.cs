using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LabelWindowContent : MonoBehaviour,IInputEventCallback
{
    public string labelWindowName;
    public LabelWindowContentType labelWindowContentType;
    public virtual void WindowSizeChanged(){}
    public virtual void Started(InputAction.CallbackContext callbackContext){ }
    public virtual void Performed(InputAction.CallbackContext callbackContext){}
    public virtual void Canceled(InputAction.CallbackContext callbackContext){}
}
