using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace ShortcutKey.Events
{
    public class SelectBox : ShortcutKeyEventBase
    {
        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        public override void Started(InputAction.CallbackContext callbackContext)
        {
            base.Started(callbackContext); 

            LabelWindowContentType labelWindowContentType = LabelWindowContentType.NoteEdit | LabelWindowContentType.EventEdit;

            if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType))
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Started(callbackContext);
            }
        }

        public override void Performed(InputAction.CallbackContext callbackContext)
        {
            base.Performed(callbackContext);

            LabelWindowContentType labelWindowContentType = LabelWindowContentType.NoteEdit | LabelWindowContentType.EventEdit;

            if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType))
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Performed(callbackContext);
            }
        }

        public override void Canceled(InputAction.CallbackContext callbackContext)
        {
            base.Canceled(callbackContext);

            LabelWindowContentType labelWindowContentType = LabelWindowContentType.NoteEdit | LabelWindowContentType.EventEdit;

            if (labelWindowContentType.HasFlag(LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.labelWindowContentType))
            {
                LabelWindowsManager.Instance.currentFocusWindow.currentLabelItem.labelWindowContent.Canceled(callbackContext);
            }
        }
    }
}