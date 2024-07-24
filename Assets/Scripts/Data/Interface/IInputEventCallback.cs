using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInputEventCallback
{
    public void Started(InputAction.CallbackContext callbackContext);
    public void Performed(InputAction.CallbackContext callbackContext);
    public void Canceled(InputAction.CallbackContext callbackContext);
}
