using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cut : ShortcutKeyEventBase
{
    private void Start()
    {
        Init();
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);

        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.EventEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(callbackContext);
        }
        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(callbackContext);
        }
    }
}
