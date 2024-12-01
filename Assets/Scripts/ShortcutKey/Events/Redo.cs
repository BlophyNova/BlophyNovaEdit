using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Redo : ShortcutKeyEventBase
{
    private void Start()
    {
        Init();
    }

    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);
        Steps.Instance.Redo();
    }
}
