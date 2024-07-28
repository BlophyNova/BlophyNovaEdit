using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseWheel : ShortcutKeyEventBase
{
    private void Start()
    {
        Init();
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
        base.Performed(callbackContext);

        Debug.Log($"MousePerformed：{callbackContext.ReadValue<float>()}");
    }
}
