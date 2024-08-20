using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLeft : ShortcutKeyEventBase
{
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
        base.Performed(callbackContext);

        LabelWindowContentType labelWindowContentType = LabelWindowContentType.NoteEdit | LabelWindowContentType.EventEdit;

        if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.labelWindowContentType))
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Performed(callbackContext);
        }
    }
}
