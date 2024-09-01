using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mirror : ShortcutKeyEventBase
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);
        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Canceled(callbackContext);
        }
    }
}
