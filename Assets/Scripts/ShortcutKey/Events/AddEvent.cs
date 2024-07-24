using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddEvent : AddNote
{
    private void Start()
    {
        Init();
    }
    public override void Started(InputAction.CallbackContext callbackContext)
    {
        base.Started(callbackContext);
        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.EventEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Started(callbackContext);
        }
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
        base.Performed(callbackContext);

        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.EventEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Performed(callbackContext);
        }
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);

        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.labelWindowContentType == LabelWindowContentType.EventEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelWindow.Canceled(callbackContext);
        }
    }
}
