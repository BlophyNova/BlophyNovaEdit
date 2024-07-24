using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UtilityCode.Singleton;

public class ShortcutKeyEventBase : MonoBehaviour, IInputEventCallback
{
    public string inputActionName;
    public void Init()
    {
        InputAction inputAction =ShortcutKeyManager.Instance.playerInput.actions[inputActionName];
        inputAction.started += callbackContext => Started(callbackContext);
        inputAction.performed += callbackContext => Performed(callbackContext);
        inputAction.canceled += callbackContext => Canceled(callbackContext);
        inputAction.Enable();
    }
    public virtual void Started(InputAction.CallbackContext callbackContext) 
    {
        Debug.Log($"{inputActionName}.Started被调用！");
    }
    public virtual void Performed(InputAction.CallbackContext callbackContext) 
    {

        Debug.Log($"{inputActionName}.Performed被调用！");
    }
    public virtual void Canceled(InputAction.CallbackContext callbackContext) 
    {
        Debug.Log($"{inputActionName}.Canceled被调用！");
    }

}
