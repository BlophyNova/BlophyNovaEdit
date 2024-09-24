using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AddFlick : AddNote
{
    private void Start()
    {
        Init();
    }
    public override void Started(InputAction.CallbackContext callbackContext)
    {
        base.Started(callbackContext);
        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Started(callbackContext);
        }
    }
    public override void Performed(InputAction.CallbackContext callbackContext)
    {
        base.Performed(callbackContext);

        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Performed(callbackContext);
        }
    }
    public override void Canceled(InputAction.CallbackContext callbackContext)
    {
        base.Canceled(callbackContext);

        if (LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType == LabelWindowContentType.NoteEdit)
        {
            LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(callbackContext);
        }
    }
}
