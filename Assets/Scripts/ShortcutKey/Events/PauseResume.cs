using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseResume : ShortcutKeyEventBase
{
    private void Start()
    {
        Init();
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);
        StateManager.Instance.IsPause = !StateManager.Instance.IsPause;
    }
}
